using PacketLib.Packets;

namespace PacketLib.Clients;

public interface IPacketHandlerLayer : INetworkLayer {
    IInboundPacket? Handle(IInboundPacket packet);
}