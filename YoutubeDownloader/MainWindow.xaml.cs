using MahApps.Metro.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
using YoutubeExplode;

namespace YoutubeDownloader
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 
   
    class ItemStorage
    {
        public string Url { get; set; }
    public string Size { get; set; }
        public string Quality { get; set; }
        public ItemStorage()//перегрузка конструктора
    {
    }
    public ItemStorage(string URL, string Size, string Quality)
    {
        this.Url = URL; this.Size = Size; this.Quality = Quality;
    }
    }



    public partial class MainWindow : MetroWindow
    {
        int selectInd = 0;
        string type = "";
        public MainWindow()
        {
            InitializeComponent();
        }
        YoutubeClient youtube = new YoutubeClient();
        string url = "";


        List<ItemStorage> sizes = new List<ItemStorage>();
        private async Task GetInfo()
        {

            var video = await youtube.Videos.GetAsync(url);

            title.Content = video.Title; // "Collections - Blender 2.80 Fundamentals"
            Console.WriteLine(video.Thumbnails[0].Url);
            poster.Source =  new ImageSourceConverter().ConvertFromString(video.Thumbnails[0].Url) as ImageSource;
            duration.Content = video.Duration.Value;  // 00:07:20
            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(url);
            var streamInfo = streamManifest.GetMuxedStreams();

            foreach (var vid in streamInfo)
            {
                sizes.Add(new ItemStorage(vid.Url, vid.Size.ToString(), vid.VideoQuality.ToString()));
                qual1.Items.Add(vid.VideoQuality);

            }
            qual1.SelectedIndex = 0;
            Console.WriteLine(streamManifest);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (qual1 == null)
                return;
            
            qual1.Items.Clear();
            url = url_Box.Text;
            GetInfo();
        }

        private void qual1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            selectInd = qual1.SelectedIndex;
            if (selectInd > sizes.Count - 1 || selectInd <0 )
                return;
            size.Content = sizes[selectInd].Size;
            url = sizes[selectInd].Url;
        }

        System.Net.WebClient webClient = new System.Net.WebClient();
        Stopwatch sw = new Stopwatch();

        void webClient_DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            try
            {
                if (downLoadSpeed.Content != (Convert.ToDouble(e.BytesReceived) / 1024 / sw.Elapsed.TotalSeconds).ToString("0"))
                    downLoadSpeed.Content = (Convert.ToDouble(e.BytesReceived) / 1024 / sw.Elapsed.TotalSeconds).ToString("0,00") + " КБ/с";

                if (progressBar.Value != e.ProgressPercentage)
                    progressBar.Value = e.ProgressPercentage;

                /*if (labelX1.Text != e.ProgressPercentage.ToString() + "%")
                    labelX1.Text = e.ProgressPercentage.ToString() + "%"; */

                downLoadSize.Content = (Convert.ToDouble(e.BytesReceived) / 1024 / 1024).ToString("0,00") + " МБ" + "  /  " + (Convert.ToDouble(e.TotalBytesToReceive) / 1024 / 1024).ToString("0,00") + " МБ";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void webClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
          /* 
           * MessageBox.Show(this, "Загрузка завершена", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            progressBarX1.Value = 100;
            labelX1.Text = "Загрузка завершена"; 
          */
        }

        private void button_skacat_Click(object sender, EventArgs e)
        {
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            type = ".mp4";
            Uri uri = new Uri(url);
            string filename = "";
            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = title.Content.ToString(); // Default file name
            dlg.DefaultExt = type; // Default file extension
            dlg.Filter = "Vide file (."+type+")|*"+type; // Filter files by extension
            Nullable<bool> result = dlg.ShowDialog();
             if (result == true)
            {
                // Save document
                 filename = dlg.FileName;
            }
            webClient.DownloadFileAsync(uri, filename);
            webClient.DownloadProgressChanged += new System.Net.DownloadProgressChangedEventHandler(webClient_DownloadProgressChanged);
            webClient.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(webClient_DownloadFileCompleted);
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            sw.Start();
        }
    }
}
