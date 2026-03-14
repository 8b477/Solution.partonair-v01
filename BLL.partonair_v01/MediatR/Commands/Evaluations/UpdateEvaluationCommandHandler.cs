using SharedModels.partonair_v01.DTOS;
using BLL.partonair_v01.Interfaces;

using MediatR;


namespace BLL.partonair_v01.MediatR.Commands.Evaluations
{
    public class UpdateEvaluationCommandHandler(IEvaluationService evaluationService) : IRequestHandler<UpdateEvaluationCommand, EvaluationViewDTO>
    {
        private readonly IEvaluationService _evaluationService = evaluationService;
        public async Task<EvaluationViewDTO> Handle(UpdateEvaluationCommand request, CancellationToken cancellationToken)
        {
            return await _evaluationService.UpdateAsyncService(request.IdEval,request.Eval);
        }
    }
}
