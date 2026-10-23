namespace Domain.partonair_v01.Enums
{
    /// <summary>Types de contrat proposés par un recruteur (combinables).</summary>
    [Flags]
    public enum ContractTypes
    {
        None = 0,
        CDI = 1,
        CDD = 2,
        Freelance = 4,
        Stage = 8,
        Alternance = 16
    }
}
