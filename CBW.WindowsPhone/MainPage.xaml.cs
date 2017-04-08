using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using CBW.WindowsPhone.Resources;
using Windows.Phone.Speech.Recognition;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using System.IO.IsolatedStorage;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Media;
using Newtonsoft.Json;

namespace CBW.WindowsPhone
{
    public partial class MainPage : PhoneApplicationPage
    {
        public Random rand = new Random();

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            //LoadRecent();
            RecentBox.DataContext = App.Memos;
            MainPageGrid.DataContext = App.Flips;

            this.TR2.btnTeach.Click += TeachButtonClick;
            this.TR2.btnRetrieve.Click += RetrieveButtonClick;

            FlipTimerAsync();
            FlipTimerOnTimeout += FlipControls;
        }

        public void InitializeFlips(IEnumerable<MemoItem> memos)
        {
            int counter = 0;
            App.Flips.Clear();
            foreach (MemoItem memo in memos)
            {
                FrontPageControl fc = new FrontPageControl();
                fc.SetKeywords(String.Join(", ", memo.Keywords));
                fc.SetAlarmTime(memo.RemindTime);
                fc.SetMemoDetail(memo.Content);
                fc.Row = counter / 2;
                fc.Column = counter % 2;
                counter++;
                App.Flips.Add(fc);
            }
        }

        public async void FlipTimerAsync()
        {
            while (true)
            {
                await Task.Run(
                    () =>
                    {
                        Thread.Sleep(500);
                    });
                if (FlipTimerOnTimeout != null)
                {
                    FlipTimerOnTimeout(Thread.CurrentThread, new EventArgs());
                }
            }
        }

        public event EventHandler FlipTimerOnTimeout;

        public void FlipControls(object sender, EventArgs args)
        {
            if (rand.NextDouble() < 0.1 && App.Flips.Count > 0)
            {
                int index = (int)(rand.NextDouble() * 567) % App.Flips.Count;
                App.Flips.ElementAt(index).Flip();
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (!IsolatedStorageSettings.ApplicationSettings.Contains("LocationConsent"))
            {
                MessageBoxResult result =
                    MessageBox.Show("This app accesses your phone's location. Is that ok?",
                    "Location",
                    MessageBoxButton.OKCancel);

                if (result == MessageBoxResult.OK)
                {
                    IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = true;
                }
                else
                {
                    IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = false;
                }

                IsolatedStorageSettings.ApplicationSettings.Save();
            }
            LoadRecent();
        }

        private void LoadRecent()
        {
            SpinnerControl spinner = new SpinnerControl();
            spinner.Start();

            WebClient client = new WebClient();
            client.DownloadStringAsync(new Uri(String.Format(Api.RecentUri, Phone.ID) +"?r=" + new Random().Next(Int32.MaxValue) + "&limit=" + 20));
            client.DownloadStringCompleted += (obj, args) =>
            {
                if (args.Error == null)
                {
                    MemoItem[] memos = JsonConvert.DeserializeObject<MemoItem[]>(args.Result);
                    App.Memos.Clear();
                    foreach (MemoItem item in memos)
                    {
                        App.Memos.Add(item);
                    }
                    InitializeFlips(App.Memos);
                    UpdateTile(memos);
                }
                spinner.Stop();
            };
        }

        private void RetrieveButtonClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/ConversationPage.xaml?action=retrieve", UriKind.Relative));
        }

        private void TeachButtonClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/ConversationPage.xaml?action=teach", UriKind.Relative));
        }

        private void GoMemoDetailTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ListBox listbox = (ListBox)sender;
            MemoItem passitem = null;
            foreach (MemoItem item in listbox.SelectedItems)
            {
                passitem = item;
                break;
            }
            App.MemoTemp = passitem;
            NavigationService.Navigate(new Uri("/DetailMemoPage.xaml", UriKind.Relative));
        }

        private void UpdateTile(IEnumerable<MemoItem> memos)
        {
            if (memos.FirstOrDefault() != null)
            {
                int count = memos.Where(x => x.HasAlarm).Count();
                if (count > 0)
                {
                    var tileData = new IconicTileData()
                    {
                        Title = "Speech Memo",
                        Count = count,
                        WideContent1 = memos.First().Keywords == null ? "" : String.Join(", ", memos.First().Keywords),
                        WideContent2 = memos.First().Content,
                        WideContent3 = memos.First().HasAlarm ? "Alarm at " + memos.First().RemindTime.ToString("ddd HH:mm") : "",
                        SmallIconImage = new Uri("/Assets/icon.png", UriKind.Relative),
                        IconImage = new Uri("/Assets/icon_s.png", UriKind.Relative),
                        BackgroundColor = (Color)Application.Current.Resources["PhoneAccentColor"]
                    };
                    ShellTile.ActiveTiles.First().Update(tileData);
                }
            }
        }

        private void MainPageGrid_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ListBox listbox = (ListBox)sender;
            MemoItem passitem = null;
            bool find = false;
            foreach (FrontPageControl item in listbox.SelectedItems)
            {
                string st = item.MemoDetail.Text + item.AlarmTime.ToString();
                foreach (MemoItem memo in App.Memos)
                {
                    item.TileSmaller();
                    string st1 = memo.Content + memo.RemindTime.ToString();
                    if (st == st1)
                    {
                        passitem = memo;
                        find = true;
                        break;
                    }
                }
                if (find) break;
            }
            App.MemoTemp = passitem;
            if (App.MemoTemp != null)
                NavigationService.Navigate(new Uri("/DetailMemoPage.xaml", UriKind.Relative));
        }
    }
}