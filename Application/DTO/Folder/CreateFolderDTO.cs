﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Folder
{
    public class CreateFolderDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public bool IsLocal { get; set; }
        public bool IsPublic { get; set; }
        public bool IsProtected { get; set; }
        public string Path { get; set; }
        public string VirtualPath { get; set; }
        public Guid UserId { get; set; }
    }
}
