using FreLib;
using System;
using System.IO;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfLib
{
    public enum UpdateResult
    {
        Error,
        NoUpdate,
        Done,
    }
    /// <summary>
    /// UpdateCheckPanel.xaml 的交互逻辑
    /// </summary>
    public partial class UpdatePanel : UserControl
    {
        private int _pCount = 0;
        private DateTime _startTime;
        private Config _config;

        private bool _isChecking = true;
        public Action<UpdateResult, string> UpdateChecked;

        public UpdatePanel()
        {
            InitializeComponent();
            using (StreamReader reader = new StreamReader("Config.txt"))
            {
                string nowConfig = reader.ReadToEnd();
                _config = new Config(nowConfig);
            }

            CompositionTarget.Rendering += CompositionTarget_Rendering;
            _startTime = DateTime.Now;
            CheckUpdate();
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if ((DateTime.Now - _startTime).TotalSeconds > 1)
            {
                string dp = "正在检查更新";
                StringBuilder builder = new StringBuilder('.');
                for (int i = 0; i < _pCount; ++i)
                {
                    builder.Append('.');
                }
                UpdateCheckingLabel.Content = $"{dp}{builder}";

                ++_pCount;
                _pCount %= 3;
                _startTime = DateTime.Now;
            }
        }

        public async void CheckUpdate()
        {
            try
            {
                bool isDone = await MainDownload.Instance.Download("tempConfig.txt", "Config.txt");
                if (!isDone)
                {
                    return;
                }
                Config config;
                using (StreamReader reader = new StreamReader("tempConfig.txt"))
                {
                    string newConfig = reader.ReadToEnd();
                    config = new Config(newConfig);
                }

                if (config > _config)
                {
                    using (StreamWriter writer = new StreamWriter("Config.txt"))
                    {
                        writer.WriteLine(config.ToString());
                    }

                    await MainDownload.Instance.Download("WpfLib.dll", "WpfLib.Update");
                    UpdateChecked?.Invoke(UpdateResult.Done, "更新完毕，请重新启动");
                }

                UpdateChecked?.Invoke(UpdateResult.NoUpdate, "");
                File.Delete("tempConfig.txt");
            }
            catch (Exception err)
            {
                UpdateChecked?.Invoke(UpdateResult.Error, err.ToString());
            }
        }
    }
}
