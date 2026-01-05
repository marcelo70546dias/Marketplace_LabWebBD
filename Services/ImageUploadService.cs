namespace Marketplace_LabWebBD.Services
{
    public class ImageUploadService : IImageUploadService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<ImageUploadService> _logger;

        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB
        private readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

        public ImageUploadService(IWebHostEnvironment environment, ILogger<ImageUploadService> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        public bool ValidateImage(IFormFile file)
        {
            return GetValidationError(file) == null;
        }

        public string? GetValidationError(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return "Nenhum ficheiro foi selecionado.";

            if (file.Length > MaxFileSize)
                return $"O ficheiro excede o tamanho máximo permitido de {MaxFileSize / (1024 * 1024)}MB.";

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
                return $"Formato de ficheiro não permitido. Formatos aceites: {string.Join(", ", AllowedExtensions)}";

            return null;
        }

        public async Task<string> UploadImageAsync(IFormFile file, string subfolder)
        {
            // Validate
            var validationError = GetValidationError(file);
            if (validationError != null)
            {
                _logger.LogWarning("Image validation failed: {Error}", validationError);
                throw new InvalidOperationException(validationError);
            }

            // Create directory if it doesn't exist
            var uploadPath = Path.Combine(_environment.WebRootPath, "images", subfolder);
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
                _logger.LogInformation("Created directory: {Path}", uploadPath);
            }

            // Generate unique filename
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileNameWithoutExtension(file.FileName)}{extension}";
            var filePath = Path.Combine(uploadPath, uniqueFileName);

            // Save file
            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                _logger.LogInformation("Image uploaded successfully: {FileName}", uniqueFileName);

                // Return relative path
                return $"/images/{subfolder}/{uniqueFileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image: {FileName}", file.FileName);
                throw new InvalidOperationException("Erro ao fazer upload da imagem.", ex);
            }
        }

        public async Task<List<string>> UploadMultipleImagesAsync(List<IFormFile> files, string subfolder)
        {
            var uploadedPaths = new List<string>();

            if (files == null || files.Count == 0)
                return uploadedPaths;

            foreach (var file in files)
            {
                try
                {
                    var path = await UploadImageAsync(file, subfolder);
                    uploadedPaths.Add(path);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error uploading image in batch: {FileName}", file?.FileName);
                    // Continue with other files even if one fails
                }
            }

            _logger.LogInformation("Uploaded {Count} out of {Total} images", uploadedPaths.Count, files.Count);
            return uploadedPaths;
        }

        public async Task<bool> DeleteImageAsync(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return false;

            try
            {
                // Remove leading slash and convert to physical path
                var relativePath = imageUrl.TrimStart('/');
                var filePath = Path.Combine(_environment.WebRootPath, relativePath.Replace("/", Path.DirectorySeparatorChar.ToString()));

                if (File.Exists(filePath))
                {
                    await Task.Run(() => File.Delete(filePath));
                    _logger.LogInformation("Image deleted successfully: {Path}", imageUrl);
                    return true;
                }
                else
                {
                    _logger.LogWarning("Image file not found: {Path}", filePath);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image: {Path}", imageUrl);
                return false;
            }
        }
    }
}
