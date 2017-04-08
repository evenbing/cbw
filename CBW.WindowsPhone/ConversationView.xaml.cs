using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;

namespace CBW.WindowsPhone
{
    public partial class ConversationView : UserControl
    {
        public ConversationView()
        {
            InitializeComponent();
            Color accent = (Color)Application.Current.Resources["PhoneAccentColor"];
            accent.R = (byte)(accent.R * 0.75);
            accent.G = (byte)(accent.G * 0.75);
            accent.B = (byte)(accent.B * 0.75);

            ((SolidColorBrush)Resources["UserConversationColorBrush"]).Color = accent;
        }
    }
}
