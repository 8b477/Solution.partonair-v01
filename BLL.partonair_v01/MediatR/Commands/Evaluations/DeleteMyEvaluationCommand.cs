using MediatR;


namespace BLL.partonair_v01.MediatR.Commands.Evaluations
{
    public record DeleteMyEvaluationCommand(Guid SenderId, Guid ReceiverId) : IRequest;
}
