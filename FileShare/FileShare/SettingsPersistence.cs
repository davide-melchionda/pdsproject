using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace FileShare
{
    /**
     * Exposes two metods to save settings on close and load settings on start if the file is available
     */
    class SettingsPersistence
    {
        private static string settingsFile = Settings.Instance.AppDataPath+"\\Settings.txt";

        public static void readSettings()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new IPAddressConverter());

            if (File.Exists(settingsFile))
            {
                using (StreamReader sR = new StreamReader(settingsFile))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(sR.ReadToEnd());
                    StorableSettings appSettings = (StorableSettings)JsonConvert.DeserializeObject(sb.ToString(), typeof(StorableSettings), settings);
                    Settings.Instance.AlwaysUseDefault = appSettings.AlwaysUseDefault;
                    Settings.Instance.AutoAcceptFiles = appSettings.AutoAccept;
                    Settings.Instance.CurrentUsername = appSettings.CurrentUsername;
                    Settings.Instance.DefaultRecvPath = appSettings.DefaultPath;
                    Settings.Instance.IsInvisible = appSettings.IsInvisible;
                    Settings.Instance.PicturePath = appSettings.PicPath;

                }
            }
        }
        
        public static void writeSettings()
        {
            StorableSettings storable= new StorableSettings();
            storable.IsInvisible = Settings.Instance.IsInvisible;
            storable.AutoAccept = Settings.Instance.AutoAcceptFiles;
            storable.CurrentUsername = Settings.Instance.CurrentUsername;
            storable.DefaultPath = Settings.Instance.DefaultRecvPath;
            storable.PicPath = Settings.Instance.PicturePath;
            storable.AlwaysUseDefault = Settings.Instance.AlwaysUseDefault;
            System.IO.DirectoryInfo di = new DirectoryInfo(Path.GetDirectoryName(settingsFile));
            if (!di.Exists)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(settingsFile));
            }
                using (StreamWriter sw = File.CreateText(settingsFile))

            {
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.Converters.Add(new IPAddressConverter());
                sw.WriteLine(JsonConvert.SerializeObject(storable, settings));


            }
        }

        /**
        * Internal class used to store only the setting's properties that we want to make persistent
        */

        class StorableSettings
        {
            private bool isInvisible;
            private bool alwaysUseDefault;
            private bool autoAccept;
            private string defaultPath;
            private string currentUsername;
            private string picPath;
            public bool IsInvisible { get => isInvisible; set => isInvisible = value; }
            public bool AlwaysUseDefault { get => alwaysUseDefault; set => alwaysUseDefault = value; }
            public bool AutoAccept { get => autoAccept; set => autoAccept = value; }
            public string DefaultPath { get => defaultPath; set => defaultPath = value; }
            public string CurrentUsername { get => currentUsername; set => currentUsername = value; }
            public string PicPath { get => picPath; set => picPath = value; }

            public StorableSettings() { }

        }
    }


 
}
