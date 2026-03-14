using SharedModels.partonair_v01.DTOS;

using MediatR;


namespace BLL.partonair_v01.MediatR.Commands.Evaluations
{
    public record AddEvaluationCommand(Guid IdSender, EvaluationCreateDTO Eval) : IRequest<EvaluationViewDTO>;
}
