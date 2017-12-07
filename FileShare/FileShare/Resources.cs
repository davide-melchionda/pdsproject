using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileShare {
    class Resources {

        /// <summary>
        /// Default picture for the generic user which has not setted a specific profile picture
        /// </summary>
        public static Uri DefaultUserPicture {
            get {
                return new Uri(Path.Combine(Directory.GetCurrentDirectory(), "imgs", "user.ico")); /* DEFAULT */
            }
        }

        /// <summary>
        /// folder icon 
        /// </summary>
        public static Uri FolderIcon {
            get {
                return new Uri(Path.Combine(Directory.GetCurrentDirectory(), "imgs", "folder.png")); /* DEFAULT */
            }
        }

        /// <summary>
        /// folder icon 
        /// </summary>
        public static Uri PictureIcon {
            get {
                return new Uri(Path.Combine(Directory.GetCurrentDirectory(), "imgs", "camera.png")); /* DEFAULT */
            }
        }

    }
}
