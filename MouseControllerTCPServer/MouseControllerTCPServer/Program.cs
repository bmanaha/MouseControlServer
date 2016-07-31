using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MouseControllerTCPServer
{
    public class Program
    {
        public const Int32 Port = 13000;
        static void Main()
        {
            SetupTracing(); // sets up tracing
            CursorServer server = new CursorServer(Port);
            server.Start();

        }
        private static void SetupTracing() // set listener to trace
        {
            Trace.Listeners.Add(new ConsoleTraceListener());
            Trace.AutoFlush = true; //makes sure that Traces will be "flushed" immidiately
        }
    }
}
