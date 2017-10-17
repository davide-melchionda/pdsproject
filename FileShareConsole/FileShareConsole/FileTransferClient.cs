using System;
using FileTransfer;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkTransmission
{
    class FileTransferClient : PacketClient
    {

        public FileIterator iterator;       // Iterator provides the abstraction towards the file location 
        public FileTransfer.Task task;      // Task that provides all the informations about transfer

        public FileTransferClient(FileIterator fIterator, FileTransfer.Task involvedTask)
        {
            this.iterator = fIterator;
            this.task = involvedTask;
        }
        protected override void execute()
        {
            //     TODO funzionamento del client 
        }
    }
}
