using System.Net;
using System.Net.Sockets;
using PacketLib.Configuration;

namespace PacketLib;

public class Server {
    public Config Config { get; private set; } = new();

    public Server() { }

    public void Start() {
        TcpListener listener = new(IPAddress.Any, this.Config.Port) {
            Server = {
                ReceiveTimeout = this.Config.ReceiveTimeout,
                SendTimeout = this.Config.SendTimeout,
            }
        };
        
        listener.Start();

        Task.Run(() => {
            TcpClient client = listener.AcceptTcpClient();
        });
    }
}