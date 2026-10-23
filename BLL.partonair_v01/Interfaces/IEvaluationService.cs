using SharedModels.partonair_v01.DTOS;


namespace BLL.partonair_v01.Interfaces
{
    public interface IEvaluationService
    {
        Task<UserEvaluationsDTO> GetUserEvaluationsAsyncService(Guid viewerId, Guid userId);
        Task<EvaluationUpsertResultDTO> UpsertAsyncService(Guid senderId, Guid receiverId, EvaluationUpsertDTO evaluation);
        Task DeleteMineAsyncService(Guid senderId, Guid receiverId);
    }
}
