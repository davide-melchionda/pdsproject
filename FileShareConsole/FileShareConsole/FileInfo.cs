namespace FileTransfer
{
    public class FileInfo
    {
        public enum TypeEnum { directory, file };
        private TypeEnum typeInfo;
        public TypeEnum TypeInfo
        {
            get
            {
                return typeInfo;
            }
            protected set
            {
                typeInfo = value;
            }
        }
        public string id { get; set; }
        public string name { get; set; }

        public string requestTimestamp { get; set; }
        public long size { get; set; }


        public FileInfo(TypeEnum te) {
            this.typeInfo = te;
        }

    }
}
