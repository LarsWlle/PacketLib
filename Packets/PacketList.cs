namespace PacketLib.Packets;

public class PacketList {
    private Dictionary<int, InboundPacket> _packets = [];

    public void Add(InboundPacket packet) {
        if (this._packets.ContainsKey(packet.GetId())) {
            Logger.Fatal($"Packet with id {packet.GetId()} already exists");
        }

        this._packets[packet.GetId()] = packet;
        Logger.Debug($"Registered new packet[id={packet.GetId()},name\"{packet.GetType().Name}\"]");
    }

    public InboundPacket? Get(int id) {
        return this._packets[id];
    }
}