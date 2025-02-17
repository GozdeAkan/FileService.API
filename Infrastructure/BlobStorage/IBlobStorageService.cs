

using Microsoft.AspNetCore.Http;

namespace Infrastructure.BlobStorage
{
    public interface IBlobStorageService
    {
        Task<string> UploadFileAsync(IFormFile file);
    }
}
