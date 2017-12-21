using FileTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTransfer {

    public class SendingJob : Job {

        /// <summary>
        /// The path of the base directory of files to zip
        /// </summary>
        private List<string> filePaths;
        /**
         * filePath property
         */
        public List<string> FilePaths {
            get {
                return filePaths;
            }
        }

        public SendingJob(Task task, List<string> filePaths) : base(task) {
            this.filePaths = filePaths;
        }

    }

}
