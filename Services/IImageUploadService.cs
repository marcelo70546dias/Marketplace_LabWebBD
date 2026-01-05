namespace Marketplace_LabWebBD.Services
{
    /// <summary>
    /// Service for handling image uploads for the marketplace
    /// </summary>
    public interface IImageUploadService
    {
        /// <summary>
        /// Upload a single image to the server
        /// </summary>
        /// <param name="file">The image file to upload</param>
        /// <param name="subfolder">Subfolder within images directory (e.g., "anuncios")</param>
        /// <returns>Relative path to the uploaded image (e.g., "/images/anuncios/filename.jpg")</returns>
        Task<string> UploadImageAsync(IFormFile file, string subfolder);

        /// <summary>
        /// Upload multiple images to the server
        /// </summary>
        /// <param name="files">List of image files to upload</param>
        /// <param name="subfolder">Subfolder within images directory</param>
        /// <returns>List of relative paths to uploaded images</returns>
        Task<List<string>> UploadMultipleImagesAsync(List<IFormFile> files, string subfolder);

        /// <summary>
        /// Delete an image from the server
        /// </summary>
        /// <param name="imageUrl">Relative path to the image (e.g., "/images/anuncios/filename.jpg")</param>
        /// <returns>True if deleted successfully, false otherwise</returns>
        Task<bool> DeleteImageAsync(string imageUrl);

        /// <summary>
        /// Validate an image file (size and extension)
        /// </summary>
        /// <param name="file">The file to validate</param>
        /// <returns>True if valid, false otherwise</returns>
        bool ValidateImage(IFormFile file);

        /// <summary>
        /// Get validation error message if file is invalid
        /// </summary>
        /// <param name="file">The file to validate</param>
        /// <returns>Error message or null if valid</returns>
        string? GetValidationError(IFormFile file);
    }
}
