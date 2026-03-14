using MediatR;


namespace BLL.partonair_v01.MediatR.Commands.Profiles
{
    public record DeleteProfileCommand(Guid Id) : IRequest;
}
