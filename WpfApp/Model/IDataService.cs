using System;
using System.Collections.Generic;
using LiveCharts;
using WpfApp.Model;

namespace WpfApp.Model
{
    public interface IDataService
    {
        void GetData(Action<List<ChartValues<MyPoint>>, Exception> callback);
    }
}
