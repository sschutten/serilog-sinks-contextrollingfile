using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog.Events;
using Serilog.Formatting;
using System.Text;
using System.Collections.Concurrent;
using Serilog.Sinks.RollingFile;
using Serilog.Parsing;
using Serilog.Sinks.File;
using System.Collections.ObjectModel;

namespace Serilog.Sinks.ContextRollingFile
{
    public class ContextRollingFileSink : ILogEventSink, IFlushableFileSink, IDisposable
    {
        readonly MessageTemplate _pathTemplate;
        readonly ITextFormatter _textFormatter;
        readonly long? _fileSizeLimitBytes;
        readonly int? _retainedFileCountLimit;
        readonly Encoding _encoding;
        readonly bool _buffered;
        readonly bool _shared;

        ConcurrentDictionary<string, RollingFileSink> _sinks = new ConcurrentDictionary<string, RollingFileSink>();

        /// <summary>Construct a <see cref="RollingFileSink"/>.</summary>
        /// <param name="pathFormat">String describing the location of the log files,
        /// with {Date} in the place of the file date. E.g. "Logs\myapp-{Date}.log" will result in log
        /// files such as "Logs\myapp-2013-10-20.log", "Logs\myapp-2013-10-21.log" and so on.</param>
        /// <param name="textFormatter">Formatter used to convert log events to text.</param>
        /// <param name="fileSizeLimitBytes">The maximum size, in bytes, to which a log file will be allowed to grow.
        /// For unrestricted growth, pass null. The default is 1 GB.</param>
        /// <param name="retainedFileCountLimit">The maximum number of log files that will be retained,
        /// including the current log file. For unlimited retention, pass null. The default is 31.</param>
        /// <param name="encoding">Character encoding used to write the text file. The default is UTF-8 without BOM.</param>
        /// <param name="buffered">Indicates if flushing to the output file can be buffered or not. The default
        /// is false.</param>
        /// <param name="shared">Allow the log files to be shared by multiple processes. The default is false.</param>
        /// <returns>Configuration object allowing method chaining.</returns>
        /// <remarks>The file will be written using the UTF-8 character set.</remarks>
        public ContextRollingFileSink(string pathFormat,
                              ITextFormatter textFormatter,
                              long? fileSizeLimitBytes,
                              int? retainedFileCountLimit,
                              Encoding encoding = null,
                              bool buffered = false,
                              bool shared = false)
        {
            if (pathFormat == null) throw new ArgumentNullException(nameof(pathFormat));
            if (fileSizeLimitBytes.HasValue && fileSizeLimitBytes < 0) throw new ArgumentException("Negative value provided; file size limit must be non-negative");
            if (retainedFileCountLimit.HasValue && retainedFileCountLimit < 1) throw new ArgumentException("Zero or negative value provided; retained file count limit must be at least 1");

            _textFormatter = textFormatter;
            _fileSizeLimitBytes = fileSizeLimitBytes;
            _retainedFileCountLimit = retainedFileCountLimit;
            _encoding = encoding;
            _buffered = buffered;
            _shared = shared;

            _pathTemplate = new MessageTemplateParser().Parse(pathFormat);
        }

        public void Emit(LogEvent logEvent)
        {
            var path = RenderPath(logEvent.Properties);
            var sink = _sinks.GetOrAdd(path, CreateSink);
            sink.Emit(logEvent);
        }

        public string RenderPath(IReadOnlyDictionary<string, LogEventPropertyValue> properties)
        {
            var propertiesWithoutDate = properties.Where(p => p.Key != "Date")
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            return _pathTemplate.Render(propertiesWithoutDate);
        }

        private RollingFileSink CreateSink(string pathFormat)
        {
            return new RollingFileSink(pathFormat,
                    _textFormatter, _fileSizeLimitBytes,
                    _retainedFileCountLimit, _encoding, _buffered,
                    _shared);
        }

        public void Dispose()
        {
            foreach (var sink in _sinks.Values)
            {
                sink.Dispose();
            }
        }

        public void FlushToDisk()
        {
            foreach (var sink in _sinks.Values)
            {
                sink.FlushToDisk();
            }
        }
    }
}
