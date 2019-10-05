using SimPadController.Device;
using SimPadController.Enum;
using SimPadController.Model;

namespace SimPadConfigSwitcher.Model
{
    public class DeviceSettingInfo
    {
        public KeySetting[] KeySetting { get; set; }

        public LightSpeed LightSpeed { get; set; }
        //public ushort EaseLightDelay { get => LightSpeed.EaseLightDelay; set => LightSpeed.EaseLightDelay = value; }
        //public ushort RainbowLightDelay { get => LightSpeed.RainbowLightDelay; set => LightSpeed.RainbowLightDelay = value; }

        public LightsType LightsType { get; set; }
        public int DelayInput { get; set; }
        public Color ColorG1 { get; set; }
        public Color ColorG2 { get; set; }

        /// <summary>
        /// 应用到设备
        /// </summary>
        public void Apply(SimPad device)
        {
            for(uint i = 0; i < device.KeyCount; ++i)
            {
                device.SetKeySetting(i + 1, KeySetting[i]);
            }

            device.LightSpeed = this.LightSpeed;
            device.LightsType = this.LightsType;
            device.DelayInput = this.DelayInput;

            device.SetLEDColor(1, this.ColorG1);
            device.SetLEDColor(2, this.ColorG2);

            device.ApplyAllSettings(); // 驱动那边做了diff了，所以这边不用管了
        }

        /// <summary>
        /// 从设备读取设置
        /// </summary>
        /// <param name="device"></param>
        public void ReadFromDevice(SimPad device)
        {
            this.KeySetting = new KeySetting[device.KeyCount];
            for(uint i = 0; i < device.KeyCount; ++i)
            {
                KeySetting[i] = device.GetKeySetting(i + 1);
            }

            this.LightSpeed = device.LightSpeed;
            this.LightsType = device.LightsType;
            this.DelayInput = device.DelayInput;

            this.ColorG1 = device.GetLEDColor(1);
            this.ColorG2 = device.GetLEDColor(2);
        }

    }
}
