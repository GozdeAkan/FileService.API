using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;


namespace Business.DTOs.File
{
    public class FileCreateDto
    {
        public string Name { get; set; }

        public IFormFile File { get; set; } 
        public Guid? FolderId { get; set; }
      
    }

}
