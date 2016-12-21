using Serilog.Sinks.ContextRollingFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serilog.Sinks.ContextRollingFile.TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new LoggerConfiguration()
                .WriteTo.ContextRollingFile("{Context:l}_{Date}.txt")
                .CreateLogger();

            logger.Information("This logs to the {Context} context", "Foo");
            logger.Information("This logs to the {Context} context", "Bar");
        }
    }
}
