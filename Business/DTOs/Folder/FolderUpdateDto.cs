using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTOs.Folder
{
    public class FolderUpdateDto
    {
        public string Name { get; set; }
        public Guid? ParentFolderId { get; set; }
    }
}
