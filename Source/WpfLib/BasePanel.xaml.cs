using System.Windows;
using System.Windows.Controls;

namespace WpfLib
{
    /// <summary>
    /// BasePanel.xaml 的交互逻辑
    /// </summary>
    public partial class BasePanel : UserControl
    {
        public BasePanel()
        {
            InitializeComponent();

            UpdatePanel.UpdateChecked = (UpdateResult check, string n) =>
            {
                switch (check)
                {
                    case UpdateResult.NoUpdate:
                        NoUpdate();
                        break;
                    case UpdateResult.Error:
                    case UpdateResult.Done:
                        MessageBox.Show(n);
                        break;
                }
            };
        }

        private void NoUpdate()
        {
            UpdatePanel.Visibility = Visibility.Hidden;
            DLPanel.Visibility = Visibility.Visible;
        }
    }
}
