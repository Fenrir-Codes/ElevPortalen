using Microsoft.AspNetCore.Components.Forms;



namespace ElevPortalen.Services
{
    public class ImageUploadService
    {
        private readonly IConfiguration Configuration;
        private const int maxAllowedFiles = 1; // Adjust this according to your requirements

        public ImageUploadService(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public async Task<(bool success, string? message)> LoadFiles(InputFileChangeEventArgs e)
        {
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
                    await file.OpenReadStream().CopyToAsync(fs);

                    return (true, newFileName);
                }
                catch (Exception ex)
                {
                    return (false, $"Error uploading file '{file.Name}': {ex.Message}");
                }
            }
            return (false, "No files were uploaded."); // Return false if no files were processed
        }
    }

}


