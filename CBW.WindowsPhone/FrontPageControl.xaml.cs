using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CBW.WindowsPhone
{
    //[ContentPropertyAttribute("Content", true)]
    public partial class FrontPageControl : UserControl
    {
        private Storyboard _frontToBack;
        private Storyboard _backToFront;
        private Storyboard _frontToBackUD;
        private Storyboard _backToFrontUD;
        private Storyboard _smaller;
        private Random rand = new Random();
        private bool _front;
        public DateTime AlarmTime;
        private int _row = 215;
        private int _col = 215;

        public int Row
        {
            get
            {
                return _row;
            }
            set
            {
                this._row = value;
                Grid.SetRow(this, this._row);
            }
        }
        public int Column
        {
            get
            {
                return this._col;
            }
            set
            {
                this._col = value;
                Grid.SetColumn(this, this._col);
            }
        }

        public FrontPageControl()
        {
            InitializeComponent();
            _frontToBack = this.Resources["TileFlipF_B"] as Storyboard;
            var k = this.Resources;
            _frontToBack.AutoReverse = false;
            _frontToBack.FillBehavior = FillBehavior.HoldEnd;

            _frontToBackUD = this.Resources["TileFlipF_BUD"] as Storyboard;
            _frontToBackUD.AutoReverse = false;
            _frontToBackUD.FillBehavior = FillBehavior.HoldEnd;

            _backToFront = this.Resources["TileFlipB_F"] as Storyboard;
            _backToFront.AutoReverse = false;
            _backToFront.FillBehavior = FillBehavior.HoldEnd;

            _backToFrontUD = this.Resources["TileFlipB_FUD"] as Storyboard;
            _backToFrontUD.AutoReverse = false;
            _backToFrontUD.FillBehavior = FillBehavior.HoldEnd;

            _smaller = this.Resources["Tile_Smaller"] as Storyboard;
            _smaller.AutoReverse = false;
            _smaller.FillBehavior = FillBehavior.Stop;

            _front = false;
        }

        public void SetKeywords(string content)
        {
            MemoKewords.DataContext = content;
        }

        public void SetAlarmTime(DateTime content)
        {
            AlarmTime = content;
            Hour.DataContext = content.ToString("HH:mm");
            DayLetter.DataContext = content.ToString("ddd");
            DayDate.DataContext = content.Day.ToString().PadLeft(2, '0');
        }

        public void SetMemoDetail(string content)
        {
            //throw new NotImplementedException();
            MemoDetail.DataContext = content;
        }

        public void TileSmaller()
        {
            _smaller.Begin();
        }

        public void Flip()
        {
            if (_front)
            {
                _front = false;
                if (rand.NextDouble() > 0.5)
                    _frontToBack.Begin();
                else
                    _frontToBackUD.Begin();
            }
            else
            {
                _front = true;
                if (rand.NextDouble() > 0.5)
                    _backToFront.Begin();
                else
                    _backToFrontUD.Begin();
            }
        }
    }
}
