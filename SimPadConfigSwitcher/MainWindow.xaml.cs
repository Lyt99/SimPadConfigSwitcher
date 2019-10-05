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
using System.Threading;
using SimPadConfigSwitcher.Helper;

namespace SimPadConfigSwitcher
{

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        private ObservableCollection<SettingInfo> settingList = null;
        private SettingInfo currentSetting = null;
        private SimPad currentDevice = null;

        private KeyBindingInfo[] keyBindings = new KeyBindingInfo[5];

        private SimPadController.SimPadController controller = new SimPadController.SimPadController();

        private OpenFileDialog openFileDialog = new OpenFileDialog()
        {
            Title =  "选择可执行文件",
            Filter = "可执行文件|*.exe",
            FileName = String.Empty,
            FilterIndex = 1,
            DefaultExt = "exe"
        };

        private System.Windows.Forms.NotifyIcon notifyIcon = new System.Windows.Forms.NotifyIcon();

        public MainWindow()
        {
            InitializeComponent();

            // 加载配置
            this.LoadConfig();

            // 初始化数据
            Globals.Devices = controller.GetDevices().ToArray();


            // 初始化绑定
            controller.OnSimpadDeviceChanged += deviceChanged;

            this.ListBoxSetting.ItemsSource = settingList;


            this.ListBoxSetting.ItemsSource = settingList;
            this.TBoxNoSelection.DataContext = settingList;
            this.ButtonAdd.DataContext = settingList;
            this.ButtonDelete.DataContext = settingList;
            this.ComboBoxDevices.ItemsSource = Globals.Devices.Select(i => i.DisplayName).Distinct();

            //设置托盘的各个属性
            notifyIcon.BalloonTipText = "程序开始运行";
            notifyIcon.Text = "托盘图标";
            //notifyIcon.Icon = new System.Drawing.Icon(System.Windows.Forms.Application.StartupPath + \"\\\\wp.ico\");
            notifyIcon.Visible = true;
            notifyIcon.ShowBalloonTip(2000);
        }

        private void deviceChanged(object sender, SimPadDeviceChangedEventArgs e)
        {
            // 更新数据
            Globals.Devices = controller.GetDevices().ToArray();
            ComboBoxDevices.Dispatcher.Invoke(() => ComboBoxDevices.ItemsSource = Globals.Devices.Select(i => i.DisplayName).Distinct());
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


            var b = currentSetting.Setting.KeySetting;

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
            if(!Globals.SettingDict.TryGetValue(current, out settingInfo))
            {
                settingInfo = new ObservableCollection<SettingInfo>();
                Globals.SettingDict[current] = settingInfo;
            }

            settingList = settingInfo;

            this.ListBoxSetting.ItemsSource = settingList;

            this.TBoxNoSelection.DataContext = settingList;
            this.ButtonAdd.DataContext = settingList;
            this.ButtonDelete.DataContext = settingList;
            this.currentDevice = Globals.Devices[((ComboBox)sender).SelectedIndex];
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

            this.SaveConfig();
            
        }

        private void SaveConfig()
        {
            PersistenceHelper.Save();
        }


        private void LoadConfig()
        {
            PersistenceHelper.Load();
        }

        private void Window_LostFocus(object sender, RoutedEventArgs e)
        {
            this.SaveConfig();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.SaveConfig();
        }
    }
}
