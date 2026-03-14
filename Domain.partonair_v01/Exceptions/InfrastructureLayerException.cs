using Domain.partonair_v01.Exceptions.Enums;
using Domain.partonair_v01.Exceptions.Handler;

namespace Domain.partonair_v01.Exceptions
{
    public class InfrastructureLayerException : Exception
    {
        public InfrastructureLayerErrorType ErrorType {  get; private set; }
        public InfrastructureLayerException(InfrastructureLayerErrorType errorType, string additionalInfo = "")
            : base(FormatExceptionMessage(errorType, additionalInfo))
        {
            ErrorType = errorType;
        }
        private static string FormatExceptionMessage(InfrastructureLayerErrorType errorType, string additionalInfo)
        {
            string baseMessage = ErrorMessageHandler.GetMessage(errorType);
            return string.IsNullOrEmpty(additionalInfo) ? baseMessage : $"{baseMessage} {additionalInfo}".Trim();
        }
    }
}
