namespace PacketLib.Layering.Impl.Inbounds;

public class EncryptionHandleLayer : INetworkLayer {
    public int GetPriority() => int.MinValue;

    public byte[] Handle(byte[] data, AbstractClient client) => client.Encryption.IsHandshakeComplete ? client.Encryption.Decrypt(data) : data;
}