﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTOs.Folder
{
    public class FolderCreateDto
    {
        public string Name { get; set; }

        public Guid? ParentFolderId { get; set; }

    }
}
