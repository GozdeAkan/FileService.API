
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class File: BaseEntity
    {

        [Required, MaxLength(200)]
        public string Name { get; set; }

        [Required, MaxLength(50)]
        public string FileType { get; set; }

        [Required]
        public long Size { get; set; }

        [Required]
        public string OwnerId { get; set; }

        public Guid? FolderId { get; set; }

        [ForeignKey("FolderId")]
        public Folder Folder { get; set; }

        [Required]
        public string BlobStoragePath { get; set; }

        public int CurrentVersion { get; set; }


        public ICollection<FileVersion> Versions { get; set; }
    }
}
