using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimPadConfigSwitcher.Utility;


namespace SimPadConfigSwitcher.Helper
{
    class ConfigSwitcher
    {
        
        private readonly SystemEvent e = new SystemEvent(SystemEvents.EVENT_SYSTEM_FOREGROUND);

        public ConfigSwitcher()
        {
            e.SystemEventHandler += handler;
        }

        private void handler(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            string path = e.GetApplicationPath();
            foreach(var i in Globals.SettingDict)
            {
                var config = i.Value.FirstOrDefault((v) => v.TargetFilePath == path);
                if (config == null) continue;
                foreach(var device in Globals.Devices.Where(v => v.DisplayName == i.Key))
                {
                    config.Setting.Apply(device);
                }
            }
        }

        public void Start()
        {
            e.Start();
        }

        public void Stop()
        {
            e.Stop();
        }

        ~ConfigSwitcher()
        {
            e.Stop();
        }
    }
}
