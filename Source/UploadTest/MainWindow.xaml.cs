using FreLib;
using System;
using System.Windows;
using System.Windows.Forms;

namespace UploadTest
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public const string IP = "34.84.177.172";
        public const string USER = "newftpuser";
        public const string PASSWARD = "showme";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void UploadBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            DialogResult result = dialog.ShowDialog();

            switch (result)
            {
                case System.Windows.Forms.DialogResult.OK:
                case System.Windows.Forms.DialogResult.Yes:
                    string path = dialog.FileName;
                    if (!string.IsNullOrEmpty(path))
                    {
                        UploadFile(path);
                    }
                    break;
            }
        }

        private void UploadFile(string path)
        {
            try
            {
                MainDownload d = new MainDownload(IP, USER, PASSWARD);
                d.Upload(UploadName.Text, path);
            }
            catch (Exception err)
            {
                System.Windows.MessageBox.Show(err.ToString());
            }
        }

        private void DownloadBtn_Click(object sender, RoutedEventArgs e)
        {
            string defaultName = DownloadName.Text;
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();

            switch (result)
            {
                case System.Windows.Forms.DialogResult.OK:
                case System.Windows.Forms.DialogResult.Yes:
                    string path = dialog.SelectedPath;
                    if (!string.IsNullOrEmpty(path))
                    {
                        DownloadPath($"{path}/{defaultName}", defaultName);
                    }
                    break;
            }
        }

        private async void DownloadPath(string srcPath, string targetPath)
        {
            try
            {
                MainDownload d = new MainDownload(IP, USER, PASSWARD);
                await d.Download(srcPath, targetPath);
            }
            catch (Exception err)
            {
                System.Windows.MessageBox.Show(err.ToString());
            }
        }

        private void EncodingBtn_Click(object sender, RoutedEventArgs e)
        {
            string edName = EncodingNameTextBox.Text;
            OpenFileDialog dialog = new OpenFileDialog();
            DialogResult result = dialog.ShowDialog();

            switch (result)
            {
                case System.Windows.Forms.DialogResult.OK:
                case System.Windows.Forms.DialogResult.Yes:
                    string path = dialog.FileName;
                    if (!string.IsNullOrEmpty(path))
                    {
                        Encoding(path, edName);
                    }
                    break;
            }
        }

        private void Encoding(string path, string name)
        {
            Coding coding = new Coding(path);
            try
            {
                coding.Encoding(name);
            }
            catch (Exception err)
            {
                System.Windows.MessageBox.Show(err.ToString());
            }
        }

        private async void Decoding(string path)
        {
            Coding coding = new Coding(path);
            try
            {
                await coding.Decoding();
            }
            catch (Exception err)
            {
                System.Windows.MessageBox.Show(err.ToString());
            }
        }

        private void DecodingBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            DialogResult result = dialog.ShowDialog();

            switch (result)
            {
                case System.Windows.Forms.DialogResult.OK:
                case System.Windows.Forms.DialogResult.Yes:
                    string path = dialog.FileName;
                    if (!string.IsNullOrEmpty(path))
                    {
                        Decoding(path);
                    }
                    break;
            }
        }

        private void UploadName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }
}
