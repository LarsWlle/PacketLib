using System.Net.Sockets;
using PacketLib.Packets;
using PacketLib.Utils;

namespace PacketLib;

public interface ISendableParticipant {
    public void Send(OutboundPacket packet);
}