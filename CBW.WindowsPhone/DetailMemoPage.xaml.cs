using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Scheduler;
using System.Windows.Controls.Primitives;
using Newtonsoft.Json;

namespace CBW.WindowsPhone
{
    public partial class DetailMemoPage : PhoneApplicationPage
    {
        protected bool OnPicking { get; set; }
        Popup EditAlarmPopUp;
        AlarmEditControl EditAlarmPopUpControl;

        public DetailMemoPage()
        {
            InitializeComponent();
            SetDateContext();
        }

        private void RefreshPage()
        {
            SetDateContext();
        }

        private void MapFocusCreated(object sender, RoutedEventArgs e)
        {
            Map TheMap = new Map();
            if (App.MemoTemp.Latitude != null && App.MemoTemp.Longitude != null)
            {
                TheMap.Center = new System.Device.Location.GeoCoordinate(App.MemoTemp.Latitude.Value, App.MemoTemp.Longitude.Value);
            }

            Microsoft.Phone.Maps.Toolkit.Pushpin pin = new Microsoft.Phone.Maps.Toolkit.Pushpin();
            pin.GeoCoordinate = TheMap.Center;
            var layer = new MapLayer();
            var overlay = new MapOverlay();
            overlay.GeoCoordinate = TheMap.Center;
            overlay.Content = pin;
            layer.Add(overlay);
            TheMap.Layers.Add(layer);
            TheMap.ZoomLevel = 16;
            TheMap.Height = 318;
            Grid.SetRow(TheMap, 4);
            ContentPanel.Children.Add(TheMap);
        }

