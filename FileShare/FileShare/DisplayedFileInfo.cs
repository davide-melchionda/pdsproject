using FileTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileShare
{
    public class DisplayedFileInfo {

        public FileInfo Info { get; set; }

        public bool IsDirectory {
            get {
                return Info.Type == FileInfo.FType.DIRECTORY;
            }
        }

        public DisplayedFileInfo(string filepath) {
            Info = new FileInfo(filepath);
        }

        public DisplayedFileInfo(FileInfo info) {
            Info = info;
        }

    }
}
