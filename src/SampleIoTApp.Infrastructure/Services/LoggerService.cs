using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SampleIoTApp.Application.Services;
using Serilog;
using Serilog.Core;
using Serilog.Formatting.Compact;

namespace SampleIoTApp.Infrastructure.Services
{
    public class LoggerService : ILoggerService
    {
        private readonly Logger _logger;
        private string? _traceId;
        public LoggerService()
        {
            this._logger = new LoggerConfiguration()
                            .WriteTo.Console(new RenderedCompactJsonFormatter())
                            .CreateLogger();
        }
        public void AddTraceId(string traceId)
        {
            this._traceId = traceId;
        }

        public void LogInfo(string message)
        {
            this._logger.ForContext("TraceId", this._traceId).Information(message);
        }
    }
}