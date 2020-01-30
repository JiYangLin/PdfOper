using System;
using System.Windows;
using System.Windows.Input;
namespace pdfOper
{
    /// <summary>
    /// setcol.xaml 的交互逻辑
    /// </summary>
    public partial class ChoiceOutputPathDlg
    {
        public ChoiceOutputPathDlg()
        {
            InitializeComponent();

            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.tbPath.Text = "D:\\";
        }

        private void SetConfirm_Click(object sender, RoutedEventArgs e)
        {
           
            this.DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void ChoiceDir_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fb = new System.Windows.Forms.FolderBrowserDialog();
            fb.Description = "选择输出目录";
            if (System.Windows.Forms.DialogResult.OK != fb.ShowDialog()) return;
            this.tbPath.Text = fb.SelectedPath;
        }
    }
}
