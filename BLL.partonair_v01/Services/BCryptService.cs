using BCrypt.Net;
using BLL.partonair_v01.Interfaces;
using Domain.partonair_v01.Exceptions;
using Domain.partonair_v01.Exceptions.Enums;


namespace BLL.partonair_v01.Services
{
    public class BCryptService : IBCryptService
    {
        public string HashPass(string passToHash, int workFactor)
        {
            try
            {
                string passwordHashed = BCrypt.Net.BCrypt.EnhancedHashPassword(passToHash, workFactor);

                return passwordHashed;
            }
            catch (SaltParseException ex)
            {
                throw new ApplicationLayerException(ApplicationLayerErrorType.SaltParseBCryptException, $"{ex.Message}");
            }
        }

        public bool VerifyPasswordMatch(string actualPass, string passHashed)
        {
            try
            {
                bool check = BCrypt.Net.BCrypt.EnhancedVerify(actualPass, passHashed);

                return check;
            }
            catch (SaltParseException)
            {
                throw new ApplicationLayerException(ApplicationLayerErrorType.SaltParseBCryptException);
            }         
        }
    }
}
