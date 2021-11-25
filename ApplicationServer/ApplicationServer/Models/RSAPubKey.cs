using System;
using System.Buffers.Text;
using System.Security.Cryptography;

namespace ApplicationServer.Models
{
    public record RSAPubKey(string Modulus, string Exponent)
    {
        public RSAParameters ToRsaParams()
        {
            return new RSAParameters { Modulus = Convert.FromBase64String(Modulus), Exponent = Convert.FromBase64String(Exponent) };
        }
    }
}