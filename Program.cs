using System.Net.Sockets;
using System.Runtime.CompilerServices;
using PacketLib.Clients;
using PacketLib.Clients.Impl.Inbounds;
using PacketLib.Clients.Impl.Outbounds;
using PacketLib.Configuration;

namespace PacketLib;

class Program {
    static void Main(string[] args) {
        Program prog = new Program();
        Task.Run(prog.TestClient);
        prog.TestServer();
    }

    private void TestServer() {
        Server server = new(((tcp, id, s) => new Client(tcp, id, s)));

        server.AddHandlerLayer(new EncryptionHandleLayer());
        server.AddPackageHandler(new EncryptionPackageLayer());
        server.Start();
    }

    private void TestClient() {
        CommunicationClient client = new();

        client.AddHandlerLayer(new EncryptionHandleLayer());
        client.AddPackageHandler(new EncryptionPackageLayer());
        client.Connect("127.0.0.1", 8080);
    }

    private class Client(TcpClient client, int id, Server server) : BaseClient(client, id, server);
}