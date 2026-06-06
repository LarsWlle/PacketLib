using System.Net;
using System.Net.Sockets;
using PacketLib.Configuration;
using PacketLib.Layering;
using PacketLib.Packets;

namespace PacketLib.Server;

public class Server(Func<TcpClient, int, Server, BaseClient> clientFactory) {
    public Config Config { get; } = new();
    private readonly List<BaseClient> _clients = [];

    private int _lastClientId = 0;

    public LayerPipeline HandleLayers => new();
    public LayerPipeline PackageLayers => new();

    public readonly PacketList InboundPackets = new();

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