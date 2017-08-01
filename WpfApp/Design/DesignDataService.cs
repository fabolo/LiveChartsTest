using System;
using System.Collections.Generic;
using LiveCharts;
using WpfApp.Model;

namespace WpfApp.Design
{
    class DesignDataService : IDataService
    {
        public void GetData(Action<List<ChartValues<MyPoint>>, Exception> callback)
        {
            var items = new List<ChartValues<MyPoint>>();

            items.Add(new ChartValues<MyPoint>()
            {
                new MyPoint { Y=60,X=System.DateTime.Now.Ticks, timestamp=System.DateTime.Now},
                new MyPoint { Y=80,X=System.DateTime.Now.AddMinutes(2).Ticks, timestamp=System.DateTime.Now.AddMinutes(2)},
                new MyPoint { Y=120,X=System.DateTime.Now.AddMinutes(2).Ticks, timestamp=System.DateTime.Now.AddMinutes(4)}
            });
            items.Add(new ChartValues<MyPoint>()
            {
                new MyPoint { Y=10,X=1000, timestamp=System.DateTime.Now},
                new MyPoint { Y=40,X=5000, timestamp=System.DateTime.Now.AddMinutes(2)},
                new MyPoint { Y=60,X=9000, timestamp=System.DateTime.Now.AddMinutes(6)}
            });
            items.Add(new ChartValues<MyPoint>()
            {
                new MyPoint { Y=5,X=1000, timestamp=System.DateTime.Now},
                new MyPoint { Y=4,X=5000, timestamp=System.DateTime.Now.AddMinutes(2)},
                new MyPoint { Y=6,X=9000, timestamp=System.DateTime.Now.AddMinutes(6)}
            });

            callback(items, null);
        }
    }
}
