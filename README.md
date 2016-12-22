# Serilog.Sinks.ContextRollingFile [![NuGet Version](http://img.shields.io/nuget/v/Serilog.Sinks.ContextRollingFile.svg?style=flat)](https://www.nuget.org/packages/Serilog.Sinks.ContextRollingFile/)

Writes [Serilog](https://serilog.net) events to a set of text files, one per day for each context.

### Getting started

Install the [Serilog.Sinks.ContextRollingFile](https://nuget.org/packages/serilog.sinks.contextrollingfile) package from NuGet:

```powershell
Install-Package Serilog.Sinks.ContextRollingFile
```

To configure the sink in C# code, call `WriteTo.ContextRollingFile()` during logger configuration:

```csharp
var log = new LoggerConfiguration()
    .WriteTo.ContextRollingFile("log-{MyProperty:l}-{Date}.txt")
    .CreateLogger();
    
Log.Information("This will be written to the rolling file set");
```

## Predefined Placeholders (HalfHour, Hour, Date)
The filename should include one the the predefined placeholders `{HalfHour}`, `{Hour}` or `{Date}` , which will be replaced with the date of the events contained in the file. Filenames use the `yyyyMMdd` date format so that files can be ordered using a lexicographic sort:

```
log-foo-20160631.txt
log-foo-20160701.txt
log-foo-20160702.txt
```

## Custom Placeholders

The filename may include one or more placeholders, which will be replaced with the properties of each log event. This can be useful if you want to have seperate log files depending on the context. Please be aware that Serilog renders string values in double quotes by default. Since quotes are not allowed in paths and filenames, you omit them using the :l literal format specifier on the property, e.g. {MyProperty:l}

The following sample creates two seperate files based on the Context property.
```csharp
var log = new LoggerConfiguration()
    .WriteTo.ContextRollingFile("log-{Context:l}-{Date}.txt")
    .CreateLogger();
    
Log.Information("This will be written to the {Context} file set", "Foo");
Log.Information("This will be written to the {Context} file set", "Bar");
```

```
log-Foo-20160701.txt
log-Bar-20160702.txt
```

**Note**: `HalfHour`, `Hour` and `Date` are predefined placeholders. As such they won't be replaced in the path template by your own properties.

## Source Contexts
Although you can use any placeholders you like, this can be particularly useful in combination with the `ForContext` method:
```csharp
var log = new LoggerConfiguration()
    .WriteTo.ContextRollingFile("log-{SourceContext:l}-{Date}.txt")
    .CreateLogger()
    .ForContext<MyClass>();
    
myLog.Information("Hello!");
```
The event written will include a property SourceContext with value "MyNamespace.MyClass" that is used in the path template to create the log files.

```
log-MyNamespace.MyClass-20160631.txt
log-MyNamespace.MyClass-20160701.txt
log-MyNamespace.MyClass-20160702.txt
```

### Other Configuration Options
The ContextRollingFile sink extends the RollingFile sink and supports the same configuration options. Refer to the [Serilog.Sinks.RollingFile](https://github.com/serilog/serilog-sinks-rollingfile) documentation for these options and their usage.
