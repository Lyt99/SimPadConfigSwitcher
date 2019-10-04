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
using Microsoft.Win32;
using System.Collections.ObjectModel;

namespace SimPadConfigSwitcher
{

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        private Dictionary<string, ObservableCollection<SettingInfo>> settingDict = new Dictionary<string, ObservableCollection<SettingInfo>>();

        private ObservableCollection<SettingInfo> settingList = null;
        private SettingInfo currentSetting = null;
        private SimPad currentDevice = null;

        private KeyBindingInfo[] keyBindings = new KeyBindingInfo[5];

        private SystemEvent e;
        private SimPadController.SimPadController controller = new SimPadController.SimPadController();

        private SimPad[] devices;

        private OpenFileDialog openFileDialog;

        public MainWindow()
        {
            //settingList = new ObservableCollection<SettingInfo>()
            //{
            //    new SettingInfo{
            //        TargetFilePath = @"E:\osu!\osu!.exe",
            //        Name = "测试"
            //    },
            //    new SettingInfo{
            //        TargetFilePath = @"E:\osu!\osu!.exe",
            //        Name = "测试1"
            //    }
            //};

            InitializeComponent();

            // 初始化数据
            devices = controller.GetDevices().ToArray();


            // 初始化绑定
            controller.OnSimpadDeviceChanged += deviceChanged;

            this.ListBoxSetting.ItemsSource = settingList;


            this.ListBoxSetting.ItemsSource = settingList;
            this.TBoxNoSelection.DataContext = settingList;
            this.ButtonAdd.DataContext = settingList;
            this.ButtonDelete.DataContext = settingList;
            this.ComboBoxDevices.ItemsSource = devices.Select(i => i.DisplayName).Distinct();

            this.openFileDialog = new OpenFileDialog();
            this.openFileDialog.Title = "选择可执行文件";
            this.openFileDialog.Filter = "可执行文件|*.exe";
            this.openFileDialog.FileName = String.Empty;
            this.openFileDialog.FilterIndex = 1;
            this.openFileDialog.DefaultExt = "exe";
        }

        private void deviceChanged(object sender, SimPadDeviceChangedEventArgs e)
        {
            // 更新数据
            devices = controller.GetDevices().ToArray();
            ComboBoxDevices.Dispatcher.Invoke(() => ComboBoxDevices.ItemsSource = devices.Select(i => i.DisplayName).Distinct());
             //this.ComboBoxDevices.Dispatcher.Invoke(() => {
             //    this.ComboBoxDevices.SelectedIndex = 0;
             //}); // 选择最新加入的那个
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
            currentSetting = (SettingInfo)((ListBox)sender).SelectedItem;


            var b = currentSetting.Setting.keySetting;

            this.TBKeyBinding1.DataContext = keyBindings[0] = new KeyBindingInfo(b[0]);
            this.TBKeyBinding2.DataContext = keyBindings[1] = new KeyBindingInfo(b[1]);
            this.TBKeyBinding3.DataContext = keyBindings[2] = new KeyBindingInfo(b[2]);
            this.TBKeyBinding4.DataContext = keyBindings[3] = new KeyBindingInfo(b[3]);
            this.TBKeyBinding5.DataContext = keyBindings[4] = new KeyBindingInfo(b[4]);


            this.GridSetting.DataContext = currentSetting;
        }

        private void ComboBoxDevices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string current = (string)((ComboBox)sender).SelectedItem;
            if (current == null) return;

            ObservableCollection<SettingInfo> settingInfo;
            if(!settingDict.TryGetValue(current, out settingInfo))
            {
                settingInfo = new ObservableCollection<SettingInfo>();
                settingDict[current] = settingInfo;
            }

            settingList = settingInfo;

            this.ListBoxSetting.ItemsSource = settingList;

            this.TBoxNoSelection.DataContext = settingList;
            this.ButtonAdd.DataContext = settingList;
            this.ButtonDelete.DataContext = settingList;
            this.currentDevice = this.devices[((ComboBox)sender).SelectedIndex];
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            if(this.openFileDialog.ShowDialog() == true)
            {
                string filename = this.openFileDialog.FileName;
                string ext = System.IO.Path.GetExtension(filename);
                if(ext.ToLower() != ".exe")
                {
                    MessageBox.Show(this, "请选择一个可执行文件！");
                }

                DeviceSettingInfo dsi = new DeviceSettingInfo();
                dsi.ReadFromDevice(this.currentDevice);

                SettingInfo si = new SettingInfo()
                {
                    Name = System.IO.Path.GetFileNameWithoutExtension(filename),
                    TargetFilePath = this.openFileDialog.FileName,
                    Setting = dsi
                };

                settingList.Add(si);
            }
        }
    }
}
