using PacketLib.Packets;

namespace PacketLib.Clients;

public interface IPacketHandlerLayer {
    IInboundPacket? Handle(IInboundPacket packet);
}