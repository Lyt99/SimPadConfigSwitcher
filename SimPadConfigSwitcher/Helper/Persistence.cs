using SimPadConfigSwitcher.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Serialization;

namespace SimPadConfigSwitcher.Helper
{
    class PersistenceHelper
    {
        public const string Path = "config.json";

        private static JsonSerializerSettings serializerSettings = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };


        public static void Save()
        {
            string jStr = JsonConvert.SerializeObject(Globals.SettingDict);
            using (StreamWriter sw = new StreamWriter(PersistenceHelper.Path))
            {
                sw.Write(jStr);
            }
        }

        public static void Load()
        {
            if(!File.Exists(PersistenceHelper.Path))
            {
                StreamWriter sw = new StreamWriter(PersistenceHelper.Path);
                sw.Write("{}");
                sw.Close();
                Globals.SettingDict = new Dictionary<string, ObservableCollection<SettingInfo>>();
                return;
            }

            using(StreamReader sr = new StreamReader(PersistenceHelper.Path))
            {
                string jStr = sr.ReadToEnd();
                Globals.SettingDict = JsonConvert.DeserializeObject<Dictionary<string, ObservableCollection<SettingInfo>>>(jStr);
            }
        }
    }
}
