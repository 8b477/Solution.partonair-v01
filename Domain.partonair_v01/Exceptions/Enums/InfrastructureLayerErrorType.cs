namespace Domain.partonair_v01.Exceptions.Enums
{
    public enum InfrastructureLayerErrorType
    {
        ResourceNotFoundException,
        EntityIsNullException,
        DatabaseConnectionErrorException,
        CreateDatabaseException,
        ConcurrencyDatabaseException,
        UpdateDatabaseException,
        CancelationDatabaseException,
        NoActiveTransactionException,
        UnexpectedDatabaseException,
    }
}
