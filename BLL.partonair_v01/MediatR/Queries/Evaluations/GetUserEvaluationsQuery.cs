using MediatR;
using SharedModels.partonair_v01.DTOS;


namespace BLL.partonair_v01.MediatR.Queries.Evaluations
{
    public record GetUserEvaluationsQuery(Guid ViewerId, Guid UserId) : IRequest<UserEvaluationsDTO>;
}
