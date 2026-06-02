using System.Net.Sockets;
using PacketLib.Clients;

namespace PacketLib;

class Program {
    static void Main(string[] args) {
        new Server(((tcp) => new Client(tcp))).Start();
    }

    private class Client(TcpClient client) : BaseClient(client);
}