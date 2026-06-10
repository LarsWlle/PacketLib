#region

using System.Net.Sockets;

#endregion

namespace PacketLib.Server;

public abstract class BaseClient : AbstractClient {
    public int Id { get; private init; }
    private readonly Server _server;


    protected BaseClient(TcpClient client, int id, Server server) : base(client, server.HandleLayers, server.PackageLayers, server.InboundPackets) {
        this.Id = id;
        this._server = server;
    }
}