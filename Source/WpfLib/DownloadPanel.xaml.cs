using FreLib;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfLib
{
    /// <summary>
    /// DownloadPanel.xaml 的交互逻辑
    /// </summary>
    public partial class DownloadPanel : UserControl
    {
        public const string DEFAULT_TEMP_NAME = "tempdata.dat";
        public DownloadPanel()
        {
            InitializeComponent();
            CompositionTarget.Rendering += Update;
        }

        private void Update(object sender, EventArgs e)
        {
            if (MainDownload.Instance.IsRunning)
            {
                ProcBar.Value = MainDownload.Instance.Proc;
            }
            else
            {
                ProcBar.Value = 0;
            }
        }

        private void DownloadBtn_Click(object sender, RoutedEventArgs e)
        {
            if (MainDownload.Instance.IsRunning)
            {
                return;
            }
            string linkPath = LinkPath.Text;
            Task task = new Task(() =>
            {
                Download(linkPath);
            });
            task.Start();
        }

        public async void Download(string fileName)
        {
            Coding coding = new Coding(DEFAULT_TEMP_NAME);
            await MainDownload.Instance.Download(DEFAULT_TEMP_NAME, $"{fileName}.dat");
            await coding.Decoding();
            File.Delete(DEFAULT_TEMP_NAME);
            Process.Start("explorer.exe", AppDomain.CurrentDomain.BaseDirectory);
        }
    }
}
