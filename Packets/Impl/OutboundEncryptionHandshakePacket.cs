namespace PacketLib.Packets.Impl;

public class OutboundEncryptionHandshakePacket : OutboundPacket {
    public override ushort GetId() => ushort.MaxValue;

    public override byte[] Package() {
        return [];
    }
}