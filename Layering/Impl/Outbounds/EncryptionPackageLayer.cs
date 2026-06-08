namespace PacketLib.Layering.Impl.Outbounds;

public class EncryptionPackageLayer : INetworkLayer {
    public byte[] Handle(byte[] packet, AbstractClient client) => client.Encryption.IsHandshakeComplete ? client.Encryption.Encrypt(packet) : packet;
}