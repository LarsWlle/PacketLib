using System.Net.Sockets;

namespace PacketLib;

public class CommunicationClient : CommunicationParticipant {
    private TcpClient _client;

    public void Connect(string endpoint, int port) {
        this._client = new(endpoint, port);
    }
}