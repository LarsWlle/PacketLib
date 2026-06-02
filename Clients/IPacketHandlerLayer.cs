using PacketLib.Packets;

namespace PacketLib.Clients;

public interface IPacketHandlerLayer : INetworkLayer {
    byte[] Handle(byte[] data);
}