#region

using System.Security.Cryptography;

#endregion

namespace PacketLib.Encryption;

public static class Encryption {
    public static (byte[] PublicKey, ECDiffieHellman Ecdh) GenerateKey() {
        ECDiffieHellman ecdh = ECDiffieHellman.Create(ECCurve.NamedCurves.nistP256);
        return (ecdh.PublicKey.ExportSubjectPublicKeyInfo(), ecdh);
    }

    public static byte[] DeriveSharedKey(ECDiffieHellman localEcdh, byte[] remotePubKeyBytes) {
        using ECDiffieHellman remotePubKey = ECDiffieHellman.Create();
        remotePubKey.ImportSubjectPublicKeyInfo(remotePubKeyBytes, out _);
        byte[] sharedSecret = localEcdh.DeriveRawSecretAgreement(remotePubKey.PublicKey);

        return HKDF.DeriveKey(HashAlgorithmName.SHA256, sharedSecret, 32, salt: null, info: "tcp-aes-key"u8.ToArray());
    }

    public static byte[] Encrypt(byte[] key, byte[] plaintext) {
        byte[] nonce = new byte[AesGcm.NonceByteSizes.MaxSize]; // 12 bytes
        RandomNumberGenerator.Fill(nonce);

        byte[] ciphertext = new byte[plaintext.Length];
        byte[] tag = new byte[AesGcm.TagByteSizes.MaxSize]; // 16 bytes

        using AesGcm aes = new AesGcm(key, tag.Length);
        aes.Encrypt(nonce, plaintext, ciphertext, tag);

        // Layout: [12 nonce][16 tag][n ciphertext]
        byte[] result = new byte[nonce.Length + tag.Length + ciphertext.Length];
        nonce.CopyTo(result, 0);
        tag.CopyTo(result, nonce.Length);
        ciphertext.CopyTo(result, nonce.Length + tag.Length);
        return result;
    }

    public static byte[] Decrypt(byte[] key, byte[] payload) {
        byte[] nonce = payload[..12];
        byte[] tag = payload[12..28];
        byte[] ciphertext = payload[28..];

        byte[] plaintext = new byte[ciphertext.Length];
        using AesGcm aes = new(key, tag.Length);
        aes.Decrypt(nonce, ciphertext, tag, plaintext);
        return plaintext;
    }
}