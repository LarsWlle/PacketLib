namespace PacketLib;

public abstract partial class AbstractClient {
    public event Action? EncryptionHandshakeComplete;

    internal void TriggerEncryptionHandshakeComplete() {
        this.EncryptionHandshakeComplete?.Invoke();
    }
}