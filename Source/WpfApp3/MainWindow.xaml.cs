using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp3
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public const string UPDATE_TEMP = "WpfLib.update";
        public const string DLL_PATH = "WpfLib.dll";
        private Assembly _assembly;

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                LoadAssembly();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }

        private void LoadAssembly()
        {
            if (File.Exists(UPDATE_TEMP))
            {
                File.Delete(DLL_PATH);
                File.Copy(UPDATE_TEMP, DLL_PATH);
                File.Delete(UPDATE_TEMP);
            }
            _assembly = Assembly.LoadFrom(DLL_PATH);
            Type type = _assembly.GetType($"WpfLib.BasePanel");
            UserControl panel = Activator.CreateInstance(type, null) as UserControl;

            panel.Visibility = Visibility.Visible;
            BaseGrid.Children.Add(panel);
        }
    }
}
