using KeyGenerator.Data;
using KeyGenerator.Models;


namespace KeyGenerator.Services
{
    public class LoggerService : ILoggerService
    {
        private readonly KeyGeneratorDBContext _context;

        public LoggerService(KeyGeneratorDBContext context)
        {
            _context = context;
        }

        public void LogEvent(string message, string category, int triggeredBy)
        {
            var log = new EventLog
            {
                Event = message,
                EventTriggeredBy = triggeredBy,
                Category = category,
            };

            _context.EventLogs.Add(log);
            _context.SaveChanges();
        }

        public void LogError(string error,string errormessage, string Controller)
        {
            var log = new ErrorLog
            {
                Error = error,
                Message = errormessage,
                OccuranceSpace = Controller
            };

            _context.ErrorLogs.Add(log);
            _context.SaveChanges();
        }
    }
}
