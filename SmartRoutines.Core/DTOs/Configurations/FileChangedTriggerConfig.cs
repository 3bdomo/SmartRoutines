using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartRoutines.Core.DTOs.Configurations
{
    public class FileChangedTriggerConfig
    {
        public string FolderPath { get; set; } = string.Empty;
        public string FileFilter { get; set; } = "*.*";
        public bool IncludeSubdirectories { get; set; }
    }

}
