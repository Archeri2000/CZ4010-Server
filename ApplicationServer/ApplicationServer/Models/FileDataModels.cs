namespace ApplicationServer.Models
{
    public record FileDataModel(string URL,string EncryptedFile);

    public record SharingDataModel(string URL, string TaggedUsername, bool IsOwner, string EncryptedKey);
}