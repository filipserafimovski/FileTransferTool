using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTransferTool.App.Models
{
    public class ChecksumResult
    {
        public required string SourceHash { get; set; }
        public required string DestinationHash { get; set; }
        public bool IsMatch { get; set; }
    }
}
