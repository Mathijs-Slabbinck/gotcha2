namespace Gotcha2.API.Constants
{
    // Disk layout for profile-image bytes.
    // Path is resolved relative to IWebHostEnvironment.WebRootPath (= <project>/wwwroot at runtime).
    // Stored as JPEG so the GET endpoint can return a single Content-Type without extension sniffing.
    public static class ProfileImageStorage
    {
        public const string FolderName = "profile-images";
        public const string FileExtension = ".jpg";
        public const string ContentType = "image/jpeg";
    }
}
