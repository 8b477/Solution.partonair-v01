using SharedModels.partonair_v01.DTOS;
using MediatR;

namespace BLL.partonair_v01.MediatR.Queries.Contacts
{
   public record GetAllPendingStatusQuery(string Status) : IRequest<ICollection<ContactViewDTO>>;
}
