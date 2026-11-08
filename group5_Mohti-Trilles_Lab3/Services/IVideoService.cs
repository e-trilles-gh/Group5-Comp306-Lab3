namespace group_5_Mohti_Trilles__Lab_3.Services
{
    public interface IVideoService
    {
        Task<string> UploadVideoAsync(IFormFile video);

        void DeleteVideo(string fileName);

        string? GetVideoPath(string fileName);
    }
}
