#region

using System.Net.Sockets;
using PacketLib.Layering;
using PacketLib.Packets;

#endregion

namespace PacketLib.Client;

public class CommunicationClient(
    string endpoint,
    int port,
    LayerPipeline handleLayers,
    LayerPipeline packageLayers,
    PacketList inboundPackets
) : AbstractClient(new TcpClient(endpoint, port), handleLayers, packageLayers, inboundPackets);