using PacketLib.Packets;

namespace PacketLib.Clients;

public interface IPacketPackageLayer : INetworkLayer {
    IOutboundPacket? Handle(IOutboundPacket packet);
}