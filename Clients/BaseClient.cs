using System.Net.Sockets;
using PacketLib.Configuration;

namespace PacketLib.Clients;

public abstract class BaseClient {
    public int Id { get; private init; }
    private readonly TcpClient _tcpClient;
    private Task _thread;
    private readonly Config _config;

    public BaseClient(TcpClient client, int id, Config config) {
        this.Id = id;
        this._tcpClient = client;
        this._config = config;

        this._thread = Task.Run(this.HandleIncomingTraffic);
    }

    private void HandleIncomingTraffic() {
        NetworkStream stream = this._tcpClient.GetStream();

        while (this._tcpClient.Connected) {
            if (!stream.CanRead || !stream.DataAvailable) {
                Thread.Sleep(10);
                continue;
            }

            byte[] buffer = new byte[this._config.MaxReadWriteBuffer];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);

            if (bytesRead == 0) {
                Logger.Warn("Data was available but read 0 bytes, something is going on!");
                continue;
            }
        }
    }
}