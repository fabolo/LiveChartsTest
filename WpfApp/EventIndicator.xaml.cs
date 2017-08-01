using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp
{
    public enum IndicatorArrowOrientations
    {
        Left = 0,
        Right = 1
    }
    /// <summary>
    /// Interaction logic for EventIndicator.xaml
    /// </summary>
    public partial class EventIndicator : UserControl
    {
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set
            {
                SetValue(OrientationProperty, value);
                this.panel.Orientation = value;
            }
        }

        // Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(EventIndicator), new PropertyMetadata(Orientation.Horizontal));

        public IndicatorArrowOrientations ArrowOrientation
        {
            get { return (IndicatorArrowOrientations)GetValue(ArrowOrientationProperty); }
            set
            {
                SetValue(ArrowOrientationProperty, value);
                if (value == IndicatorArrowOrientations.Right)
                {
                    this.arrow.LayoutTransform = new ScaleTransform() { ScaleX = -1 };
                    this.arrow.RenderTransform = new TranslateTransform() { X = this.arrow.Width * -2 };
                    this.border.RenderTransform = new TranslateTransform() { X = this.arrow.Width * -2 };
                }
            }
        }

        // Using a DependencyProperty as the backing store for ArrowOrientation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ArrowOrientationProperty =
            DependencyProperty.Register("ArrowOrientation", typeof(IndicatorArrowOrientations), typeof(EventIndicator), new PropertyMetadata(IndicatorArrowOrientations.Left));


        public EventIndicator()
        {
            InitializeComponent();
        }
    }
}
