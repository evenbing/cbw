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
    public partial class SpinnerControl : UserControl
    {
        public SpinnerControl()
        {
            InitializeComponent();
        }

        public void Start()
        {
            this.LayoutRoot.Visibility = System.Windows.Visibility.Visible;
            this.SpinningAnimation.Begin();
        }

        public void Stop()
        {
            this.SpinningAnimation.Stop();
            this.LayoutRoot.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
