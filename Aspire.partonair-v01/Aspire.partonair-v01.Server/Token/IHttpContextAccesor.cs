namespace API.partonair_v01.Token
{
    public interface IHttpContextAccesor
    {
        string? UserId();
        string? Role();
    }
}
