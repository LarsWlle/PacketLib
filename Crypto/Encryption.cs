#region

using System.Security.Cryptography;

#endregion

namespace PacketLib.Crypto;

public class Encryption {
    [Flags]
    public enum HandshakeStatus {
        Nothing = 0,
        Sent = 1,
        Received = 2,
        Both = HandshakeStatus.Sent | HandshakeStatus.Received
    }

    public readonly byte[] PublicKey;
    public byte[] RemotePublicKey { get; set; } = [];

    public HandshakeStatus KeyExchangeStatus {
        get;
        set {
            field = value;

            if (value == HandshakeStatus.Both)
                this._client.TriggerEncryptionHandshakeComplete();
        }
    } = HandshakeStatus.Nothing;

    private readonly ECDiffieHellman _ecdh;
    private byte[] _sharedKey = [];
    private readonly AbstractClient _client;

    public Encryption(AbstractClient client) {
        (byte[] PublicKey, ECDiffieHellman Ecdh) key = this.GenerateKey();
        this.PublicKey = key.PublicKey;
        this._ecdh = key.Ecdh;
        this._client = client;
    }

    public void SetRemotePublicKey(byte[] publicKey) {
        this.RemotePublicKey = publicKey;
        this._sharedKey = this.DeriveSharedKey();
        Logger.Debug($"Shared key: [{string.Join(", ", this._sharedKey)}]");
    }

    private (byte[] PublicKey, ECDiffieHellman Ecdh) GenerateKey() {
        ECDiffieHellman ecdh = ECDiffieHellman.Create(ECCurve.NamedCurves.nistP256);
        return (ecdh.PublicKey.ExportSubjectPublicKeyInfo(), ecdh);
    }

    private byte[] DeriveSharedKey() {
        using ECDiffieHellman remotePubKey = ECDiffieHellman.Create();
        remotePubKey.ImportSubjectPublicKeyInfo(this.RemotePublicKey, out _);
        byte[] sharedSecret = this._ecdh.DeriveRawSecretAgreement(remotePubKey.PublicKey);

        return HKDF.DeriveKey(HashAlgorithmName.SHA256, sharedSecret, 32, salt: null, info: "tcp-aes-key"u8.ToArray());
    }

    public byte[] Encrypt(byte[] plaintext) {
        byte[] nonce = new byte[AesGcm.NonceByteSizes.MaxSize];
        RandomNumberGenerator.Fill(nonce);

        byte[] ciphertext = new byte[plaintext.Length];
        byte[] tag = new byte[AesGcm.TagByteSizes.MaxSize];

        using AesGcm aes = new(this._sharedKey, tag.Length);
        aes.Encrypt(nonce, plaintext, ciphertext, tag);

        // Layout: [12 nonce][16 tag][n ciphertext]
        byte[] result = new byte[nonce.Length + tag.Length + ciphertext.Length];
        nonce.CopyTo(result, 0);
        tag.CopyTo(result, nonce.Length);
        ciphertext.CopyTo(result, nonce.Length + tag.Length);
        return result;
    }

    public byte[] Decrypt(byte[] payload) {
        switch (payload.Length) {
            case 0: return payload;
            case < 28:
                Logger.Warn("Tried to decrypt payload but length is smaller than 28!");
                return payload;
        }

        byte[] nonce = payload.Take(12).ToArray();
        byte[] tag = payload.Skip(12).Take(16).ToArray();
        byte[] cipher = payload.Skip(28).ToArray();

        byte[] plaintext = new byte[cipher.Length];
        using AesGcm aes = new(this._sharedKey, tag.Length);
        aes.Decrypt(nonce, cipher, tag, plaintext);
        return plaintext;
    }
}