namespace group_5_Mohti_Trilles__Lab_3.Services
{
    public class VideoService : IVideoService
    {
        private readonly string _path;

        public VideoService(IWebHostEnvironment webHostEnvironment)
        {
            _path = Path.Combine(webHostEnvironment.WebRootPath, "Videos");

            Directory.CreateDirectory(_path);
        }
        public void DeleteVideo(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)
                || Path.GetFileName(fileName) != fileName)
            {
                return;
            }

            string filePath = Path.Combine(_path, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public async Task<string> UploadVideoAsync(IFormFile video)
        {
            string fileName = Guid.NewGuid().ToString()
                + Path.GetExtension(video.FileName).ToLowerInvariant();

            string filePath = Path.Combine(_path, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);

            await video.CopyToAsync(stream);

            return fileName;
        }
    }
}
