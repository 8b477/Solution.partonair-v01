using BLL.partonair_v01.Interfaces;
using MediatR;


namespace BLL.partonair_v01.MediatR.Commands.Evaluations
{
    public class DeleteMyEvaluationCommandHandler(IEvaluationService evaluationService) : IRequestHandler<DeleteMyEvaluationCommand>
    {
        private readonly IEvaluationService _evaluationService = evaluationService;

        public async Task Handle(DeleteMyEvaluationCommand request, CancellationToken cancellationToken)
        {
            await _evaluationService.DeleteMineAsyncService(request.SenderId, request.ReceiverId);
        }
    }
}
