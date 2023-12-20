namespace SampleIoTApp.Application.Services
{
    public interface ILoggerService
    {
        void LogInfo(string message);

        void AddTraceId(string traceId);
    }
}