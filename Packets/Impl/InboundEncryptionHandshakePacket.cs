#region

using PacketLib.Crypto;

#endregion

namespace PacketLib.Packets.Impl;

internal class InboundEncryptionHandshakePacket : InboundPacket {
    public override ushort GetId() => ushort.MaxValue;

    public override void Handle(byte[] data, AbstractClient client) {
        if (data.Length != 91) {
            Logger.Warn("The received public key of the remote has an invalid length! Terminating connection");
            Logger.Warn($"Got {data.Length}, expected 91");
            client.Terminate();
            return;
        }

        if (data.All(b => b == 0)) {
            Logger.Warn("The received public key of the remote only contains zeros! Terminating connection");
            client.Terminate();
            return;
        }

        client.Encryption.SetRemotePublicKey(data);
        client.Encryption.KeyExchangeStatus |= Encryption.HandshakeStatus.Received;

        if ((client.Encryption.KeyExchangeStatus & Encryption.HandshakeStatus.Sent) == 0) {
            client.Send(new OutboundEncryptionHandshakePacket(client));
            client.Encryption.KeyExchangeStatus |= Encryption.HandshakeStatus.Sent;
        }
    }
}