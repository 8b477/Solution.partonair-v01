
namespace SharedModels.partonair_v01.DTOS
{
    public record LoginDefaultResponse(
        string Token,
        Guid UserId,
        string UserName,
        string Role,
        Guid? ProfileId,
        string? AvatarSeed);
}
