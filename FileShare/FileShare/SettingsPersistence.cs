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
    class SettingsPersistence
    {
        private static string settingsFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\settingssdss.txt";
      
        public static void readSettings()
        {
            //Settings.Instance.PropertyChanged += Instance_PropertyChanged;
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new IPAddressConverter());

            if (File.Exists(settingsFile))
            {
                using (StreamReader sR = new StreamReader(settingsFile))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(sR.ReadToEnd());
                    Settings appSettings = (Settings)JsonConvert.DeserializeObject(sb.ToString(), typeof(Settings), settings);
                    Settings.Instance.AlwaysUseDefault = appSettings.AlwaysUseDefault;
                    Settings.Instance.AutoAcceptFiles = appSettings.AutoAcceptFiles;
                    Settings.Instance.CurrentUsername = appSettings.CurrentUsername;
                    Settings.Instance.DefaultRecvPath = appSettings.DefaultRecvPath;
                    Settings.Instance.IsInvisible = appSettings.IsInvisible;
              ;
                }
            }
        }

       

        public static void writeSettings()
        {

            using (StreamWriter sw = new StreamWriter(settingsFile))
            {
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.Converters.Add(new IPAddressConverter());
                sw.WriteLine(JsonConvert.SerializeObject(Settings.Instance, settings));
                //TODO non funge poiché ipaddress necessita serializer a se
              
             
            }
        }

        //private static void Instance_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    writeSettings();
        //}
    }
}
