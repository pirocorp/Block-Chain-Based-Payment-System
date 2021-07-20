namespace PaymentSystem.WalletApp.Services
{
    public interface ISecurelyEncryptDataService
    {
        byte[] EncryptData(object data, byte[] keyBytes, byte[] saltBytes);

        string EncryptDataHex(object data, byte[] keyBytes, byte[] saltBytes);

        byte[] DecryptBytes(byte[] cryptBytes, byte[] keyBytes, byte[] saltBytes);

        T Decrypt<T>(byte[] cryptBytes, byte[] keyBytes, byte[] saltBytes);
    }
}
