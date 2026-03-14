using Domain.partonair_v01.Exceptions.Enums;


namespace Domain.partonair_v01.Exceptions.Handler
{
    /// <summary>
    /// Manages and centralizes error messages.
    /// </summary>
    public static class ErrorMessageHandler
    {

        private static readonly Dictionary<Enum, string> Messages = [];

        static ErrorMessageHandler()
        {
            InitializeErrorMessages();
        }

        private static void InitializeErrorMessages()
        {
            // ApplicationLayerErrorType
            AddErrorMessage(ApplicationLayerErrorType.ConstraintViolationErrorException, "The request conflicted with a constraint.");
            AddErrorMessage(ApplicationLayerErrorType.SaltParseBCryptException, "Some thing was wrong with the cryptage, check the password supplied.");
            AddErrorMessage(ApplicationLayerErrorType.EntityIsNotExistingException, "The entity don't match.");

            // InfrastructureLayerErrorType
            AddErrorMessage(InfrastructureLayerErrorType.ResourceNotFoundException, "The requested resource was not found.");
            AddErrorMessage(InfrastructureLayerErrorType.EntityIsNullException, "Entity is null.");
            AddErrorMessage(InfrastructureLayerErrorType.DatabaseConnectionErrorException, "Database connection failed");
            AddErrorMessage(InfrastructureLayerErrorType.ConcurrencyDatabaseException, "A concurrency conflict occurred while saving changes");
            AddErrorMessage(InfrastructureLayerErrorType.UpdateDatabaseException, "A database update error occurred while saving changes");
            AddErrorMessage(InfrastructureLayerErrorType.CancelationDatabaseException, "The operation was canceled");
            AddErrorMessage(InfrastructureLayerErrorType.UnexpectedDatabaseException, "An unexpected error occurred while saving changes");
            AddErrorMessage(InfrastructureLayerErrorType.NoActiveTransactionException, "Attempted to commit a transaction when no transaction was active.");
            AddErrorMessage(InfrastructureLayerErrorType.CreateDatabaseException, "The add request is failed.");
        }

        /// <summary>
        /// Ajoute un nouveau message d'erreur au dictionnaire.
        /// </summary>
        /// <param name="errorType">Le type d'erreur (doit être une énumération).</param>
        /// <param name="message">Le message d'erreur correspondant.</param>
        public static void AddErrorMessage(Enum errorType, string message)
        {
            Messages[errorType] = message;
        }

        /// <summary>
        /// Fournit un message pertinent basé sur le type d'erreur.
        /// </summary>
        /// <param name="errorType">Enum représentant le type d'erreur.</param>
        /// <returns>Retourne le message approprié pour le type d'erreur donné.</returns>
        public static string GetMessage(Enum errorType)
        {
            return Messages.TryGetValue(errorType, out var message)
                ? message
                : "An unknown error occurred.";
        }

    }
}
