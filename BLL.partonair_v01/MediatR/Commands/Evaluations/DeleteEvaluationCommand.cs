using MediatR;


namespace BLL.partonair_v01.MediatR.Commands.Evaluations
{
    public record DeleteEvaluationCommand(Guid Id) : IRequest;
}
