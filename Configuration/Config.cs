using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace PacketLib.Configuration;

public class Config {
    public int Port { get; private set; }
    public int MaxConnections { get; private set; }

    private const string PATH = "settings.json";

    public Config() {
        this.CreateIfNotExists();

        IConfigurationRoot config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(Config.PATH)
            .Build();

        this.Port = Convert.ToInt32(config["Server:Port"]);
        this.MaxConnections = Convert.ToInt32(config["MaxClients"]);
    }

    private void CreateIfNotExists() {
        if (File.Exists(Config.PATH)) return;

        var defaults = new {
            Server = new { Port = 8080 },
            MaxClients = 100
        };

        File.WriteAllText(Config.PATH, JsonSerializer.Serialize(defaults, new JsonSerializerOptions { WriteIndented = true }));
    }
}