namespace PacketLib.Layering.Impl.Inbounds;

public class EncryptionHandleLayer : INetworkLayer {
    public byte[] Handle(byte[] data, AbstractClient client) => client.Encryption.IsHandshakeComplete ? client.Encryption.Decrypt(data) : data;
}