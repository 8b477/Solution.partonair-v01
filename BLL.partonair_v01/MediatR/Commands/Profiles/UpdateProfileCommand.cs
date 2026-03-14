using MediatR;
using SharedModels.partonair_v01.DTOS;


namespace BLL.partonair_v01.MediatR.Commands.Profiles
{
    public record UpdateProfileCommand(Guid Id, ProfileUpdateDTO Profile) : IRequest<ProfileViewDTO>;
}
