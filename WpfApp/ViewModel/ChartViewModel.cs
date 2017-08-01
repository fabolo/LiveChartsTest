using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using System;
using System.Windows.Media;
using System.Collections.Generic;
using System.Linq;
using WpfApp.Model;

namespace WpfApp.ViewModel
{
    public class ChartViewModel : ViewModelBase
    {
        private SeriesCollection seriesCollection = new SeriesCollection();
        public SeriesCollection SeriesCollection { get { return seriesCollection; } set { seriesCollection = value; RaisePropertyChanged("SeriesCollection"); } }

        private CartesianMapper<MyPoint> mapper;
        public Func<double, string> MinuteFormatter { get; set; }
        public Func<double, string> DegreeFormatter { get; set; }

        #region properties
        private double _xAxisMax;
        public double xAxisMax
        {
            get { return _xAxisMax; }
            set
            {
                _xAxisMax = value;
                RaisePropertyChanged("xAxisMax");
            }
        }
        private double _xAxisMin;
        public double xAxisMin
        {
            get { return _xAxisMin; }
            set
            {
                _xAxisMin = value;
                RaisePropertyChanged("xAxisMin");
            }
        }
        private double _yAxisMax;
        public double yAxisMax
        {
            get { return _yAxisMax; }
            set
            {
                _yAxisMax = value;
                RaisePropertyChanged("yAxisMax");
            }
        }
        private double _yAxisMin;
        public double yAxisMin
        {
            get { return _yAxisMin; }
            set
            {
                _yAxisMin = value;
                RaisePropertyChanged("yAxisMin");
            }
        }
        #endregion




        private int minute = 0;

        public ChartViewModel()
        {

        }

        [PreferredConstructor]
        public ChartViewModel(IDataService pDataService)
        {
            //xAxisMax = 20;
            //xAxisMin = 1;
            //yAxisMax = 100;
            //yAxisMin = 0;

            Setup();

            pDataService.GetData(
                (List<ChartValues<MyPoint>> items, Exception error) =>
                {
                    if (error != null)
                    {
                        // Report error here
                        return;
                    }


                    var item = items[0];

                    SetAxisLimits(Convert.ToInt64(item.First().X), Convert.ToInt64(item.Last().Y));

                    createSeries();

                    if (seriesCollection.Count == 3)
                    {
                        seriesCollection[0].Values = items[0];
                        seriesCollection[1].Values = items[1];
                        seriesCollection[2].Values = items[2];
                    }

                });

        }

        private void Setup()
        {
            mapper = Mappers.Xy<MyPoint>().X(rec => rec.X / TimeSpan.FromMinutes(1).Ticks)
                .Y(rec => Convert.ToDouble(rec.Y));

            MinuteFormatter = value => new DateTime((long)(value * TimeSpan.FromMinutes(1).Ticks)).AddMinutes(minute * -1).ToString("mm");
            DegreeFormatter = val => val.ToString("# '°C'");

            //lets save the mapper globally.
            Charting.For<MyPoint>(mapper);
        }
        private void createSeries()
        {
            SeriesCollection = new SeriesCollection();
            //SeriesCollection.NoisyCollectionChanged += SeriesCollection_NoisyCollectionChanged;
            //IEnumerable<object> seiries
            SeriesCollection.AddRange(new List<object>() {
                        new RoastingSeries(InitPhases())
                        {
                            Title = "Temperatura",
                            //Values = items[0],
                            PointGeometrySize = 2,
                            Configuration = mapper
                        }
                        ,
                       new LineSeries()
                       {
                           Title = "Valvola",
                           //Values = items[1],
                           PointGeometrySize = 2,
                            Configuration = mapper
                        },
                        new LineSeries()
                        {
                            Title = "De Temper.",
                            //Values = items[2],
                            PointGeometrySize = 2,
                            ScalesYAt=1,
                            Fill=Brushes.Transparent,
                            Configuration = mapper
                        }
            }
                              );
        }
        private PhasesData InitPhases()
        {

            // TODO read limit from database table
            var phaseData = new PhasesData();
            phaseData.Drying.Limits = new PhasesData.PhaseLimits() { fromTemperature = 70, toTemperature = 150 };
            //inizioEssiccatura = phaseData.Drying.Limits.fromTemperature;
            //fineEssiccatura = phaseData.Drying.Limits.toTemperature;
            //phaseData.Maillard.Limits = new PhasesData.PhaseLimits() { fromTemperature = 150, toTemperature = 180 };
            //inizioImbrunimento = phaseData.Maillard.Limits.fromTemperature;
            //fineImbrunimento = phaseData.Maillard.Limits.toTemperature;
            //phaseData.Development.Limits = new PhasesData.PhaseLimits() { fromTemperature = 180, toTemperature = 205 };
            //inizioSviluppo = phaseData.Development.Limits.fromTemperature;
            //fineSviluppo = phaseData.Development.Limits.toTemperature;

            return phaseData;
        }
        private void SetAxisLimits(long start, long end)
        {
            minute = new DateTime(start).Minute;
            yAxisMin = 10.0F;
            yAxisMax = 250.0F;
            xAxisMin = (double)(start - TimeSpan.FromSeconds(30).Ticks) / TimeSpan.FromMinutes(1).Ticks; //
            xAxisMax = (double)(start + TimeSpan.FromMinutes(18).Ticks + TimeSpan.FromSeconds(30).Ticks) / TimeSpan.FromMinutes(1).Ticks; // 
        }
    }
}
