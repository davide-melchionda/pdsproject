using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileShare {
    public class Resources {

        /// <summary>
        /// Default picture for the generic user which has not setted a specific profile picture
        /// </summary>
        public Uri DefaultUserPicture {
            get {
                return new Uri(Path.Combine(Directory.GetCurrentDirectory(), "imgs", "user.ico")); /* DEFAULT */
            }
        }
        
        /// <summary>
        /// Default picture for the generic user which has not setted a specific profile picture
        /// </summary>
        public Uri WinLogo {
            get {
                return new Uri(Path.Combine(Directory.GetCurrentDirectory(), "imgs", "winlogo.png")); /* DEFAULT */
            }
        }

        /// <summary>
        /// folder icon 
        /// </summary>
        public Uri FolderIcon {
            get {
                return new Uri(Path.Combine(Directory.GetCurrentDirectory(), "imgs", "folder.png")); /* DEFAULT */
            }
        }

        /// <summary>
        /// folder icon 
        /// </summary>
        public Uri PictureIcon {
            get {
                return new Uri(Path.Combine(Directory.GetCurrentDirectory(), "imgs", "camera.png")); /* DEFAULT */
            }
        }

        /// <summary>
        /// folder icon 
        /// </summary>
        public Uri NothingToDoIcon {
            get {
                return new Uri(Path.Combine(Directory.GetCurrentDirectory(), "imgs", "nothing-to-do.png")); /* DEFAULT */
            }
        }

        /// <summary>
        /// folder icon 
        /// </summary>
        public Uri FileSharingPicture {
            get {
                return new Uri(Path.Combine(Directory.GetCurrentDirectory(), "imgs", "file-sharing.png")); /* DEFAULT */
            }
        }

    }
}
