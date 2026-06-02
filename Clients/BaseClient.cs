using System.Net.Sockets;
using PacketLib.Configuration;

namespace PacketLib.Clients;

public abstract class BaseClient {
    public BaseClient(TcpClient client) { }
}