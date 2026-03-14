using BLL.partonair_v01.Interfaces;

using MediatR;


namespace BLL.partonair_v01.MediatR.Commands.Evaluations
{
    public class DeleteEvaluationCommandHandler(IEvaluationService evaluationService) : IRequestHandler<DeleteEvaluationCommand>
    {
        private readonly IEvaluationService _evaluationService = evaluationService;
        public async Task Handle(DeleteEvaluationCommand request, CancellationToken cancellationToken)
        {
            await _evaluationService.DeleteAsyncService(request.Id);
        }
    }
}
