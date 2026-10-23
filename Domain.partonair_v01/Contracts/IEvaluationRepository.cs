using Domain.partonair_v01.Entities;


namespace Domain.partonair_v01.Contracts
{
    public interface IEvaluationRepository : IGenericRepository<Evaluation>
    {
        /// <summary>Avis reçus par un membre (auteur + son profil chargés), du plus récent au plus ancien.</summary>
        Task<List<Evaluation>> GetByReceiverAsync(Guid receiverId);
    }
}
