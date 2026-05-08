

namespace API.partonair_v01.Token
{
    public static class MapperTokenDatas
    {
        public static Guid FromStringToGuid(this string data)
        {
            _ = Guid.TryParse(data, out Guid guid);

            return guid;
        }
    }
}
