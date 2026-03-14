using SharedModels.partonair_v01.DTOS;
using BLL.partonair_v01.Interfaces;

using MediatR;


namespace BLL.partonair_v01.MediatR.Queries.Evaluations
{
    public class GetByGuidEvaluationQueryHandler(IEvaluationService evaluationService) : IRequestHandler<GetByGuidEvaluationQuery, EvaluationViewDTO>
    {
        private readonly IEvaluationService _evaluationService = evaluationService;
        public async Task<EvaluationViewDTO> Handle(GetByGuidEvaluationQuery request, CancellationToken cancellationToken)
        {
            return await _evaluationService.GetByGuidAsyncService(request.Id);
        }
    }
}
