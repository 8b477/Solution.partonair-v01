using MediatR;
using SharedModels.partonair_v01.DTOS;

namespace BLL.partonair_v01.MediatR.Queries.Profiles
{
    public record class GetByIdProfileQuery(Guid IdUser) : IRequest<ProfileViewDTO>;
}
