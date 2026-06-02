using System.Net;
using System.Net.Sockets;
using PacketLib.Clients;
using PacketLib.Configuration;
using PacketLib.Packets;

namespace PacketLib;

public class Server(Func<TcpClient, int, Server, BaseClient> clientFactory) {
    public Config Config { get; private set; } = new();
    private List<BaseClient> _clients = [];
    private readonly List<IPacketHandlerLayer> _handleLayers = [];
    private readonly List<IPacketPackageLayer> _packageLayers = [];

    public IReadOnlyList<IPacketHandlerLayer> HandleLayers => this._handleLayers.AsReadOnly();

    private readonly List<InboundPacket> _inboundPackets = [];

    private int _lastClientId = 0;

    public void AddPackageHandler(IPacketPackageLayer handler) {
        this._packageLayers.Add(handler);
        Logger.Debug($"Added new packet package layer with priority = {handler.GetPriority()}");
    }

    public void AddHandlerLayer(IPacketHandlerLayer handler) {
        this._handleLayers.Add(handler);
        Logger.Debug($"Added new packet handler layer with priority = {handler.GetPriority()}");
    }

    public void RegisterInboundPacket(InboundPacket packet) {
        if (this._inboundPackets.Contains(packet)) {
            Logger.Fatal($"Packet with id {packet.GetId()} already exists");
        }

        this._inboundPackets.Add(packet);
    }

    internal InboundPacket? GetInboundPacket(ushort id) {
        try {
            return this._inboundPackets.First(p => p.GetId() == id);
        }
        catch (InvalidOperationException ex) {
            Logger.Fatal($"Could not find packet with id {id}");
            return null;
        }
    }

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


        Task.Run(() => {
            TcpClient tcpClient = listener.AcceptTcpClient();
            int id = ++this._lastClientId;
            Logger.Info($"Client connected, id = {id}");
            BaseClient client = clientFactory(tcpClient, id, this);
            this._clients.Add(client);
        });
    }
}