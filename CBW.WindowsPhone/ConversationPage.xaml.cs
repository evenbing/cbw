using Microsoft.Phone.Controls;
using Microsoft.Phone.Scheduler;
using Newtonsoft.Json;
using System;
using System.IO.IsolatedStorage;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Navigation;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Phone.Speech.Recognition;
using Windows.Phone.Speech.Synthesis;

namespace CBW.WindowsPhone
{
    public partial class ConversationPage : PhoneApplicationPage
    {
        public MessageCollection Conversation = new MessageCollection();
        protected SpeechRecognizerUI SpeechUI { get; set; }
        protected Popup NewMemoPopUp { get; set; }
        protected NewMemoPopUp NewMemoPopUpControl { get; set; }
        protected SpeechSynthesizer Synthesizer { get; set; }
        protected bool OnPicking { get; set; }
        protected bool IsTeach { get; set; }
        protected double? CurrentLongitude { get; set; }
        protected double? CurrentLatitude { get; set; }
        private static Regex ProfanityRegex { get; set; }

        static ConversationPage()
        {
            ProfanityRegex = new Regex("<profanity>.+</profanity>");
        }

        public ConversationPage()
        {
            InitializeComponent();

            this.TR.btnRetrieve.Checked += this.btnRetrieve_Checked;
            this.TR.btnTeach.Checked += this.btnTeach_Checked;
            this.TR.btnRetrieve.Unchecked += this.btnTeach_Checked;
            this.TR.btnTeach.Unchecked += this.btnRetrieve_Checked;
            this.TS.MouseLeftButtonDown += this.tapToSpeakGrid_MouseLeftButtonDown;
            this.TS.MouseLeftButtonUp += this.tapToSpeakGrid_MouseLeftButtonUp;

            this.DataContext = Conversation;

            this.SpeechUI = new SpeechRecognizerUI();
            this.Synthesizer = new SpeechSynthesizer();

            NewMemoPopUp = new Popup();
            NewMemoPopUpControl = new NewMemoPopUp();
            NewMemoPopUpControl.Width = Application.Current.Host.Content.ActualWidth;
            NewMemoPopUpControl.btnOK.Click += (s, args) =>
            {
                MemoItem item = new MemoItem();
                item.Latitude = this.CurrentLatitude;
                item.Longitude = this.CurrentLongitude;
                item.Content = this.NewMemoPopUpControl.txtBox.Text;
                item.HasAlarm = this.NewMemoPopUpControl.alarmCheck.IsChecked.Value;
                DateTime? date = NewMemoPopUpControl.datePicker.Value;
                DateTime? time = NewMemoPopUpControl.timePicker.Value;
                item.RemindTime = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day,
                            time.Value.Hour, time.Value.Minute, time.Value.Second);
                string teachRequest = JsonConvert.SerializeObject(item);
                NewMemoPopUp.IsOpen = false;
                WebClient client = new WebClient();
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                client.Headers[HttpRequestHeader.Accept] = "application/json";
                client.UploadStringAsync(new Uri(String.Format(Api.TeachUri, Phone.ID)), teachRequest);
                client.UploadStringCompleted += async (obj, e) =>
                {
                    if (e.Error == null)
                    {
                        if (item.HasAlarm)
                        {
                            Reminder reminder = new Reminder(item.RemindTime.ToString());
                            reminder.Title = "Speech Memo Reminder";
                            reminder.Content = item.Content;
                            reminder.BeginTime = item.RemindTime;
                            reminder.ExpirationTime = item.RemindTime + new TimeSpan(0, 5, 0);
                            reminder.NavigationUri = new Uri("/MainPage.xaml", UriKind.Relative);
                            try
                            {
                                ScheduledActionService.Add(reminder);
                            }
                            catch
                            {
                            }
                        }
                        string x = JsonConvert.DeserializeObject<string>(e.Result);
                        IAsyncAction action = this.Synthesizer.SpeakTextAsync(x);
                        this.Conversation.Add(new Message(x, MessageSide.Cloud));
                        this.BeginScrollDown();
                        await action;
                    }
                    else
                    {
                        await HandleInternetException();
                    }
                };
            };

