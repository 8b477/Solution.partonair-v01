using MediatR;
using SharedModels.partonair_v01.DTOS;


namespace BLL.partonair_v01.MediatR.Commands.Profiles
{
    public record AddProfileCommand(Guid IdUser, ProfileCreateDTO Entity) : IRequest<ProfileViewDTO>;
}