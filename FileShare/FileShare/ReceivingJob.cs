using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTransfer {

    public class ReceivingJob : Job {

        /// <summary>
        /// The path of the destination directory where received files will be putted.
        /// </summary>
        public string DestinationPath { get; set; }

        public ReceivingJob(Task task, string filePath) : base(task) {
            DestinationPath = filePath;
        }

    }

}
