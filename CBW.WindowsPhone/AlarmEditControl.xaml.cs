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
    public partial class AlarmEditControl : UserControl
    {

        public AlarmEditControl(DateTime dateTime)
        {
            InitializeComponent();
            this.EditTimePicker.DataContext = dateTime;
            this.EditDatePicker.DataContext = dateTime;
        }
    }
}
