using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Extensions.NETCore.Setup;
using group_5_Mohti_Trilles__Lab_3.Model;
using group_5_Mohti_Trilles__Lab_3.Services;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Microsoft.EntityFrameworkCore;
using Amazon.Runtime;
using Amazon;
using Amazon.Runtime.CredentialManagement;

var builder = WebApplication.CreateBuilder(args);

var awsRegion = RegionEndpoint.USEast1;

AWSCredentials? awsCredentials = null;

if (builder.Environment.IsDevelopment())
{
    var credentialPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        ".aws",
        "credentials"
        );

    var profileChain = new CredentialProfileStoreChain(credentialPath);

    if (!profileChain.TryGetAWSCredentials("podcast-statistics-lab3", out awsCredentials))
    {
        throw new Exception("AWS credentials profile not found.");
    }
}

//Configure AWS
var awsOptions = new AWSOptions
{
    Credentials = awsCredentials,
    Region = awsRegion
};

builder.Services.AddDefaultAWSOptions(awsOptions);

//DynamoDB
builder.Services.AddAWSService<IAmazonDynamoDB>();
builder.Services.AddScoped<IDynamoDBContext, DynamoDBContext>();
builder.Services.AddScoped<StatisticsService>();

//get connection string from parameter store
string connectionString;

using (var ssmClient = awsCredentials != null
    ? new AmazonSimpleSystemsManagementClient(awsCredentials, awsRegion)
    : new AmazonSimpleSystemsManagementClient(awsRegion))
{
    var parameter = await ssmClient.GetParameterAsync(
        new GetParameterRequest
        {
            Name = "/podcast/ConnectionString",
            WithDecryption = true
        });
    connectionString = parameter.Parameter.Value;
}

//SQL Server connection
builder.Services.AddDbContext<DatabaseContext>(options =>
options.UseSqlServer(connectionString));

// Add services to the container.
builder.Services.AddRazorPages();



builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

builder.Services.AddScoped<IVideoService, VideoService>();

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 150 * 1024 * 1024;
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
