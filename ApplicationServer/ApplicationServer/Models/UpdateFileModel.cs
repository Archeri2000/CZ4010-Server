using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ApplicationServer.Models
{
    public record UpdateFileSignedRequest(UpdateFileRequest Request, string Signature);

    public record UpdateFileRequest(string URL, string EncryptedFile, string TaggedUsername);

    public record UpdateFileResponse(string URL);

}