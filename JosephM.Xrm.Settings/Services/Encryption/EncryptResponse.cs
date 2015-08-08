namespace JosephM.Xrm.Settings.Services.Encryption
{
    public class EncryptResponse
    {
        public string Encrypted { get; set; }
        public string EncryptedVi { get; set; }

        public EncryptResponse(string encrypted, string encryptedVi)
        {
            Encrypted = encrypted;
            EncryptedVi = encryptedVi;
        }
    }
}