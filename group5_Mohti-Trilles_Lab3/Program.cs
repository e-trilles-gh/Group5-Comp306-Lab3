using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Extensions.NETCore.Setup;
using group_5_Mohti_Trilles__Lab_3.Model;
using group_5_Mohti_Trilles__Lab_3.Services;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var awsOptions = new Amazon.Extensions.NETCore.Setup.AWSOptions
{
    Region = Amazon.RegionEndpoint.USEast1
};

if (builder.Environment.IsDevelopment())
{
    var credentialPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        ".aws",
        "credentials"
        );

    var profileChain = new Amazon.Runtime.CredentialManagement.CredentialProfileStoreChain(credentialPath);

    if (!profileChain.TryGetAWSCredentials("podcast-statistics-lab3", out var awsCredentials))
    {
        throw new Exception("AWS credentials profile not found.");
    }

    awsOptions.Credentials = awsCredentials;
}

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddDbContext<DatabaseContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("connectServer")));

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
