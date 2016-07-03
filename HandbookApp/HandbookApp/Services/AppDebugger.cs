using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Splat;

namespace HandbookApp.Services
{
    public class AppDebugger : ILogger
    {
        public void Write(string message, LogLevel logLevel)
        {
            if ((int)logLevel < (int)Level) return;
            var dt = DateTime.Now; 
            Debug.WriteLine("{0:o}: {1}", dt, message);
        }

        public LogLevel Level { get; set; }
    }
}
