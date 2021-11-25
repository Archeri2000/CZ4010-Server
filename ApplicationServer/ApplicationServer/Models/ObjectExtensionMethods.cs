using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ApplicationServer.Models
{
    public static class ObjectExtensionMethods
    {
        public static byte[] ToBytes(this object x)
        {
            return Encoding.ASCII.GetBytes(JsonSerializer.Serialize(x));
        }

        public static byte[] GetSHA256(this object x)
        {
            return SHA256.HashData(x.ToBytes());
        }
    }
}