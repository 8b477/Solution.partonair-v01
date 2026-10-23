using MediatR;
using SharedModels.partonair_v01.DTOS;


namespace BLL.partonair_v01.MediatR.Commands.Evaluations
{
    public record UpsertEvaluationCommand(Guid SenderId, Guid ReceiverId, EvaluationUpsertDTO Evaluation) : IRequest<EvaluationUpsertResultDTO>;
}