            NewMemoPopUpControl.btnCancel.Click += (s, args) =>
            {
                NewMemoPopUp.IsOpen = false;
            };

            NewMemoPopUp.Child = NewMemoPopUpControl;
            Startup();
        }

        protected async void Startup()
        {
            Conversation.Add(new Message("What can I do for you?", MessageSide.Cloud));
            try
            {
                IAsyncAction action = this.Synthesizer.SpeakTextAsync("What can I do for you?");
                this.BeginScrollDown();
                await action;
            }
            catch
            {
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (e.Uri == NewMemoPopUpControl.datePicker.PickerPageUri || e.Uri == NewMemoPopUpControl.timePicker.PickerPageUri)
            {
                NewMemoPopUp.IsOpen = false;
                OnPicking = true;
            }
            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (OnPicking)
            {
                NewMemoPopUp.IsOpen = true;
                OnPicking = false;
            }
            else
            {
                string action;
                if (NavigationContext.QueryString.TryGetValue("action", out action))
                {
                    if (action == "teach")
                    {
                        this.IsTeach = true;
                        this.TR.btnTeach.IsChecked = true;
                        this.TR.btnRetrieve.IsChecked = false;
                    }
                    else if (action == "retrieve")
                    {
                        this.IsTeach = false;
                        this.TR.btnRetrieve.IsChecked = true;
                        this.TR.btnTeach.IsChecked = false;
                    }
                }
            }
            base.OnNavigatedTo(e);
        }

        protected void SetupPopup(string message, DateTime dt, bool check)
        {
            NewMemoPopUpControl.datePicker.Value = dt;
            NewMemoPopUpControl.timePicker.Value = dt;
            NewMemoPopUpControl.alarmCheck.IsChecked = check;
            NewMemoPopUpControl.datePicker.IsEnabled = check;
            NewMemoPopUpControl.timePicker.IsEnabled = check;
            NewMemoPopUpControl.txtBox.Text = message;
            NewMemoPopUp.IsOpen = true;
        }

        protected void BeginScrollDown()
        {
            this.ScrollPanel.UpdateLayout();
            this.ScrollPanel.ScrollToVerticalOffset(Double.MaxValue);
        }

        protected async Task NewMemoSpeech()
        {
            string message;
            try
            {
                this.SpeechUI.Settings.ShowConfirmation = false;
                message = CensorProfanity((await SpeechUI.RecognizeWithUIAsync()).RecognitionResult.Text);
            }
            catch
            {
                Conversation.Add(new Message("Hello", MessageSide.User));
                return;
            }

            // Got call back.
            Conversation.Add(new Message(message, MessageSide.User));
            message = message.Trim('.', '?', '!').ToLowerInvariant();

            WebClient client = new WebClient();
            if (this.IsTeach)
            {
                Conversation.Add(new Message(String.Format("Adding memo {0}.", message), MessageSide.Cloud));
                BeginScrollDown();
                await this.Synthesizer.SpeakTextAsync(String.Format("Adding memo {0}.", message));

                // Call API
                client.Headers[HttpRequestHeader.Accept] = "application/json";
                client.DownloadStringAsync(
                    new Uri(String.Format(Api.TimeExtractionUri, HttpUtility.UrlEncode(message), DateTime.Now.ToString("s"))));

                // Get time extraction response
                client.DownloadStringCompleted += async (sender, e) =>
                {
                    if (e.Error == null)
                    {
                        DateTime[] time = await JsonConvert.DeserializeObjectAsync<DateTime[]>(e.Result);

                        if (time.Length > 0)
                        {
                            SetupPopup(message, time[0], true);
                        }
                        else
                        {
                            SetupPopup(message, DateTime.Now + new TimeSpan(0, 30, 0), false);
                        }
                    }
                    else
                    {
                        SetupPopup(message, DateTime.Now + new TimeSpan(0, 30, 0), false);
                    }

                };

                // Get geo-position
                Geoposition geoposition;
                if ((bool)IsolatedStorageSettings.ApplicationSettings["LocationConsent"] == true)
                {
                    Geolocator geolocator = new Geolocator();
                    geolocator.DesiredAccuracyInMeters = 50;

                    try
                    {
                        geoposition = await geolocator.GetGeopositionAsync(
                            maximumAge: TimeSpan.FromMinutes(5),
                            timeout: TimeSpan.FromSeconds(10)
                            );
                        this.CurrentLatitude = geoposition.Coordinate.Latitude;
                        this.CurrentLongitude = geoposition.Coordinate.Longitude;
                    }
                    catch
                    {
                        this.CurrentLatitude = null;
                        this.CurrentLongitude = null;
                    }
                }
            }
            else
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                client.Headers[HttpRequestHeader.Accept] = "application/json";
                client.UploadStringAsync(
                    new Uri(String.Format(Api.RetrieveUri, Phone.ID)),
                    JsonConvert.SerializeObject(new
                    {
                        content = message
                    }));
                client.UploadStringCompleted += async (sender, args) =>
                {
                    if (args.Error == null)
                    {
                        string[] results = JsonConvert.DeserializeObject<string[]>(args.Result);
                        if (results.Length > 0)
                        {
                            IAsyncAction resultAction = this.Synthesizer.SpeakTextAsync(results[0]);
                            this.Conversation.Add(new Message(results[0], MessageSide.Cloud));
                            this.BeginScrollDown();
                            await resultAction;
                        }
                        else
                        {
                            IAsyncAction resultAction = this.Synthesizer.SpeakTextAsync("Can't recall \"" + message + "\".");
                            this.Conversation.Add(new Message("Can't recall \"" + message + "\".", MessageSide.Cloud));
                            this.BeginScrollDown();
                            await resultAction;
                        }
                    }
                    else
                    {
                        await HandleInternetException();
                    }
                };
            }
        }

