namespace PacketLib.Clients;

public interface INetworkLayer {
    public byte[] Handle(byte[] data, AbstractClient client);
}