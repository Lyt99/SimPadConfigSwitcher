using SimPadConfigSwitcher.Model;
using SimPadController.Device;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimPadConfigSwitcher
{
    class Globals
    {
        public delegate void ShowBallonTipDelegate(string text, int delay = 2000);

        public static ShowBallonTipDelegate ShowBallonTip;
        public static Dictionary<string, ObservableCollection<SettingInfo>> SettingDict;
        public static SimPad[] Devices;
    }
}
