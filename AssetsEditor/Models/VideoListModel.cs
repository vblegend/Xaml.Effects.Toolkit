using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Assets.Editor.Utils;
using Xaml.Effects.Toolkit.Model;

namespace Assets.Editor.Models
{
    public class VideoListModel : DialogModel
    {
        public readonly VideoInfo EmptyVideoInfo  = new VideoInfo();
        public ICommand DownloadCommand { get; protected set; }

        public ICommand PreviewCommand { get; protected set; }

        public ICommand DeleteCommand { get; protected set; }

        public ICommand CleanListCommand { get; protected set; }
        public ICommand DownloadAllCommand { get; protected set; }


        public ICommand ExplorerCommand { get; protected set; }
        public ICommand LoadListCommand { get; protected set; }
        public ICommand SaveListCommand { get; protected set; }


        public ICommand BackOffCommand { get; protected set; }
        public ICommand ForwardCommand { get; protected set; }


        public ICommand CommitTimeCommand { get; protected set; }



        public ObservableCollection<VideoInfo> VideoList { get; protected set; }


        public VideoListModel(ObservableCollection<VideoInfo> VideoList)
        {
            this.playingVideo = EmptyVideoInfo;
            this.videoTime = new TimeSpan(0);
            this.videoLength = new TimeSpan(0);
            this.VideoList = VideoList;
            this.title = "视频列表";
            this.PreviewCommand = new RelayCommand<VideoInfo>(Preview_Click);
            this.DownloadCommand = new RelayCommand<VideoInfo>(Download_Click);
            this.DeleteCommand = new RelayCommand<VideoInfo>(Delete_Click);
            this.ExplorerCommand = new RelayCommand<VideoInfo>(Explorer_Click);
            this.CommitTimeCommand = new RelayCommand<Object>(CommitTime_Click);
            //
            this.CleanListCommand = new RelayCommand(CleanList_Click);
            this.DownloadAllCommand = new RelayCommand(DownloadAll_Click);
            this.SaveListCommand = new RelayCommand(SaveList_Click);
            this.LoadListCommand = new RelayCommand(LoadList_Click);
            this.BackOffCommand = new RelayCommand(BackOff_Click);
            this.ForwardCommand = new RelayCommand(Forward_Click);

        }


        private void BackOff_Click()
        {

        }

        private void Forward_Click()
        {
        }




        public void CommitTime_Click(Object arg)
        {
            Task.Run(async () =>
            {
            });
        }

        private void LoadList_Click()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "*.vlist|*.vlist";
            var result = openFileDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                var json = File.ReadAllText(openFileDialog.FileName);
                var list = JsonSerializer.Deserialize<List<VideoInfo>>(json);
                VideoList.Clear();
                foreach (var item in list)
                {
                    VideoList.Add(item);
                }
            }
        }



        private void SaveList_Click()
        {
            SaveFileDialog openFileDialog = new SaveFileDialog();
            openFileDialog.Filter = "*.vlist|*.vlist";
            var result = openFileDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                var options = new JsonSerializerOptions();
                options.WriteIndented = true;
                var json = JsonSerializer.Serialize(VideoList, typeof(ObservableCollection<VideoInfo>), options);
                File.WriteAllText(openFileDialog.FileName, json);

            }
        }






        private void CleanList_Click()
        {
            VideoList.Clear();
        }





        private void Explorer_Click(VideoInfo video)
        {
            var path = System.IO.Path.Combine("X:\\HitPaw Video Downloader", video.Title + ".mp4");
            if (File.Exists(path))
            {
                ExplorerHelper.ExploreFile(path);
            }
        }




        private void Delete_Click(VideoInfo video)
        {
            VideoList.Remove(video);
        }


        private void Preview_Click(VideoInfo video)
        {
            PlayVideo(video);
        }


        private async Task PlayVideo(VideoInfo video)
        {
            var path = System.IO.Path.Combine("X:\\HitPaw Video Downloader", video.Title + ".mp4");

            if (video != this.playingVideo)
            {
                this.playingVideo = video;
                this.OnPropertyChanged(nameof(this.PlayingVideo));
            }
            var url = File.Exists(path) ? path : video.VideoUrl;

        }



        private async Task RePlayVideo()
        {
            if (this.playingVideo != EmptyVideoInfo)
            {
                await this.PlayVideo(this.playingVideo);
            }
        }



        private void Download_Click(VideoInfo video)
        {
            var path = System.IO.Path.Combine("X:\\HitPaw Video Downloader", video.Title + ".mp4");

            if (!File.Exists(path))
            {
                N_m3u8DLHelper.Download(video.VideoUrl, "X:\\HitPaw Video Downloader", video.Title);
            }
        }

        private async void DownloadAll_Click()
        {
            if (downloadThread != null)
            {
                downloadThread.Abort();
            }
            downloadThread = new Thread(onDOwnloadAll);
            downloadThread.Start();


        }


        Thread downloadThread = null;


        private async void onDOwnloadAll()
        {
            while (true)
            {
                Thread.Sleep(300);
                var video = VideoList.FirstOrDefault(e =>
                {
                    var path = System.IO.Path.Combine("X:\\HitPaw Video Downloader", e.Title + ".mp4");
                    return !File.Exists(path);
                });
   
                if (video == null) return;
                this.playingVideo = video;
                this.OnPropertyChanged(nameof(this.PlayingVideo));
                await N_m3u8DLHelper.Download(video.VideoUrl, "X:\\HitPaw Video Downloader", video.Title);

            }
        }







        public float VideoPosition
        {
            get
            {
                return this.videoPosition;
            }
            set
            {
                base.SetProperty(ref this.videoPosition, value);
            }
        }

        private float videoPosition;



        public TimeSpan VideoLength
        {
            get
            {
                return this.videoLength;
            }
            set
            {
                base.SetProperty(ref this.videoLength, value);
            }
        }

        private TimeSpan videoLength;

        public TimeSpan VideoTime
        {
            get
            {
                return this.videoTime;
            }
            set
            {
                base.SetProperty(ref this.videoTime, value);
            }
        }

        private TimeSpan videoTime;


        public VideoInfo PlayingVideo
        {

            get
            {
                return this.playingVideo;
            }
            set
            {

            }
        }

        private VideoInfo playingVideo;
    }
}
