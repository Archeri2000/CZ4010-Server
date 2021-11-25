using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ApplicationServer.Models
{
    public record GetFileRequest(string Url, string TaggedUsername);

    public record GetFileResponse(string EncryptedFile, string EncryptedKey);
}