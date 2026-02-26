using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using SweetShop.Models.Interfaces;

namespace SweetShop.Services;

public class AzureBlobStorageService : IImageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName = "sweetshop-images";

    public AzureBlobStorageService(IConfiguration configuration)
    {
        // يجب إضافة AzureBlobStorage في appsettings.json
        var connectionString = configuration.GetConnectionString("AzureBlobStorage");
        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException("AzureBlobStorage connection string is missing.");

        _blobServiceClient = new BlobServiceClient(connectionString);
    }

    public async Task<string> UploadImageAsync(IFormFile file, string folderName = "products")
    {
        if (file == null || file.Length == 0)
            return string.Empty;

        // الحصول على الحاوية Container وإنشاؤها إذا لم تكن موجودة وجعلها عامة للقراءة
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

        // توليد اسم فريد للملف لتجنب استبدال ملفات بنفس الاسم
        var fileName = $"{folderName}/{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var blobClient = containerClient.GetBlobClient(fileName);

        // رفع الملف
        using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });

        // إرجاع الرابط الكامل للصورة المرفوعة
        return blobClient.Uri.ToString();
    }

    public async Task<bool> DeleteImageAsync(string fileUrl)
    {
        if (string.IsNullOrEmpty(fileUrl)) return false;

        try
        {
            var uri = new Uri(fileUrl);
            // استخراج اسم الفولدر واسم الملف من الرابط السحابي
            // مثال: products/1234.jpg
            var blobName = uri.Segments[^2] + uri.Segments[^1];

            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            return await blobClient.DeleteIfExistsAsync();
        }
        catch
        {
            return false;
        }
    }
}
