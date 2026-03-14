    namespace BLL.partonair_v01.Interfaces
{
    public interface IBCryptService
    {
        string HashPass(string passToHash, int workFactor = 13);
        bool VerifyPasswordMatch(string actualPass, string passHashed);
    }
}
