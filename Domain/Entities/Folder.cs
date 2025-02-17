

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class Folder: BaseEntity
    {

        [Required, MaxLength(200)]
        public string Name { get; set; }

        public Guid? ParentFolderId { get; set; }

        [ForeignKey("ParentFolderId")]
        public Folder ParentFolder { get; set; }

        public ICollection<Folder> SubFolders { get; set; }
        public ICollection<File> Files { get; set; }

        [Required]
        public string OwnerId { get; set; }


    }
}
