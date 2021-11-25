namespace ApplicationServer.Models
{
    public record DeleteFileSignedRequest(DeleteFileRequest Request, string Signature);
    public record DeleteFileRequest(string URL, string TaggedUsername);
}