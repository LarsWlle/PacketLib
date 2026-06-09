namespace PacketLib.Packets.Impl;

internal class OutboundEncryptionHandshakePacket(AbstractClient client) : OutboundPacket {
    public override ushort GetId() => ushort.MaxValue;

    public override byte[] Package() => client.Encryption.PublicKey;
}