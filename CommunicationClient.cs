using System.Net.Sockets;
using PacketLib.Clients;
using PacketLib.Packets;
using PacketLib.Utils;

namespace PacketLib;

public class CommunicationClient : CommunicationParticipant, ISendableParticipant {
    private TcpClient _client;

    public void Connect(string endpoint, int port) {
        this._client = new TcpClient(endpoint, port);

        Task.Run(this.HandleIncomingTraffic);
    }


    private void HandleIncomingTraffic() {
        NetworkStream stream = this._client.GetStream();

        while (this._client.Connected) {
            if (!stream.CanRead || !stream.DataAvailable) {
                Thread.Sleep(10);
                continue;
            }

            byte[] buffer = new byte[2048];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);

            if (bytesRead == 0) {
                Logger.Warn("Data was available but read 0 bytes, something is going on!");
                continue;
            }

            ReadOnlySpan<byte> span = buffer.AsSpan(0, bytesRead);
            byte[] transformed = this.PerformLayerActions(this.HandleLayers, span.ToArray());

            int packetLength = transformed.ExtractInt(0);

            if (packetLength != transformed.Length) {
                Logger.Warn($"Packet length {packetLength} != {transformed.Length}, discarding packet!");
                continue;
            }

            ushort packetId = transformed.ExtractUShort(4);
            InboundPacket? packet = this.GetInboundPacket(packetId);
            packet?.Handle(transformed, this);
        }
    }


    private byte[] PerformLayerActions(IReadOnlyList<INetworkLayer> layers, byte[] data) {
        IReadOnlyList<INetworkLayer> sorted = layers.OrderBy(p => p.GetPriority()).ToList();

        return sorted.Aggregate(data, (current, layer) => layer.Handle(current, this));
    }

    public void Send(OutboundPacket packet) {
        NetworkStream stream = this._client.GetStream();
        byte[] data = packet.Package();
        byte[] result = this.PerformLayerActions(this.PackageLayers, data);

        byte[] final = [
            ..(result.Length + 6).ToByteArray(), // + 6 = + 4 (length) + 2 (packet id)
            ..packet.GetId().ToByteArray(),
            ..result
        ];

        stream.Write(final, 0, final.Length);
    }
}