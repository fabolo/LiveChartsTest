using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp
{
    class PhasesData : DependencyObject
    {
        //private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region data classes
        public class EventData
        {
            public bool Occurred = false;
            public double xLocation { get; set; }
            public double yLocation { get; set; }
            public DateTime Timestamp { get; set; }
            public int Temperature { get; set; }
        }
        public class PhaseLimits
        {
            public int fromTemperature { get; set; }
            public int toTemperature { get; set; }

        }
        public class PhaseData
        {
            /// <summary>
            /// Chart Duration is scaled to chart
            /// </summary>
            public double ChartDuration { get; set; }
            public int Percentage { get; set; }
            public PhaseLimits Limits { get; set; }

        }
        #endregion

        #region dependencyProperties
        public long dryingDuration
        {
            get { return (long)GetValue(dryingDurationProperty); }
            set { SetValue(dryingDurationProperty, value); }
        }

        public static readonly DependencyProperty dryingDurationProperty = DependencyProperty.Register(
           "dryingDuration", typeof(long), typeof(PhasesData));

        public int dryingPercentage
        {
            get { return (int)GetValue(dryingPercentageProperty); }
            set { SetValue(dryingPercentageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for dryingPercentage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty dryingPercentageProperty =
            DependencyProperty.Register("dryingPercentage", typeof(int), typeof(PhasesData), new PropertyMetadata(0));

        public long maillardDuration
        {
            get { return (long)GetValue(maillardDurationProperty); }
            set { SetValue(maillardDurationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for maillardDuration.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty maillardDurationProperty =
            DependencyProperty.Register("maillardDuration", typeof(long), typeof(PhasesData));

        public int maillardPercentage
        {
            get { return (int)GetValue(maillardPercentageProperty); }
            set { SetValue(maillardPercentageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for maillardPercentage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty maillardPercentageProperty =
            DependencyProperty.Register("maillardPercentage", typeof(int), typeof(PhasesData), new PropertyMetadata(0));

        /// <summary>
        /// time duration in ticks
        /// </summary>
        public long developDuration
        {
            get { return (long)GetValue(developDurationProperty); }
            set { SetValue(developDurationProperty, value); }
        }

        public static readonly DependencyProperty developDurationProperty = DependencyProperty.Register(
            "developDuration", typeof(long), typeof(PhasesData));

        public int developPercentage
        {
            get { return (int)GetValue(developPercentageProperty); }
            set { SetValue(developPercentageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for developPercentage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty developPercentageProperty =
            DependencyProperty.Register("developPercentage", typeof(int), typeof(PhasesData), new PropertyMetadata(0));

        #endregion

        public EventData Charge;
        public EventData Minimum;
        public EventData DryingEnd;
        public EventData MaillardEnd;
        public EventData Drop;
        public PhaseData Drying;
        public PhaseData Maillard;
        public PhaseData Development;

        private ChartPoint currentPoint;

        public PhasesData()
        {
            Charge = new EventData();
            Minimum = new EventData();
            DryingEnd = new EventData();
            MaillardEnd = new EventData();
            Drop = new EventData();
            Drying = new PhaseData();
            Maillard = new PhaseData();
            Development = new PhaseData();

        }

        public void UpdateAll(IEnumerable<ChartPoint> points)
        {
            // if already had a pass but not all data are populated
            // it will switch to incremental update
            if (currentPoint != null && !Drop.Occurred)
            {
                Update(points);
                return;
            }

            Charge.Occurred = false;
            Minimum.Occurred = false;
            DryingEnd.Occurred = false;
            MaillardEnd.Occurred = false;
            Drop.Occurred = false;
            //Drying = new PhaseData();
            //Maillard = new PhaseData();
            //Development



            List<ChartPoint> runningList = new List<ChartPoint>();

            foreach (var item in points)
            {
                runningList.Add(item);
                Update(runningList);
            }
        }
        /// <summary>
        /// populate data event and phases 
        /// </summary>
        /// <param name="points"></param>
        private void Update(IEnumerable<ChartPoint> points)
        {
            //bool noMoreData = false;
            //if (currentPoint != null)
            //    if (currentPoint.ChartLocation.X == points.Last().ChartLocation.X)
            //        noMoreData = true;

            currentPoint = points.Last();
            var curtimestamp = (DateTime)PhasesData.GetPropValue(currentPoint.Instance, "timestamp");
            long totalDuration = curtimestamp.Ticks;

            // a max temperature happen before chargement
            ChartPoint point = points.TakeWhile(p => true).Aggregate((curMax, p) => p.Y > curMax.Y ? p : curMax);

            if (!Charge.Occurred && currentPoint.Y < point.Y)
            {
                Charge.Timestamp = (DateTime)PhasesData.GetPropValue(point.Instance, "timestamp");
                Charge.Temperature = Convert.ToInt32(point.Y);
                Charge.xLocation = point.ChartLocation.X;
                Charge.yLocation = point.ChartLocation.Y;
                Charge.Occurred = true;
            }
            // test minimum (inside drying phase)
            point = points.Aggregate((curMin, p) => p.Y < curMin.Y ? p : curMin);
            if (Charge.Occurred && !Minimum.Occurred && currentPoint.Y > point.Y)
            {
                Minimum.Timestamp = (DateTime)PhasesData.GetPropValue(point.Instance, "timestamp");
                Minimum.Temperature = Convert.ToInt32(point.Y);
                Minimum.xLocation = point.ChartLocation.X;
                Minimum.yLocation = point.ChartLocation.Y;
                Minimum.Occurred = true;
            }
            // test the end of drying
            point = (from p in points where Minimum.Occurred && p.ChartLocation.X > Minimum.xLocation && p.Y >= Drying.Limits.toTemperature select p).FirstOrDefault();
            if (point != null && !DryingEnd.Occurred)
            {
                DryingEnd.Timestamp = (DateTime)PhasesData.GetPropValue(point.Instance, "timestamp");
                DryingEnd.Temperature = Convert.ToInt32(point.Y);
                DryingEnd.xLocation = point.ChartLocation.X;
                DryingEnd.yLocation = point.ChartLocation.Y;
                dryingDuration = DryingEnd.Timestamp.Subtract(Charge.Timestamp).Ticks;
                DryingEnd.Occurred = true;
            }

            if (Charge.Occurred)
            {
                // charge is the start of process, if Occurred refer to it
                totalDuration = curtimestamp.Subtract(Charge.Timestamp).Ticks;

                // drying in progress
                if (!DryingEnd.Occurred)
                {
                    Drying.ChartDuration = currentPoint.ChartLocation.X - Charge.xLocation;
                    //DryingEnd.xLocation = currentPoint.ChartLocation.X;
                    DryingEnd.Timestamp = curtimestamp; //(DateTime)PhasesData.GetPropValue(currentPoint.Instance, "timestamp");
                    dryingDuration = DryingEnd.Timestamp.Subtract(Charge.Timestamp).Ticks;
                }
                else
                {
                    Drying.ChartDuration = DryingEnd.xLocation - Charge.xLocation;

                    dryingPercentage = (int)((dryingDuration * 100) / totalDuration);
                }
            }


            if (DryingEnd.Occurred)
            {
                // test the end of maillard
                point = (from p in points where p.ChartLocation.X > DryingEnd.xLocation && p.Y >= Maillard.Limits.toTemperature select p).FirstOrDefault();
                if (point != null && !MaillardEnd.Occurred)
                {
                    MaillardEnd.Timestamp = (DateTime)PhasesData.GetPropValue(point.Instance, "timestamp");
                    MaillardEnd.Temperature = Convert.ToInt32(point.Y);
                    MaillardEnd.xLocation = point.ChartLocation.X;
                    MaillardEnd.yLocation = point.ChartLocation.Y;
                    maillardDuration = MaillardEnd.Timestamp.Subtract(DryingEnd.Timestamp).Ticks;
                    MaillardEnd.Occurred = true;
                }
                // maillard in progress
                if (!MaillardEnd.Occurred)
                {
                    Maillard.ChartDuration = currentPoint.ChartLocation.X - DryingEnd.xLocation;
                    //MaillardEnd.xLocation = currentPoint.ChartLocation.X;
                    MaillardEnd.Timestamp = (DateTime)PhasesData.GetPropValue(currentPoint.Instance, "timestamp");
                    maillardDuration = MaillardEnd.Timestamp.Subtract(DryingEnd.Timestamp).Ticks;
                }
                else
                {
                    Maillard.ChartDuration = MaillardEnd.xLocation - DryingEnd.xLocation;

                    maillardPercentage = (int)((maillardDuration * 100) / totalDuration);
                }
            }
            if (MaillardEnd.Occurred)
            {
                // test the end of development
                point = null;
                var endpoints = (from p in points where p.ChartLocation.X > MaillardEnd.xLocation && p.ChartLocation.X < currentPoint.ChartLocation.X select p).ToList();
                if (endpoints.Count > 0)
                {
                    ChartPoint maxpoint = endpoints.Aggregate((curMax, p) => p.Y > curMax.Y ? p : curMax);
                    if (currentPoint.Y < maxpoint.Y - 2)
                    {
                        //log.Debug(string.Format("development maxpoint {0} #C", maxpoint.Y));
                        point = maxpoint;
                    }

                    //log.Debug(string.Format("point != null && !Drop.Occurred {0} && {1}", point != null, !Drop.Occurred));
                    //else
                    //{
                    //    //if (noMoreData)
                    //        point = currentPoint;
                    //}

                    if (point != null && !Drop.Occurred)
                    {
                        Drop.Timestamp = (DateTime)PhasesData.GetPropValue(point.Instance, "timestamp");
                        Drop.Temperature = Convert.ToInt32(point.Y);
                        Drop.xLocation = point.ChartLocation.X;
                        Drop.yLocation = point.ChartLocation.Y;
                        Development.ChartDuration = Drop.xLocation - MaillardEnd.xLocation;
                        Drop.Occurred = true;
                    }
                    // development in progress
                    if (!Drop.Occurred)
                    {
                        //log.Debug(string.Format("development in progress ChartDuration {0}", currentPoint.ChartLocation.X - MaillardEnd.xLocation));
                        Development.ChartDuration = currentPoint.ChartLocation.X - MaillardEnd.xLocation;
                        //MaillardEnd.xLocation = currentPoint.ChartLocation.X;
                        Drop.Timestamp = (DateTime)PhasesData.GetPropValue(currentPoint.Instance, "timestamp");
                        developDuration = Drop.Timestamp.Subtract(MaillardEnd.Timestamp).Ticks;
                    }
                    else
                    {
                        // final calculation
                        totalDuration = Drop.Timestamp.Subtract(Charge.Timestamp).Ticks;
                        dryingPercentage = (int)Math.Round(dryingDuration * 100.0D / totalDuration);
                        maillardPercentage = (int)Math.Round(maillardDuration * 100.0D / totalDuration);
                        developPercentage = (int)Math.Round(developDuration * 100.0D / totalDuration);
                    }
                }

            }


        }
        public static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }
    }
}
