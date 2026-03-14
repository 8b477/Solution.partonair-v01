using SharedModels.partonair_v01.DTOS;
using BLL.partonair_v01.Interfaces;

using MediatR;


namespace BLL.partonair_v01.MediatR.Commands.Evaluations
{
    public class AddEvaluationCommandHandler(IEvaluationService evaluationService) : IRequestHandler<AddEvaluationCommand, EvaluationViewDTO>
    {
        private readonly IEvaluationService _evaluationService = evaluationService;
        public async Task<EvaluationViewDTO> Handle(AddEvaluationCommand request, CancellationToken cancellationToken)
        {
            return await _evaluationService.CreateAsyncService(request.IdSender,request.Eval);
        }
    }
}
