namespace PacketLib.Packets;

public interface IPacket {
    public short GetId();
}

public interface IOutboundPacket : IPacket {
    public byte[] Package();
}

public interface IInboundPacket : IPacket {
    public void Handle(byte[] data);
}