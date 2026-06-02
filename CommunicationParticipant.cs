using PacketLib.Clients;
using PacketLib.Packets;

namespace PacketLib;

public abstract class CommunicationParticipant {
    private readonly List<INetworkLayer> _handleLayers = [];
    private readonly List<INetworkLayer> _packageLayers = [];

    public IReadOnlyList<INetworkLayer> HandleLayers => this._handleLayers.AsReadOnly();
    public IReadOnlyList<INetworkLayer> PackageLayers => this._packageLayers.AsReadOnly();

    private readonly List<InboundPacket<BaseClient>> _inboundPackets = [];

    public void AddPackageHandler(INetworkLayer handler) {
        this._packageLayers.Add(handler);
        Logger.Debug($"Added new packet package layer with priority = {handler.GetPriority()}");
    }

    public void AddHandlerLayer(INetworkLayer handler) {
        this._handleLayers.Add(handler);
        Logger.Debug($"Added new packet handler layer with priority = {handler.GetPriority()}");
    }

    public void RegisterInboundPacket(InboundPacket<BaseClient> packet) {
        if (this._inboundPackets.Contains(packet)) {
            Logger.Fatal($"Packet with id {packet.GetId()} already exists");
        }

        this._inboundPackets.Add(packet);
    }

    internal InboundPacket<BaseClient>? GetInboundPacket(ushort id) {
        try {
            return this._inboundPackets.First(p => p.GetId() == id);
        }
        catch (InvalidOperationException ex) {
            Logger.Fatal($"Could not find packet with id {id}");
            return null;
        }
    }
}