using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTransfer
{
    public class FileInfo
    {
        public enum Type { directory, file };
        public Type type;
        public string id;
        public string name;
        public string sender;
        public string receiver;
        public string requestTimestamp;
        public long size;
    }

}
