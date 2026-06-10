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
public abstract partial class AbstractClient {
    private readonly TcpClient _client;
    private readonly LayerPipeline _handleLayers;
    private readonly LayerPipeline _packageLayers;
    private readonly PacketList _packetList;

    private const int HeaderLength = 6; // 4 (length) + 2 (packet id)

    public Encryption Encryption { get; }

    protected AbstractClient(
        TcpClient client,
        LayerPipeline handleLayers,
        LayerPipeline packageLayers,
        PacketList inboundPackets
    ) {
        this._client = client;
        this._handleLayers = handleLayers;
        this._packageLayers = packageLayers;
        this._packetList = inboundPackets;
        this.Encryption = new Encryption(this);
        Task.Run(this.HandleIncomingTraffic);
    }

    private async Task HandleIncomingTraffic() {
        NetworkStream stream = this._client.GetStream();
        Logger.Debug("Starting listener on new thread");

        while (this._client.Connected) {
            try {
                byte[] header = new byte[AbstractClient.HeaderLength];
                await stream.ReadExactlyAsync(header, 0, header.Length);

                int length = header.ExtractInt(0);
                ushort packetId = header.ExtractUShort(4);

                byte[] payload = new byte[length];
                await stream.ReadExactlyAsync(payload, 0, payload.Length);

                byte[] transformed = this._handleLayers.Perform(payload, this);

                InboundPacket? packet = this._packetList.Get(packetId);
                Logger.Info($"Received packet with {packetId}, found a valid handler!");

                byte[] toPassOn = transformed
                    .Skip(AbstractClient.HeaderLength)
                    .Take(length - AbstractClient.HeaderLength)
                    .ToArray();

                packet?.Handle(toPassOn, this);
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
            ..result.Length.ToByteArray(),
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

    public void EnableEncryption() {
        this._packetList.Add(new InboundEncryptionHandshakePacket());
    }

    public void StartEncryptionHandshake() {
        InboundEncryptionHandshakePacket packet = new();
        if (!this._packetList.Contains(packet.GetId())) this._packetList.Add(packet);

        if ((this.Encryption.KeyExchangeStatus & Encryption.HandshakeStatus.Sent) != 0) return;

        this.Send(new OutboundEncryptionHandshakePacket(this));
        this.Encryption.KeyExchangeStatus |= Encryption.HandshakeStatus.Sent;
    }
}