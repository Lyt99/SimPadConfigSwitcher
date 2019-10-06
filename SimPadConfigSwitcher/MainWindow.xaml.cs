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

        private TextBox[] keyBindingTextboxes;

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

        private ConfigSwitcher configSwitcher = new ConfigSwitcher();

        public MainWindow()
        {
            InitializeComponent();
            // 文本框
            keyBindingTextboxes = new TextBox[]
            {
                this.TBKeyBinding1,
                this.TBKeyBinding2,
                this.TBKeyBinding3,
                this.TBKeyBinding4,
                this.TBKeyBinding5
            };

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
            notifyIcon.DoubleClick += NotifyIcon_DoubleClick;

            notifyIcon.Text = "SimPad配置切换工具";
            notifyIcon.Icon = new Icon("Resources\\icon.ico");
            notifyIcon.Visible = true;

            var menu = new System.Windows.Forms.ContextMenuStrip();

            var mi = new System.Windows.Forms.ToolStripMenuItem();
            mi.Text = "开启/关闭功能";
            mi.Click += SwitchSwither;
            menu.Items.Add(mi);

            mi = new System.Windows.Forms.ToolStripMenuItem();
            mi.Text = "退出";
            mi.Click += ClickExit;
            menu.Items.Add(mi);

            notifyIcon.ContextMenuStrip = menu;

            Globals.ShowBallonTip = this.ShowBallonTip;

            this.configSwitcher.Start();
            this.ShowBallonTip("SimPad设置切换工具功能已启动");
        }

        private void ClickExit(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SwitchSwither(object sender, EventArgs e)
        {
            if(this.configSwitcher.IsEnabled)
            {
                this.configSwitcher.Stop();
                this.ShowBallonTip("功能已关闭");
            } else
            {
                this.configSwitcher.Start();
                this.ShowBallonTip("功能已启动");
            }
        }

        public void ShowBallonTip(string text, int delay = 200)
        {
            notifyIcon.BalloonTipText = text;
            notifyIcon.ShowBalloonTip(delay);
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = WindowState.Normal;
            this.Activate();
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

            if (currentSetting == null) return;

            var b = currentSetting.Setting.KeySetting;

            int i = 0;
            for(; i < this.keyBindingTextboxes.Length && i < b.Length; ++i)
            {
                var inf = new KeyBindingInfo(b[i]);
                this.keyBindingTextboxes[i].DataContext = inf;
                this.keyBindingTextboxes[i].Text = inf.ToString();
            }

            for(; i < this.keyBindingTextboxes.Length; ++i)
            {
                this.keyBindingTextboxes[i].IsEnabled = false;
            }


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
            this.notifyIcon.Dispose();
            this.SaveConfig();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if(this.WindowState == WindowState.Minimized)
            {
                this.Hide();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var a = new AboutWindow();
            a.Owner = this;
            a.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("未完成!");
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            int index = ListBoxSetting.SelectedIndex;

            if (index == -1) return;

            this.settingList.RemoveAt(index);
        }
    }
}
