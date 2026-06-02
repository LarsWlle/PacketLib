using System.Net;
using System.Net.Sockets;
using PacketLib.Clients;
using PacketLib.Configuration;
using PacketLib.Packets;

namespace PacketLib;

public class Server(Func<TcpClient, int, Server, BaseClient> clientFactory) : CommunicationParticipant {
    public Config Config { get; } = new();
    private readonly List<BaseClient> _clients = [];

    private int _lastClientId = 0;

    public void Start() {
        TcpListener listener = new(IPAddress.Any, this.Config.Port) {
            Server = {
                ReceiveTimeout = this.Config.ReceiveTimeout,
                SendTimeout = this.Config.SendTimeout,
            }
        };

        try {
            listener.Start();
        }
        catch (SocketException ex) {
            Logger.Fatal($"Could not start listener. Reason: {ex}");
        }

        while (true) {
            TcpClient tcpClient = listener.AcceptTcpClient();
            int id = ++this._lastClientId;
            Logger.Info($"Client connected, id = {id}");
            BaseClient client = clientFactory(tcpClient, id, this);
            this._clients.Add(client);
        }
    }

    public BaseClient GetClient(int id) {
        return this._clients.First(c => c.Id == id);
    }
}