using SharedModels.partonair_v01.DTOS;
using MediatR;


namespace BLL.partonair_v01.MediatR.Queries.Contacts
{
    public record GetAllBlockedStatusQuery(string Status) : IRequest<ICollection<ContactViewDTO>>;
}
