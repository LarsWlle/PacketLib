using PacketLib.Packets;

namespace PacketLib.Clients.Impl.Outbounds;

public class EncryptionPackageLayer : INetworkLayer {
    public int GetPriority() {
        return int.MaxValue;
    }

    public byte[] Handle(byte[] packet) {
        return packet;
    }
}