using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ApplicationServer.Models
{
    public record SubmitFileSignedRequest(SubmitFileRequest Request, string Signature);

    public record SubmitFileRequest(string EncryptedFile, string EncryptedKey, string TaggedUsername);

    public record SubmitFileResponse(string URL);

}