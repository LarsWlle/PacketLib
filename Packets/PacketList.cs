namespace PacketLib.Packets;

public class PacketList {
    private readonly Dictionary<int, InboundPacket> _packets = [];

    public PacketList Add(InboundPacket packet) {
        if (this._packets.ContainsKey(packet.GetId())) {
            Logger.Fatal($"Packet with id {packet.GetId()} already exists");
        }

        this._packets[packet.GetId()] = packet;
        Logger.Debug($"Registered new packet[id={packet.GetId()},name\"{packet.GetType().Name}\"]");

        return this;
    }

    public bool Contains(int id) {
        return this._packets.ContainsKey(id);
    }

    public InboundPacket? Get(int id) {
        return this._packets[id];
    }
}