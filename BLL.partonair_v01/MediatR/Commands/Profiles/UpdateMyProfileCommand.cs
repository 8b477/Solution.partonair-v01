using MediatR;
using SharedModels.partonair_v01.DTOS;


namespace BLL.partonair_v01.MediatR.Commands.Profiles
{
    public record UpdateMyProfileCommand(Guid IdUser, MyProfileUpdateDTO Profile) : IRequest<MyProfileViewDTO>;
}
