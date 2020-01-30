
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Documents;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.Diagnostics;
using System.Collections.Generic;
using PdfSharp.Pdf.IO;
using System.Drawing;

namespace pdfOper
{

    public class PdfData
    {
        public PdfData() { }
        public PdfData(string pat) {
            name = pat.Substring(pat.LastIndexOf("\\") + 1);
            pathName = pat;
        }
        public int num { get; set; }
        public string name { get; set; }

        public string pathName;
    }



    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {


        List<PdfData> m_PdfData = new List<PdfData>();
        void InsertData(PdfData dat)
        {
            foreach (PdfData item in m_PdfData)
            {
                if (item.pathName == dat.pathName) return;
            }
            m_PdfData.Add(dat);
        }

        public MainWindow()
        {
            InitializeComponent();
            ShowStatus("欢迎使用PdfOper");
        }
        void ShowStatus(string log)
        {
            this.Dispatcher.Invoke(()=> {
                Status.Content = DateTime.Now.ToString("yyyyMMddHHmmss: ") + log;
            });
        }
        #region ThrCTL
        public ManualResetEvent m_runing = new ManualResetEvent(false);

        bool isRunning()
        {
            if (m_runing.WaitOne(0))
            {
                OptWaitWnd wnd = new OptWaitWnd("", "正在运行,请稍后操作");
                wnd.ShowDialog();
                return true;
            }
            return false;
        }
        delegate void thrRun();
        void Run(thrRun fun)
        {
            m_runing.Set();
            Thread thread = new Thread(() => {
                try
                {
                    fun();
                    m_runing.Reset();
                }
                catch (Exception ex)
                {
                    OptWaitWnd wnd = new OptWaitWnd("", ex.ToString());
                    wnd.ShowDialog();
                    m_runing.Reset();
                }
            });
            thread.IsBackground = true;
            thread.Start();
        }
        #endregion






        void FlushDg()
        {
            for(int i = 0; i < m_PdfData.Count;++i)
            {
                m_PdfData[i].num = i + 1;
            }
            SourceList.ItemsSource = null;
            SourceList.ItemsSource = m_PdfData;
        }
        private void AddFile_Click(object sender, RoutedEventArgs e)
        {
            if (isRunning()) return;
            var of = new Microsoft.Win32.OpenFileDialog() { Filter = "PDF文件|*.pdf||" };
            if (true != of.ShowDialog()) return;
            InsertData(new PdfData(of.FileName));
            FlushDg();
        }
        private void AddFileBatch_Click(object sender, RoutedEventArgs e)
        {
            if (isRunning()) return;

            System.Windows.Forms.FolderBrowserDialog fb = new System.Windows.Forms.FolderBrowserDialog();
            fb.Description = "选择PDF目录";
            if (System.Windows.Forms.DialogResult.OK != fb.ShowDialog()) return;
  
            DirectoryInfo TheFolder = new DirectoryInfo(fb.SelectedPath);
            foreach (FileInfo NextFile in TheFolder.GetFiles("*.pdf"))
            {
                InsertData(new PdfData(NextFile.FullName));
            }
            FlushDg();
        }
        private void ClearFile_Click(object sender, RoutedEventArgs e)
        {
            if (isRunning()) return;
            m_PdfData.Clear();
            FlushDg();
        }
        private void DelteSelItem_Click(object sender, RoutedEventArgs e)
        {
            if (isRunning()) return;
            List<PdfData> itemList = new List<PdfData>();
            foreach (PdfData item in SourceList.SelectedItems)
            {
                itemList.Add(item);
            }
            foreach (PdfData item in itemList)
            {
                m_PdfData.Remove(item);
            }
            FlushDg();
        }
        private void SourceList_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (isRunning()) return;
            if (SourceList.SelectedIndex < 0) return;

            pdfViewer.OpenFile(m_PdfData[SourceList.SelectedIndex].pathName);
            pdfViewer.Zoom(1.0);
            ShowStatus("查看文件 " + m_PdfData[SourceList.SelectedIndex].pathName);
        }




        private void MergeFile_Click(object sender, RoutedEventArgs e)
        {
            if (isRunning()) return;
            if (m_PdfData.Count == 0) return;

            ChoiceOutputPathDlg dlg = new ChoiceOutputPathDlg();
            if (true != dlg.ShowDialog()) return;
            string outputpathName = dlg.tbPath.Text + "\\" + DateTime.Now.ToString("yyyyMMddHHmmss-") + "MergeFile.pdf";

            Run(()=> {
                PdfDocument outputDocument = new PdfDocument();
                foreach (var file in m_PdfData)
                {
                    PdfDocument inputDocument = PdfReader.Open(file.pathName, PdfDocumentOpenMode.Import);
                    foreach (var page in inputDocument.Pages) outputDocument.AddPage(page);
                }
                outputDocument.Save(outputpathName);
                ShowStatus("生成文件 " + outputpathName);
                Process.Start(outputpathName);
            });
        }

    }
}
