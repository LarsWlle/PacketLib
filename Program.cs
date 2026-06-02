using System.Net.Sockets;
using PacketLib.Clients;
using PacketLib.Clients.Impl.Inbounds;
using PacketLib.Clients.Impl.Outbounds;
using PacketLib.Configuration;

namespace PacketLib;

class Program {
    static void Main(string[] args) {
        Server server = new(((tcp, id, s) => new Client(tcp, id, s)));

        server.AddHandlerLayer(new EncryptionHandleLayer());
        server.AddPackageHandler(new EncryptionPackageLayer());
        server.Start();
    }

    private class Client(TcpClient client, int id, Server server) : BaseClient(client, id, server);
}