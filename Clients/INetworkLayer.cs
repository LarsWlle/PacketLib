namespace PacketLib.Clients;

public interface INetworkLayer {
    public int GetPriority();

    public byte[] Handle(byte[] data);
}