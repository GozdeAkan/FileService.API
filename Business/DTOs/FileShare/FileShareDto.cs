using Domain.Enums;
using System;

namespace Business.DTOs.File
{
    public class FileShareDto
    {
        public Guid? FileId { get; set; }
        public Guid? FolderId { get; set; }
        public string? SharedToUserId { get; set; }
        public string? SharedToEmail { get; set; }
        public AccessLevel AccessLevel { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }

  
}
