using PacketLib.Packets;

namespace PacketLib.Clients;

public interface IPacketPackageLayer : INetworkLayer {
    byte[] Handle(byte[] packet);
}