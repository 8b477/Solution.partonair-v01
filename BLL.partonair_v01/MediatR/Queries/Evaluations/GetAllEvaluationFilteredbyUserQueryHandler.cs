using SharedModels.partonair_v01.DTOS;
using BLL.partonair_v01.Interfaces;

using MediatR;


namespace BLL.partonair_v01.MediatR.Queries.Evaluations
{
    public class GetAllEvaluationFilteredbyUserQueryHandler(IEvaluationService evaluationService) : IRequestHandler<GetAllEvaluationFilteredbyUserQuery, ICollection<EvaluationViewDTO>>
    {
        private readonly IEvaluationService _evaluationService = evaluationService;
        public async Task<ICollection<EvaluationViewDTO>> Handle(GetAllEvaluationFilteredbyUserQuery request, CancellationToken cancellationToken)
        {
            return await _evaluationService.GetAllGrouppByUserAsyncService();
        }
    }
}
