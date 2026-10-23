using BLL.partonair_v01.Interfaces;
using MediatR;
using SharedModels.partonair_v01.DTOS;


namespace BLL.partonair_v01.MediatR.Commands.Evaluations
{
    public class UpsertEvaluationCommandHandler(IEvaluationService evaluationService) : IRequestHandler<UpsertEvaluationCommand, EvaluationUpsertResultDTO>
    {
        private readonly IEvaluationService _evaluationService = evaluationService;

        public async Task<EvaluationUpsertResultDTO> Handle(UpsertEvaluationCommand request, CancellationToken cancellationToken)
        {
            return await _evaluationService.UpsertAsyncService(request.SenderId, request.ReceiverId, request.Evaluation);
        }
    }
}
