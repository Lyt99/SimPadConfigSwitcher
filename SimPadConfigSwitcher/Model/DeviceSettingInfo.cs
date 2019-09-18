using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimPadController.Device;
using SimPadController.Model;
using SimPadController.Enum;


namespace SimPadConfigSwitcher.Model
{
    public class DeviceSettingInfo
    {
        public string DeviceName;
        public List<SettingInfo> Settings;
        public KeySetting[] keySettings;
        public LightSpeed LightSpeed;
        public LightsType LightsType;

        /// <summary>
        /// 应用到设备
        /// </summary>
        public void Apply(SimPad device)
        {

        }

        /// <summary>
        /// 先differ，然后应用到设备
        /// </summary>
        /// <param name="info"></param>
        public void ApplyDiffer(SimPad device, DeviceSettingInfo info)
        {

        }

    }
}
