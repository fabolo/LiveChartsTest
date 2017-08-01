using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp.Model
{
    
    public class MyPoint
    {
        /// <summary>
        /// Gets the X point value
        /// </summary>
        public double X { get; internal set; }

        /// <summary>
        /// Gets the Y point value
        /// </summary>
        public double Y { get; internal set; }

        public DateTime timestamp { get; internal set; }

    }
}
