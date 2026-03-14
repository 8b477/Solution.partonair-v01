using SharedModels.partonair_v01.DTOS;

using MediatR;


namespace BLL.partonair_v01.MediatR.Queries.Evaluations
{
    public record GetByGuidEvaluationQuery(Guid Id) : IRequest<EvaluationViewDTO>;

}
