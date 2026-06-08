#region

using PacketLib.Crypto;

#endregion

namespace PacketLib.Packets.Impl;

public class OutboundEncryptionHandshakePacket(AbstractClient client) : OutboundPacket {
    public override ushort GetId() => ushort.MaxValue;

    public override byte[] Package() {
        client.Encryption.KeyExchangeStatus &= Encryption.HandshakeStatus.Sent;
        return client.Encryption.PublicKey;
    }
}