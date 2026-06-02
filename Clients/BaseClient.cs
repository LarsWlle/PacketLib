using System.Net.Sockets;
using PacketLib.Configuration;
using PacketLib.Packets;
using PacketLib.Utils;

namespace PacketLib.Clients;

public abstract class BaseClient {
    public int Id { get; private init; }
    private readonly TcpClient _tcpClient;
    private Task _thread;
    private readonly Server _server;


    public BaseClient(TcpClient client, int id, Server server) {
        this.Id = id;
        this._tcpClient = client;
        this._server = server;

        this._thread = Task.Run(this.HandleIncomingTraffic);
    }

    private void HandleIncomingTraffic() {
        NetworkStream stream = this._tcpClient.GetStream();

        while (this._tcpClient.Connected) {
            if (!stream.CanRead || !stream.DataAvailable) {
                Thread.Sleep(10);
                continue;
            }

            byte[] buffer = new byte[this._server.Config.MaxReadWriteBuffer];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);

            if (bytesRead == 0) {
                Logger.Warn("Data was available but read 0 bytes, something is going on!");
                continue;
            }

            ReadOnlySpan<byte> span = buffer.AsSpan(0, bytesRead);
            byte[] transformed = this.HandleLayers(span.ToArray());

            int packetLength = transformed.ExtractInt(0);

            if (packetLength != transformed.Length) {
                Logger.Warn($"Packet length {packetLength} != {transformed.Length}, discarding packet!");
                continue;
            }

            ushort packetId = transformed.ExtractUShort(4);
            InboundPacket? packet = this._server.GetInboundPacket(packetId);
            packet.Handle(transformed);
        }
    }

    private byte[] HandleLayers(byte[] data) {
        IReadOnlyList<IPacketHandlerLayer> layers = this._server.HandleLayers.OrderBy(p => p.GetPriority()).ToList();

        return layers.Aggregate(data, (current, layer) => layer.Handle(current));
    }
}