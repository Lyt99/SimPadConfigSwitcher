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

namespace SimPadConfigSwitcher
{

    public class SettingInfo
    {
        public ImageSource Icon { get; set; }
        public string Name { get; set; }

    }
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<SettingInfo> SettingList { get; set; }

        public SystemEvent e;

        public MainWindow()
        {
            Console.WriteLine("Hello World!");

            SettingList = new List<SettingInfo>()
            {
                new SettingInfo{
                    Icon = null,
                    Name = "测试"
                }
            };

            InitializeComponent();

            this.ListBoxSetting.ItemsSource = SettingList;
        }
    }
}
