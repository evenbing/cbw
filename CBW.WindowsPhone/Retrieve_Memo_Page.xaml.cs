using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Collections.ObjectModel;
using Windows.Phone.Speech.Recognition;

namespace CBW.WindowsPhone
{
    public partial class Retrieve_Memo_Page : PhoneApplicationPage
    {
        public ObservableCollection<MemoItem> Memos = new ObservableCollection<MemoItem>();
        SpeechRecognizerUI SpeechUI;

        public Retrieve_Memo_Page()
        {
            InitializeComponent();
            RetrieveMemoSpeech();
            MemoBox.DataContext = Memos;
            // Play animation
        }

        private async void RetrieveMemoSpeech()
        {
            this.SpeechUI = new SpeechRecognizerUI();
            SpeechRecognitionUIResult recoResult = await SpeechUI.RecognizeWithUIAsync();
            String phoneID = Phone.ID;
            String message = recoResult.RecognitionResult.Text;
            MessageBox.Show(string.Format("You said {0}.", recoResult.RecognitionResult.Text));

            // Call API
            // Got Call back

            Memos.Add(new MemoItem());
        }
    }
}