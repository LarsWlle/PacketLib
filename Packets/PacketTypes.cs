namespace PacketLib.Packets;

public abstract class Packet {
    public abstract ushort GetId();

    public override bool Equals(object? obj) {
        if (obj is not Packet other) return false;

        if (ReferenceEquals(this, obj)) return true;

        return this.GetId() == other.GetId() && this.GetType() == other.GetType();
    }

    public override int GetHashCode() {
        return HashCode.Combine(this.GetId());
    }
}

public abstract class OutboundPacket : Packet {
    public abstract byte[] Package();
}

public abstract class InboundPacket : Packet {
    public abstract void Handle(byte[] data);
}