        private void ConversationView_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private async void tapToSpeakGrid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.TS.onTappingStoryBoard.Begin();
            this.TS.onTappingStoryBoard2.Begin();
            bool error = false;
            try
            {
                await NewMemoSpeech();
            }
            catch
            {
                error = true;
            }
            if (error)
            {
                await HandleSpeechException();
            }
        }

        private void tapToSpeakGrid_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.TS.onReleasingStoryBoard.Begin();
            this.TS.onReleasingStoryBoard2.Begin();
        }

        private void btnTeach_Checked(object sender, RoutedEventArgs e)
        {
            this.TR.btnRetrieve.IsChecked = false;
            this.TR.btnTeach.IsChecked = true;
            this.IsTeach = true;
        }

        private void btnRetrieve_Checked(object sender, RoutedEventArgs e)
        {
            this.TR.btnTeach.IsChecked = false;
            this.TR.btnRetrieve.IsChecked = true;
            this.IsTeach = false;
        }

        private async Task HandleSpeechException()
        {
            IAsyncAction exAction = this.Synthesizer.SpeakTextAsync("Hold on.");
            Conversation.Add(new Message("Hold on.", MessageSide.Cloud));
            this.BeginScrollDown();
            await exAction;
        }

        private async Task HandleInternetException()
        {
            IAsyncAction action = this.Synthesizer.SpeakTextAsync("Internet connection is required to save your memo");
            this.Conversation.Add(new Message("Internet connection is required to save your memo", MessageSide.Cloud));
            this.BeginScrollDown();
            await action;
        }

        private static string CensorProfanity(string rawContent)
        {
            return ProfanityRegex.Replace(rawContent, String.Empty);
        }
    }
}