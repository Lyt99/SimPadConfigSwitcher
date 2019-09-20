using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SimPadController;
using SimPadConfigSwitcher.Utility;
using SimPadConfigSwitcher.Model;
using SimPadController.Device;
using SimPadController.Enum;

namespace SimPadConfigSwitcher
{

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        public Dictionary<string, List<SettingInfo>> settingDict = new Dictionary<string, List<SettingInfo>>();

        public List<SettingInfo> SettingList => null;
        public SettingInfo CurrentSetting = null;

        public KeyBindingInfo[] KeyBindings = new KeyBindingInfo[5];

        public SystemEvent e;
        public SimPadController.SimPadController controller = new SimPadController.SimPadController();

        public SimPad[] Devices;

        public MainWindow()
        {
            //SettingList = new List<SettingInfo>()
            //{
            //    new SettingInfo{
            //        Icon = null,
            //        Name = "测试"
            //    }
            //};

            InitializeComponent();

            // 初始化数据
            Devices = controller.GetDevices().ToArray();


            // 初始化绑定
            controller.OnSimpadDeviceChanged += deviceChanged;

            this.ListBoxSetting.ItemsSource = SettingList;


            this.TBKeyBinding1.DataContext = KeyBindings[0] = new KeyBindingInfo();
            this.TBKeyBinding2.DataContext = KeyBindings[1] = new KeyBindingInfo();
            this.TBKeyBinding3.DataContext = KeyBindings[2] = new KeyBindingInfo();
            this.TBKeyBinding4.DataContext = KeyBindings[3] = new KeyBindingInfo();
            this.TBKeyBinding5.DataContext = KeyBindings[4] = new KeyBindingInfo();

            this.ComboBoxDevices.ItemsSource = Devices.Select(i => i.DisplayName).Distinct();
        }

        private void deviceChanged(object sender, SimPadDeviceChangedEventArgs e)
        {
            // 更新数据
            Devices = controller.GetDevices().ToArray();
            ComboBoxDevices.Dispatcher.Invoke(() => ComboBoxDevices.ItemsSource = Devices.Select(i => i.DisplayName).Distinct());
        }

        private void TBKeyBinding_KeyUp(object sender, KeyEventArgs e)
        {
            var tb = (TextBox)sender;
            var info = (KeyBindingInfo)tb.DataContext;

            info.Apply();
            tb.Text = info.ToString();
        }

        private void TBKeyBinding_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var tb = (TextBox)sender;
            var info = (KeyBindingInfo)tb.DataContext;

            info.Modifiers = Keyboard.Modifiers;

            Func<Key, bool> isNormal = (Key i) => i != Key.LeftCtrl && i != Key.RightCtrl && i != Key.LeftAlt && i != Key.RightAlt && i != Key.LeftShift && i != Key.RightShift && i != Key.System;

            if (isNormal(e.Key))
            {
                info.NormalKey = e.Key;
            }


            tb.Text = info.ToString();
            e.Handled = true;
        }

        private void ListBoxSetting_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentSetting = (SettingInfo)((ComboBox)sender).SelectedItem;
        }

        private void ComboBoxDevices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string current = (string)((ComboBox)sender).SelectedItem;
            if (current == null) return;

            List<SettingInfo> settingInfo;
            if(!settingDict.TryGetValue(current, out settingInfo))
            {
                settingInfo = new List<SettingInfo>();
                settingDict[current] = settingInfo;
            }



        }
    }
}
