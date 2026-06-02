using System.Net;
using System.Net.Sockets;
using PacketLib.Clients;
using PacketLib.Configuration;

namespace PacketLib;

public class Server(Func<TcpClient, BaseClient> clientFactory) {
    public Config Config { get; private set; } = new();
    private List<BaseClient> _clients = [];
    private List<IPacketHandlerLayer> _handleLayers = [];
    private List<IPacketPackageLayer> _packageLayers = [];

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
            BaseClient client = clientFactory(tcpClient);
            this._clients.Add(client);
        });
    }
}