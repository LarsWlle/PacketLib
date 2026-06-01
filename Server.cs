using PacketLib.Configuration;

namespace PacketLib;

public class Server {
    public Config Config { get; private set; } = new();

    public Server() {
        this.Start();
    }

    private void Start() {
        Console.WriteLine(this.Config.Port);
    }
}