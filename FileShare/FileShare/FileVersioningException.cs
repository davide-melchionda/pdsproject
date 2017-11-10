using System;
using System.Runtime.Serialization;

namespace FileShareConsole {

    /**
     * Detects a situation in which an operation on a file was not possible due to a problem reguarding
     * a versioning system.
     */

    [Serializable]
    internal class FileVersioningException : Exception {
        public FileVersioningException() {
        }

        public FileVersioningException(string message) : base(message) {
        }

        public FileVersioningException(string message, Exception innerException) : base(message, innerException) {
        }

        protected FileVersioningException(SerializationInfo info, StreamingContext context) : base(info, context) {
        }
    }
}