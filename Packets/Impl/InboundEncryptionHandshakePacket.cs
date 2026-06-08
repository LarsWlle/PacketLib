namespace PacketLib.Packets.Impl;

public class InboundEncryptionHandshakePacket : InboundPacket {
    public override ushort GetId() => ushort.MaxValue;

    public override void Handle(byte[] data, AbstractClient client) { }
}