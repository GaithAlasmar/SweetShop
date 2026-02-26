using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace SweetShop.Models.Interfaces;

public interface IImageService
{
    /// <summary>
    /// Uploads an image to the cloud storage and returns its URL.
    /// </summary>
    /// <param name="file">The image file uploaded by the user.</param>
    /// <param name="folderName">The logical folder name (e.g. products, users).</param>
    /// <returns>The full URL of the uploaded image.</returns>
    Task<string> UploadImageAsync(IFormFile file, string folderName = "products");

    /// <summary>
    /// Deletes an image from the cloud storage.
    /// </summary>
    /// <param name="fileUrl">The full URL of the image to delete.</param>
    /// <returns>True if deleted successfully, false otherwise.</returns>
    Task<bool> DeleteImageAsync(string fileUrl);
}
