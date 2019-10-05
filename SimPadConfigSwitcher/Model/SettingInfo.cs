using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using SimPadConfigSwitcher.Utility;
using Newtonsoft.Json;

namespace SimPadConfigSwitcher.Model
{
    public class SettingInfo
    { 
        private ImageSource icon;

        [JsonIgnore]
        public ImageSource Icon
        {
            get
            {
                if (icon == null && System.IO.File.Exists(this.TargetFilePath))
                {
                    icon = System.Drawing.Icon.ExtractAssociatedIcon(this.TargetFilePath).ToImageSource();
                }

                return icon;
            }
        }

        public bool IsDefault { get; set; }
        public string Name { get; set; }
        public string TargetFilePath { get; set; }
        public DeviceSettingInfo Setting { get; set; }
    }
}
