using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Data.Models
{
    public class FileData
    {
        public string FileName { get; set; }
        public byte[] File { get; set; }

        public FileData(string fileName, byte[] file)
        {
            FileName = fileName;
            File = file;
        }
    }
}
