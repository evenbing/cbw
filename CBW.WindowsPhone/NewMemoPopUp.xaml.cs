using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace CBW.WindowsPhone
{
    public partial class NewMemoPopUp : UserControl
    {
        public NewMemoPopUp()
        {
            InitializeComponent();
        }

        private void alarmCheck_Checked(object sender, RoutedEventArgs e)
        {
            this.datePicker.IsEnabled = true;
            this.timePicker.IsEnabled = true;
        }

        private void alarmCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            this.datePicker.IsEnabled = false;
            this.timePicker.IsEnabled = false;
        }
    }
}
