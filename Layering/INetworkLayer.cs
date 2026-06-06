namespace PacketLib.Layering;

public interface INetworkLayer {
    public byte[] Handle(byte[] data, AbstractClient client);
}