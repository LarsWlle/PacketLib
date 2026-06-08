#region

using System.Net.Sockets;
using PacketLib.Crypto;
using PacketLib.Layering;
using PacketLib.Packets;
using PacketLib.Packets.Impl;
using PacketLib.Utils;

#endregion

namespace PacketLib;

/// <summary>
///     A class that handles read/write of a network buffer,
///     based on an already created tcp client.
///     Also contains extra methods that manage this tcp connection.
/// </summary>
public abstract class AbstractClient {
    private readonly TcpClient _client;
    private readonly LayerPipeline _handleLayers;
    private readonly LayerPipeline _packageLayers;
    private readonly PacketList _packetList;
    private readonly int _maxReadBufferLength;

    public Encryption Encryption { get; private set; } = new();

    protected AbstractClient(
        TcpClient client,
        LayerPipeline handleLayers,
        LayerPipeline packageLayers,
        PacketList inboundPackets,
        int maxReadBufferLength
    ) {
        this._client = client;
        this._handleLayers = handleLayers;
        this._packageLayers = packageLayers;
        this._packetList = inboundPackets;
        this._maxReadBufferLength = maxReadBufferLength;
        Thread thread = new(this.HandleIncomingTraffic);
        thread.Start();
    }

    private void HandleIncomingTraffic() {
        NetworkStream stream = this._client.GetStream();
        Logger.Debug("Starting listener on new thread");

        while (this._client.Connected) {
            try {
                if (!stream.CanRead || !stream.DataAvailable) {
                    Thread.Sleep(10);
                    continue;
                }

                byte[] buffer = new byte[this._maxReadBufferLength];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);

                if (bytesRead == 0) {
                    Logger.Warn("Data was available but read 0 bytes, something is going on!");
                    continue;
                }

                ReadOnlySpan<byte> span = buffer.AsSpan(0, bytesRead);
                byte[] transformed = this._handleLayers.Perform(span.ToArray(), this);

                int packetLength = transformed.ExtractInt(0);

                if (packetLength != transformed.Length) {
                    Logger.Warn($"Packet length {packetLength} != {transformed.Length}, discarding packet!");
                    continue;
                }

                ushort packetId = transformed.ExtractUShort(4);
                InboundPacket? packet = this._packetList.Get(packetId);
                Logger.Info($"Received packet with {packetId}, found a valid handler!");
                packet?.Handle(transformed, this);
            }
            catch (IOException ex) {
                Logger.Warn(ex.Message);
                break;
            }
        }
    }

    public void Send(OutboundPacket packet) {
        NetworkStream stream = this._client.GetStream();
        byte[] data = packet.Package();
        byte[] result = this._packageLayers.Perform(data, this);

        byte[] final = [
            ..(result.Length + 6).ToByteArray(), // + 6 = + 4 (length) + 2 (packet id)
            ..packet.GetId().ToByteArray(),
            ..result
        ];

        stream.Write(final, 0, final.Length);
    }

    public void Terminate() {
        NetworkStream stream = this._client.GetStream();
        stream.Flush();
        stream.Close();
        this._client.Close(); // Should close the thread
    }

    public void StartEncryptionHandshake() {
        this.Send(new OutboundEncryptionHandshakePacket(this));
    }
}