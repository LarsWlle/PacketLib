#region

using PacketLib.Crypto;

#endregion

namespace PacketLib.Layering.Impl.Inbounds;

public class EncryptionHandleLayer : INetworkLayer {
    public byte[] Handle(byte[] data, AbstractClient client) =>
        client.Encryption.KeyExchangeStatus == Encryption.HandshakeStatus.Both ? client.Encryption.Decrypt(data) : data;
}