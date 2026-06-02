using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace PacketLib.Configuration;

public class Config {
    public int Port { get; private set; }
    public int MaxConnections { get; private set; }

    public int ReceiveTimeout { get; private set; }
    public int SendTimeout { get; private set; }

    public int MaxReadWriteBuffer { get; private set; }

    private const string Path = "settings.json";

    public Config() {
        this.CreateIfNotExists();

        IConfigurationRoot config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(Config.Path)
            .Build();

        this.Port = Convert.ToInt32(config["Server:Port"]);
        this.MaxConnections = Convert.ToInt32(config["MaxClients"]);

        this.ReceiveTimeout = Convert.ToInt32(config["Timeouts:ReceiveTimeout"]);
        this.SendTimeout = Convert.ToInt32(config["Timeouts:SendTimeout"]);
        this.MaxReadWriteBuffer = Convert.ToInt32(config["MaxReadWriteBuffer"]);
    }

    private void CreateIfNotExists() {
        if (File.Exists(Config.Path)) return;

        var defaults = new {
            Server = new { Port = 8080 },
            MaxClients = 100,
            Timeouts = new {
                SendTimeout = 10000,
                ReceiveTimeout = 10000,
            },
            MaxReadWriteBuffer = 2048
        };

        File.WriteAllText(Config.Path, JsonSerializer.Serialize(defaults, new JsonSerializerOptions { WriteIndented = true }));
    }
}