using System.Net.Sockets;
using PacketLib.Layering;
using PacketLib.Packets;

namespace PacketLib.Client;

public class CommunicationClient(
    string endpoint,
    int port,
    LayerPipeline handleLayers,
    LayerPipeline packageLayers,
    PacketList inboundPackets,
    int maxReadBufferLength
) : AbstractClient(new TcpClient(endpoint, port), handleLayers, packageLayers, inboundPackets, maxReadBufferLength);