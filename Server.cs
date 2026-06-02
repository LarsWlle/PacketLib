using System.Net;
using System.Net.Sockets;
using PacketLib.Clients;
using PacketLib.Configuration;

namespace PacketLib;

public class Server(Func<TcpClient, int, BaseClient> clientFactory) {
    public Config Config { get; private set; } = new();
    private List<BaseClient> _clients = [];
    private List<IPacketHandlerLayer> _handleLayers = [];
    private List<IPacketPackageLayer> _packageLayers = [];

    private int _lastClientId = 0;

    public void AddPackageHandler(IPacketPackageLayer handler) {
        this._packageLayers.Add(handler);
        Logger.Debug($"Added new packet package layer with priority = {handler.GetPriority()}");
    }

    public void AddHandlerLayer(IPacketHandlerLayer handler) {
        this._handleLayers.Add(handler);
        Logger.Debug($"Added new packet handler layer with priority = {handler.GetPriority()}");
    }

    public void Start() {
        TcpListener listener = new(IPAddress.Any, this.Config.Port) {
            Server = {
                ReceiveTimeout = this.Config.ReceiveTimeout,
                SendTimeout = this.Config.SendTimeout,
            }
        };

        listener.Start();

        Task.Run(() => {
            TcpClient tcpClient = listener.AcceptTcpClient();
            int id = ++this._lastClientId;
            Logger.Info($"Client connected, id = {id}");
            BaseClient client = clientFactory(tcpClient, id);
            this._clients.Add(client);
        });
    }
}