using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
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
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        YoutubeClient youtube = new YoutubeClient();
        string url = "";
        
        
      
        private async Task GetInfo()
        {
           // qual;
            var video = await youtube.Videos.GetAsync(url);

            title.Content = video.Title; // "Collections - Blender 2.80 Fundamentals"
                                         //  var author = video.Author.Title;  "Blender" size nyjen
            duration.Content = video.Duration.Value;  // 00:07:20
            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(url);
            var streamInfo = streamManifest.GetMuxedStreams();
            foreach(var vid in streamInfo)
            {
                qual.Items.Add(vid.Size);
            }
            
            Console.WriteLine(streamManifest);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            url = url_Box.Text;
            GetInfo();
        }
    }
}
