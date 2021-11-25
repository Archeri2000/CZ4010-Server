using System.Collections.Generic;

namespace ApplicationServer.Models
{
    public record SharingFileSignedRequest(SharingFileRequest Request, string Signature);

    public record SharingFileRequest(string URL, UsernameKeyPair[] TaggedUsernamesToShareWith, string CallerTaggedUsername);

    public record UsernameKeyPair(string EncryptedKey, string TaggedUsername);
    public record SharingFileResponse(string URL, IEnumerable<string> TaggedUsernames);

    public record UnsharingFileSignedRequest(UnsharingFileRequest Request, string Signature);

    public record UnsharingFileRequest(string URL, string[] TaggedUsernamesToRemove, string CallerTaggedUsername);
}