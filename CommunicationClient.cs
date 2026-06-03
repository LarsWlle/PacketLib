using System.Net.Sockets;
using PacketLib.Clients;
using PacketLib.Packets;
using PacketLib.Utils;

namespace PacketLib;

public class CommunicationClient : CommunicationParticipant {
    private TcpClient _client;

    public void Connect(string endpoint, int port) {
        this._client = new(endpoint, port);
    }

    public void Send(OutboundPacket packet) {
        NetworkStream stream = this._client.GetStream();
        byte[] data = packet.Package();

        IReadOnlyList<INetworkLayer> sorted = this.PackageLayers.OrderBy(p => p.GetPriority()).ToList();
        byte[] result = sorted.Aggregate(data, (current, layer) => layer.Handle(current));

        byte[] final = [
            ..(result.Length + 6).ToByteArray(), // + 6 = + 4 (length) + 2 (packet id)
            ..packet.GetId().ToByteArray(),
            ..result
        ];

        stream.Write(final, 0, final.Length);
    }
}