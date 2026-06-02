using PacketLib.Packets;

namespace PacketLib.Clients.Impl.Outbounds;

public class EncryptionPackageLayer : IPacketPackageLayer {
    public int GetPriority() {
        return int.MinValue;
    }

    public byte[] Handle(byte[] packet) {
        return packet;
    }
}