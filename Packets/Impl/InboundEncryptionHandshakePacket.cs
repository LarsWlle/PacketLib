namespace PacketLib.Packets.Impl;

public class InboundEncryptionHandshakePacket : InboundPacket {
    public override ushort GetId() => ushort.MaxValue;

    public override void Handle(byte[] data, AbstractClient client) {
        if (data.Length != 32) {
            Logger.Warn("The received public key of the remote has an invalid length! Terminating connection");
            client.Terminate();
            return;
        }

        if (data.All(b => b == 0)) {
            Logger.Warn("The received public key of the remote only contains zeros! Terminating connection");
            client.Terminate();
            return;
        }

        client.Encryption.SetRemotePublicKey(data);
        client.Encryption.IsHandshakeComplete = true;
    }
}