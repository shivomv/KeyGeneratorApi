namespace KeyGenerator.Services
{
    public interface ILoggerService
    {
        void LogEvent(string message,string category, int triggeredBy);
        void LogError(string error, string errorMsg,string controller);
    }
}
