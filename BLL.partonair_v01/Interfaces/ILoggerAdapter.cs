
namespace BLL.partonair_v01.Interfaces
{


    public interface ILoggerAdapter<T>
    {
        void LogInformation(string message, params object[] args);
        void LogWarning(string message, params object[] args);
        void LogError(Exception ex, string message, params object[] args);
        IDisposable BeginScope(string messageFormat, params object[] args);
    }

}
