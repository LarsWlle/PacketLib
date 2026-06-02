using System.Net.Sockets;
using PacketLib.Clients;
using PacketLib.Clients.Impl.Inbounds;
using PacketLib.Clients.Impl.Outbounds;
using PacketLib.Configuration;

namespace PacketLib;

class Program {
    static void Main(string[] args) {
        Server server = new Server(((tcp, id, config) => new Client(tcp, id, config)));

        server.AddHandlerLayer(new EncryptionHandleLayer());
        server.AddPackageHandler(new EncryptionPackageLayer());
        server.Start();
    }

    private class Client(TcpClient client, int id, Config config) : BaseClient(client, id, config);
}