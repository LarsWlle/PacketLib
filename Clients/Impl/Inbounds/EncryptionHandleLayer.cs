namespace PacketLib.Clients.Impl.Inbounds;

public class EncryptionHandleLayer : IPacketHandlerLayer {
    public int GetPriority() {
        return int.MinValue;
    }

    public byte[] Handle(byte[] data) {
        return data;
    }
}