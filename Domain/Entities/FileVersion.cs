using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class FileVersion : BaseEntity
    {

        [ForeignKey("File")]
        public Guid FileId { get; set; }

        public File File { get; set; }  

        public int VersionNumber { get; set; }

        public string FilePath { get; set; }

    }
}
