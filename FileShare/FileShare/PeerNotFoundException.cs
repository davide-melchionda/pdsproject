using System;
using System.Runtime.Serialization;

namespace HelloProtocol {
    [Serializable]
    internal class PeerNotFoundException : Exception {
        public PeerNotFoundException() {
        }

        public PeerNotFoundException(string message) : base(message) {
        }

        public PeerNotFoundException(string message, Exception innerException) : base(message, innerException) {
        }

        protected PeerNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) {
        }
    }
}