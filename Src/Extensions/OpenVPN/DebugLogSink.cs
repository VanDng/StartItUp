using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenVPN
{
    public class StealthConsoleSink : ILogEventSink
    {
        //IFormatProvider _formatProvider;

        //public StealthConsoleSink(IFormatProvider formatProvider)
        //{
        //    _formatProvider = formatProvider;
        //}

        //public void Emit(LogEvent logEvent)
        //{
        //    Console.ForegroundColor = ConsoleColor.Black;
        //    Console.WriteLine(logEvent.RenderMessage(_formatProvider));
        //    Console.ResetColor();
        //}

        //public void Emit(LogEvent logEvent)
        //{
        //    throw new NotImplementedException();
        //}
        public void Emit(LogEvent logEvent)
        {
            throw new NotImplementedException();
        }
    }
}
