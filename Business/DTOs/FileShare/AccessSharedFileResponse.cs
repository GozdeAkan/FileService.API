using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTOs.FileShare
{
    public class AccessSharedFileResponse
    {
        public List<string> Url { get; set; }
        public AccessLevel AccessLevel { get; set; } 
    }
}
