namespace PacketLib.Layering.Impl.Inbounds;

public class EncryptionHandleLayer : INetworkLayer {
    public int GetPriority() {
        return int.MinValue;
    }

    public byte[] Handle(byte[] data, AbstractClient client) {
        return data;
    }
}