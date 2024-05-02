using Microsoft.AspNetCore.Components.Forms;

/// <summary>
///  Lavet af Jozsef
/// </summary>
namespace ElevPortalen.Services
{
    public class ImageUploadService
    {
        private readonly IConfiguration Configuration;
        private const int maxAllowedFiles = 1; // max 1 file
        private long maxFileSize = 1024 * 1024 * 1; // represents 1MB

        public ImageUploadService(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public async Task<(bool success, string? message)> LoadFiles(InputFileChangeEventArgs e)
        {
            if (e.FileCount > maxAllowedFiles)
            {
                return (false, $"Error: Attempting to upload file, but only {maxAllowedFiles} files are allowed");
            }
            if (e.File.Size > maxFileSize)
            {
                return (false, $"Error: File size exceeds the maximum allowed size of 1 megabytes");
            }

            foreach (var file in e.GetMultipleFiles(maxAllowedFiles))
            {
                try
                {

                    string newFileName = Path.ChangeExtension(
                        Path.GetRandomFileName(),
                        Path.GetExtension(file.Name));

                    string path = Path.Combine(
                        Configuration.GetValue<string>("FileStorage")!,
                        "",
                        newFileName);

                    Directory.CreateDirectory(Path.GetDirectoryName(path)!);

                    await using FileStream fs = new(path, FileMode.Create);
                    await file.OpenReadStream(maxFileSize).CopyToAsync(fs);

                    return (true, newFileName);
                }
                catch (Exception ex)
                {
                    return (false, $"Error uploading file. {ex.Message}");
                }
            }
            return (false, "No files were uploaded."); // Return false if no files were processed
        }
    }

}


