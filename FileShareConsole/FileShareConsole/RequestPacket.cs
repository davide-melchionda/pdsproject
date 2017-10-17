using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkTransmission
{
    class RequestPacket : TransmissionPacket
    {
        /**
         * The constructor sets the appropriate property Type 
         */
        public RequestPacket(Task involvedTask)
        {
            Type = PacketType.request;
            this.task = involvedTask;
        }

        /**
         * The task containing all the information about transmission.
         */
        public Task task
        {
            get
            {
                return task;
            }
            set
            {
                task = value;
            }
        }
    }
}
