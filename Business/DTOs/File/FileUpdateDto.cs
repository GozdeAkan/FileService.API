
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace Business.DTOs.File
{
    public class FileUpdateDto
    {
        public string Name { get; set; }
        public Guid? FolderId { get; set; }
        public IFormFile? File { get; set; }

    }
}
