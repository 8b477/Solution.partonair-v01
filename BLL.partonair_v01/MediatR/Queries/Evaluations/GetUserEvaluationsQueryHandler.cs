using BLL.partonair_v01.Interfaces;
using MediatR;
using SharedModels.partonair_v01.DTOS;


namespace BLL.partonair_v01.MediatR.Queries.Evaluations
{
    public class GetUserEvaluationsQueryHandler(IEvaluationService evaluationService) : IRequestHandler<GetUserEvaluationsQuery, UserEvaluationsDTO>
    {
        private readonly IEvaluationService _evaluationService = evaluationService;

        public async Task<UserEvaluationsDTO> Handle(GetUserEvaluationsQuery request, CancellationToken cancellationToken)
        {
            return await _evaluationService.GetUserEvaluationsAsyncService(request.ViewerId, request.UserId);
        }
    }
}
