using FluentAssertions;
using Serilog.Events;
using Serilog.Parsing;
using System.Collections.Generic;
using Xunit;
using Serilog.Sinks.ContextRollingFile;
using Serilog.Formatting.Display;

namespace Serilog.Sinks.ContextRollingFile.Tests
{
    public class ContextRollingFileSinkTest
    {
        [Fact]
        public void WhenRenderingPathTemplateDateTokenRemains()
        {
            var formatter = new MessageTemplateTextFormatter(string.Empty, null);
            var sink = new ContextRollingFileSink("/Logs/{MyToken:l}_{Date}.txt", formatter, 1, 1);

            var properties = new Dictionary<string, LogEventPropertyValue>
                {
                    { "MyToken", new ScalarValue("Foo") }
                };
            var path = sink.RenderPath(properties);

            path.Should().Be("/Logs/Foo_{Date}.txt");
        }

        [Fact]
        public void WhenRenderingDateTokenShouldNotBeParsed()
        {
            var formatter = new MessageTemplateTextFormatter(string.Empty, null);
            var sink = new ContextRollingFileSink("/Logs/{MyToken:l}_{Date}.txt", formatter, 1, 1);

            var properties = new Dictionary<string, LogEventPropertyValue>
                {
                    { "MyToken", new ScalarValue("Foo") },
                    { "Date", new ScalarValue("Bar") }
                };
            var path = sink.RenderPath(properties);

            path.Should().Be("/Logs/Foo_{Date}.txt");
        }
    }
}