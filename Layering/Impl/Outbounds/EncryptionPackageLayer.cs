namespace PacketLib.Layering.Impl.Outbounds;

public class EncryptionPackageLayer : INetworkLayer {
    public int GetPriority() {
        return int.MaxValue;
    }

    public byte[] Handle(byte[] packet, AbstractClient client) {
        if (!client.Encryption.IsHandshakeComplete) return packet;
        return client.Encryption.Encrypt(packet);
    }
}