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
        public KeySetting[] keySetting { get; set; }

        public LightSpeed LightSpeed { get; set; }
        public ushort EaseLightDelay { get => LightSpeed.EaseLightDelay; set => LightSpeed.EaseLightDelay = value; }
        public ushort RainbowLightDelay { get => LightSpeed.RainbowLightDelay; set => LightSpeed.RainbowLightDelay = value; }

        public LightsType LightsType { get; set; }
        public int DelayInput { get; set; }
        public Color ColorG1 { get; set; }
        public Color ColorG2 { get; set; }

        /// <summary>
        /// 应用到设备
        /// </summary>
        public void Apply(SimPad device)
        {

        }

        /// <summary>
        /// 先diff，然后应用到设备
        /// </summary>
        /// <param name="info"></param>
        public void ApplyDiff(SimPad device, DeviceSettingInfo info)
        {

        }

        /// <summary>
        /// 从设备读取设置
        /// </summary>
        /// <param name="device"></param>
        public void ReadFromDevice(SimPad device)
        {
            this.keySetting = new KeySetting[device.KeyCount];
            for(uint i = 0;i < device.KeyCount; ++i)
            {
                keySetting[i] = device.GetKeySetting(i + 1);
            }

            this.LightSpeed = device.LightSpeed;
            this.LightsType = device.LightsType;
            this.DelayInput = device.DelayInput;

            this.ColorG1 = device.GetLEDColor(1);
            this.ColorG2 = device.GetLEDColor(2);
        }

    }
}
