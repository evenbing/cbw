using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBW.WindowsPhone
{
    class MainPageCollection<T> : ObservableCollection<T>
    {
        protected override void OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //UpDateMainPage();
            base.OnCollectionChanged(e);
        }
    }
}