        private void AlarmOnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                App.MemoTemp.HasAlarm = true;
                AlarmSwitch.DataContext = true;
                Reminder reminder = new Reminder(App.MemoTemp.RemindTime.ToString());
                reminder.Title = "Speech Memo Reminder";
                reminder.Content = App.MemoTemp.Content;
                reminder.BeginTime = App.MemoTemp.RemindTime;
                reminder.ExpirationTime = App.MemoTemp.RemindTime + new TimeSpan(0, 5, 0);
                reminder.NavigationUri = new Uri("/MainPage.xaml", UriKind.Relative);
                ScheduledActionService.Add(reminder);
            }
            catch
            {
            }
        }

        private void AlarmOffClick(object sender, RoutedEventArgs e)
        {
            App.MemoTemp.HasAlarm = false;
            AlarmSwitch.DataContext = false;
            try
            {
                ScheduledActionService.Remove(App.MemoTemp.RemindTime.ToString());
            }
            catch
            { }
        }

        private void SetDateContext()
        {
            CreatedTime.DataContext = App.MemoTemp.CreatedTime.ToString("yyyy/MM/dd HH:mm");
            MemoDetail.DataContext = App.MemoTemp.Content;
            AlarmTime.DataContext = App.MemoTemp.RemindTime.ToString("yyyy/MM/dd HH:mm");
            AlarmSwitch.DataContext = App.MemoTemp.HasAlarm;
        }

        private void ReFreshDetailPage()
        {
            SetDateContext();
        }

        private async void Edit(object sender, EventArgs e)
        {
            Popup editMemoPopUp = new Popup();
            MemoEditControl editMemoPopUpControl = new MemoEditControl(App.MemoTemp.Content);

            editMemoPopUpControl.Width = Application.Current.Host.Content.ActualWidth;

            editMemoPopUp.IsOpen = true;

            editMemoPopUpControl.MemoEditOk.Click += (s, args) =>
            {
                editMemoPopUp.IsOpen = false;
                UpdateMemo(editMemoPopUpControl.MemoEdittxtBox.Text);
            };
            editMemoPopUpControl.MemoEditCancel.Click += (s, args) =>
            {
                editMemoPopUp.IsOpen = false;
            };
            editMemoPopUp.Child = editMemoPopUpControl;
        }

        private async void SetAlarm(object sender, EventArgs e)
        {
            EditAlarmPopUp = new Popup();
            EditAlarmPopUpControl = new AlarmEditControl(App.MemoTemp.RemindTime);

            EditAlarmPopUpControl.Width = Application.Current.Host.Content.ActualWidth;

            EditAlarmPopUp.IsOpen = true;

            EditAlarmPopUpControl.EditAlarmBtnOK.Click += (s, args) =>
            {
                EditAlarmPopUp.IsOpen = false;
                UpdateAlarm(EditAlarmPopUpControl.EditDatePicker.Value, EditAlarmPopUpControl.EditTimePicker.Value);
            };
            EditAlarmPopUpControl.EditAlarmBtnCancel.Click += (s, args) =>
            {
                EditAlarmPopUp.IsOpen = false;
            };
            EditAlarmPopUp.Child = EditAlarmPopUpControl;
        }

        private void UpdateAlarm(DateTime? date, DateTime? time)
        {
            if ((date != null) && (time != null))
            {
                DateTime datetime = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day,
                    time.Value.Hour, time.Value.Minute, time.Value.Second);
                App.MemoTemp.RemindTime = datetime;
            }
            RefreshPage();
            WebClient client = new WebClient();
            client.Headers[HttpRequestHeader.ContentType] = "application/json";
            client.Headers[HttpRequestHeader.Accept] = "application/json";
            client.UploadStringAsync(new Uri(String.Format(Api.MemoUri, Phone.ID, App.MemoTemp.Id)), "PUT", JsonConvert.SerializeObject(App.MemoTemp));
            client.UploadStringCompleted += (obj, args) =>
            {
                // Yeah
            };
        }

        private void UpdateMemo(string newMemo)
        {
            App.MemoTemp.Content = newMemo;
            ReFreshDetailPage();

            WebClient client = new WebClient();
            client.Headers[HttpRequestHeader.ContentType] = "application/json";
            client.Headers[HttpRequestHeader.Accept] = "application/json";
            client.UploadStringAsync(new Uri(String.Format(Api.MemoUri, Phone.ID, App.MemoTemp.Id)), "PUT", JsonConvert.SerializeObject(App.MemoTemp));
            Uri st = new Uri(String.Format(Api.MemoUri, Phone.ID, App.MemoTemp.Id));
            string data = JsonConvert.SerializeObject(App.MemoTemp);
            client.UploadStringCompleted += (obj, args) =>
            {
                // Yeah
                int i = 0;
            };
        }

        private async void Remove(object sender, EventArgs e)
        {
            WebClient client = new WebClient();
            client.UploadStringAsync(new Uri(String.Format(Api.MemoUri, Phone.ID, App.MemoTemp.Id)), "DELETE", "");
            client.UploadStringCompleted += (obj, args) =>
            {
                int k = App.Memos.IndexOf(App.MemoTemp);
                App.MemoTemp = null;
                App.Memos.RemoveAt(k);
                NavigationService.GoBack();
            };
        }

        private async void PinToMain(object sender, EventArgs e)
        {
            App.PinToMain(App.MemoTemp);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (EditAlarmPopUpControl != null)
            {
                if (e.Uri == EditAlarmPopUpControl.EditTimePicker.PickerPageUri || e.Uri == EditAlarmPopUpControl.EditDatePicker.PickerPageUri)
                {
                    EditAlarmPopUp.IsOpen = false;
                    OnPicking = true;
                }
            }
            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (OnPicking)
            {
                EditAlarmPopUp.IsOpen = true;
                OnPicking = false;
            }
            //else
            //{
            //    string action;
            //    if (NavigationContext.QueryString.TryGetValue("action", out action))
            //    {
            //        if (action == "teach")
            //        {
            //            this.IsTeach = true;
            //            this.btnTeach.IsChecked = true;
            //            this.btnRetrieve.IsChecked = false;
            //        }
            //        else if (action == "retrieve")
            //        {
            //            this.IsTeach = false;
            //            this.btnRetrieve.IsChecked = true;
            //            this.btnTeach.IsChecked = false;
            //        }
            //    }
            //}
            base.OnNavigatedTo(e);
        }

    }
}