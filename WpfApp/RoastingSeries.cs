using LiveCharts.Wpf;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp
{
    class RoastingSeries : LineSeries
    {

        //private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public PhaseBar DryingBarControl { get; set; }
        public PhaseBar MaillardBarControl { get; set; }
        public PhaseBar DevelopBarControl { get; set; }
        public EventIndicator ChargeIndicatorControl { get; set; }
        public EventIndicator MinimumIndicatorControl { get; set; }
        public EventIndicator DropIndicatorControl { get; set; }

        private PhasesData phasesData;

        public RoastingSeries(PhasesData pPhasesData)
        {
            phasesData = pPhasesData;


            //CallChartUpdater();
        }
        /// <summary>
        /// run the ChartUpdater
        /// </summary>
        public bool InitPhaseData()
        {
            Model.Chart.Updater.Run(true, false);
            return true;

            if (Model.Chart.AxisX != null && Model.Chart.AxisY != null)
                return true;// Model.Chart.Updater.Run();

            return false;
            //Thread bigStackThread = new Thread(() =>
            //{
            //    //while (Model.Chart == null)
            //    //{
            //    //    Thread.Sleep(100);
            //    //}
            //    if(Model.Chart != null)
            //Model.Chart.Updater.Run();
            //});

            //bigStackThread.Start();
            ////bigStackThread.Join();
            ////Model.Chart.Updater.Run();
        }

        public override void DrawSpecializedElements()
        {
            //if (!IsLoaded) return;

            if (ChargeIndicatorControl == null)
            {
                ChargeIndicatorControl = new EventIndicator();
            }
            if (ChargeIndicatorControl.Parent == null)
            {
                Model.Chart.View.AddToView(ChargeIndicatorControl);
            }
            if (DropIndicatorControl == null)
            {
                DropIndicatorControl = new EventIndicator();
                DropIndicatorControl.Visibility = Visibility.Hidden;
                DropIndicatorControl.Orientation = Orientation.Vertical;
                DropIndicatorControl.ArrowOrientation = IndicatorArrowOrientations.Right;
            }
            if (DropIndicatorControl.Parent == null)
            {
                Model.Chart.View.AddToView(DropIndicatorControl);
            }

            if (DryingBarControl == null)
            {
                DryingBarControl = new PhaseBar();
            }
            if (DryingBarControl.Parent == null)
            {
                DryingBarControl.Width = 0;
                //DryingBarControl.UpdateBarFill(new SolidColorBrush(Color.FromRgb(80, 160, 80)));
                //DryingBarControl.DataContext = phasesData;
                //DryingBarControl.SetBinding(PhaseBar.DurationProperty,
                //   new Binding { Path = new PropertyPath(PhasesData.dryingDurationProperty), Source = phasesData });
                //DryingBarControl.SetBinding(PhaseBar.PercentageProperty,
                //   new Binding { Path = new PropertyPath(PhasesData.dryingPercentageProperty), Source = phasesData });
                DryingBarControl.Visibility = Visibility.Hidden;
                Model.Chart.View.AddToView(DryingBarControl);

            }

            if (MinimumIndicatorControl == null)
            {
                MinimumIndicatorControl = new EventIndicator();
            }
            if (MinimumIndicatorControl.Parent == null)
            {
                Model.Chart.View.AddToView(MinimumIndicatorControl);
            }

            if (MaillardBarControl == null)
            {
                MaillardBarControl = new PhaseBar();
            }
            if (MaillardBarControl.Parent == null)
            {
                MaillardBarControl.Width = 0;
                //MaillardBarControl.UpdateBarFill(new SolidColorBrush(Color.FromRgb(230, 130, 60)));
                //MaillardBarControl.DataContext = phasesData;
                //MaillardBarControl.SetBinding(PhaseBar.DurationProperty,
                //   new Binding { Path = new PropertyPath(PhasesData.maillardDurationProperty), Source = phasesData });
                //MaillardBarControl.SetBinding(PhaseBar.PercentageProperty,
                //  new Binding { Path = new PropertyPath(PhasesData.maillardPercentageProperty), Source = phasesData });
                MaillardBarControl.Visibility = Visibility.Hidden;
                Model.Chart.View.AddToView(MaillardBarControl);

            }
            if (DevelopBarControl == null)
            {
                DevelopBarControl = new PhaseBar();
            }
            if (DevelopBarControl.Parent == null)
            {
                DevelopBarControl.Width = 0;
                //DevelopBarControl.UpdateBarFill(new SolidColorBrush(Color.FromRgb(110, 30, 30)));
                //DevelopBarControl.DataContext = phasesData;
                //DevelopBarControl.SetBinding(PhaseBar.DurationProperty,
                //   new Binding { Path = new PropertyPath(PhasesData.developDurationProperty), Source = phasesData });
                //DevelopBarControl.SetBinding(PhaseBar.PercentageProperty,
                //                   new Binding { Path = new PropertyPath(PhasesData.developPercentageProperty), Source = phasesData });
                DevelopBarControl.Visibility = Visibility.Hidden;
                Model.Chart.View.AddToView(DevelopBarControl);

            }

            //Model.Chart.ControlSize = new CoreSize(Model.Chart.ControlSize.Width,
            //Model.Chart.ControlSize.Height + 80);

            //log.Debug("DrawSpecializedElements");
            base.DrawSpecializedElements();
        }
        public override void PlaceSpecializedElements()
        {
            //if (!IsLoaded) return;

            phasesData.UpdateAll(Model.View.ActualValues.GetPoints(Model.View));

            if (phasesData.Charge.Occurred)
            {
                DryingBarControl.Visibility = Visibility.Visible;
                DryingBarControl.Width = phasesData.Drying.ChartDuration;
                Canvas.SetTop(DryingBarControl, Model.Chart.DrawMargin.Top);
                Canvas.SetLeft(DryingBarControl, Model.Chart.DrawMargin.Left + phasesData.Charge.xLocation);

                Canvas.SetTop(ChargeIndicatorControl, Model.Chart.DrawMargin.Top + phasesData.Charge.yLocation);
                Canvas.SetLeft(ChargeIndicatorControl, Model.Chart.DrawMargin.Left + phasesData.Charge.xLocation);
                //ChargeIndicatorControl.SetTemperature(phasesData.Charge.Temperature);
            }
            if (phasesData.Minimum.Occurred)
            {

                Canvas.SetTop(MinimumIndicatorControl, Model.Chart.DrawMargin.Top + phasesData.Minimum.yLocation);
                Canvas.SetLeft(MinimumIndicatorControl, Model.Chart.DrawMargin.Left + phasesData.Minimum.xLocation);
                //MinimumIndicatorControl.SetTemperature(phasesData.Minimum.Temperature);
            }

            if (phasesData.DryingEnd.Occurred)
            {
                MaillardBarControl.Visibility = Visibility.Visible;
                MaillardBarControl.Width = phasesData.Maillard.ChartDuration;
                Canvas.SetTop(MaillardBarControl, Model.Chart.DrawMargin.Top);
                Canvas.SetLeft(MaillardBarControl, Model.Chart.DrawMargin.Left + phasesData.DryingEnd.xLocation);
            }
            if (phasesData.MaillardEnd.Occurred)
            {
                DevelopBarControl.Visibility = Visibility.Visible;
                DevelopBarControl.Width = phasesData.Development.ChartDuration;
                Canvas.SetTop(DevelopBarControl, Model.Chart.DrawMargin.Top);
                Canvas.SetLeft(DevelopBarControl, Model.Chart.DrawMargin.Left + phasesData.MaillardEnd.xLocation);
            }
            if (phasesData.Drop.Occurred)
            {
                DropIndicatorControl.Visibility = Visibility.Visible;
                Canvas.SetTop(DropIndicatorControl, Model.Chart.DrawMargin.Top + phasesData.Drop.yLocation);
                Canvas.SetLeft(DropIndicatorControl, Model.Chart.DrawMargin.Left + phasesData.Drop.xLocation);
                //DropIndicatorControl.SetTemperature(phasesData.Drop.Temperature);
            }
            //}
            //Model.Chart.ControlSize = new CoreSize(Model.Chart.ControlSize.Width,
            //Model.Chart.ControlSize.Height+30);

            //(ActualValues.GetTracker(this).YLimit

            //log.Debug("PlaceSpecializedElements");

            base.PlaceSpecializedElements();
        }
        public override void OnSeriesUpdatedFinish()
        {
            //log.Debug("OnSeriesUpdatedFinish");
            base.OnSeriesUpdatedFinish();
        }
    }
}
