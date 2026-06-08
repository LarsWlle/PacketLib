#region

using PacketLib.Crypto;

#endregion

namespace PacketLib.Layering.Impl.Outbounds;

public class EncryptionPackageLayer : INetworkLayer {
    public byte[] Handle(byte[] packet, AbstractClient client) =>
        client.Encryption.KeyExchangeStatus == Encryption.HandshakeStatus.Both ? client.Encryption.Encrypt(packet) : packet;
}