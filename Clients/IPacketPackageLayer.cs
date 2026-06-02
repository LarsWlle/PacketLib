using PacketLib.Packets;

namespace PacketLib.Clients;

public interface IPacketPackageLayer {
    IOutboundPacket? Handle(IOutboundPacket packet);
}