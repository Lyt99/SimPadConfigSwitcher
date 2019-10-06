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
        public bool IsEnabled { get; private set; }

        private readonly SystemEvent e = new SystemEvent(SystemEvents.EVENT_SYSTEM_FOREGROUND);

        private string LastApplication;

        public ConfigSwitcher()
        {
            IsEnabled = false;
            e.SystemEventHandler += handler;
        }

        private void handler(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            
            string path = e.GetApplicationPath();
            if(LastApplication == path)
            {
                return;
            }

            LastApplication = path;
            bool switched = false;

            foreach(var i in Globals.SettingDict)
            {
                var config = i.Value.FirstOrDefault((v) => v.TargetFilePath == path);
                if (config == null) continue;
                foreach(var device in Globals.Devices.Where(v => v.DisplayName == i.Key))
                {
                    switched = true;
                    config.Setting.Apply(device);
                }
            }

            if(switched)
            {
                Globals.ShowBallonTip("已为该应用切换设置");
            }
        }

        public void Start()
        {
            e.Start();
            this.IsEnabled = true;
        }

        public void Stop()
        {
            e.Stop();
            this.IsEnabled = false;
        }

        ~ConfigSwitcher()
        {
            e.Stop();
        }
    }
}
