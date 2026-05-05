using Domain.partonair_v01.Contracts;
using MediatR;

namespace BLL.partonair_v01.MediatR.Commands.Users
{
    public class ChangeUserRoleCommandHandler(IUnitOfWork uow) : IRequestHandler<ChangeUserRoleCommand, bool>
    {
        private readonly IUnitOfWork _uow = uow;

        public async Task<bool> Handle(ChangeUserRoleCommand request, CancellationToken cancellationToken)
        {
            bool changed = await _uow.Users.ChangeRoleAsync(request.Id, request.Role);

            if (changed)
                await _uow.SaveChangesAsync(cancellationToken);

            return changed;
        }
    }
}
