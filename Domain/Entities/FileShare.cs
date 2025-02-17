using Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class FileShare : BaseEntity
    {

        public Guid? FileId { get; set; }

        [ForeignKey("FileId")]
        public File File { get; set; }
        public Guid? FolderId { get; set; }

        [ForeignKey("FolderId")]
        public Folder Folder { get; set; }


        [Required]
        public string OwnerUserId { get; set; }

        public string? SharedToUserId { get; set; }
        public string? SharedToEmail { get; set; }

        [Required]
        public string UniqueLinkToken { get; set; }

        [Required]
        public AccessLevel AccessLevel { get; set; }
        public DateTime? ExpirationDate { get; set; }

    }

    
}
