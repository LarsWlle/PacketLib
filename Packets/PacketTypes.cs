namespace PacketLib.Packets;

public interface IPacket {
    public ushort GetId();
}

public interface IOutboundPacket : IPacket {
    public byte[] Package();
}

public interface IInboundPacket : IPacket {
    public void Handle(byte[] data);
}