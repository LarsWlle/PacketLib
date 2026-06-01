using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace PacketLib.Configuration;

public class Config {
    public int Port { get; private set; }
    public int MaxConnections { get; private set; }

    private const string Path = "settings.json";

    public Config() {
        this.CreateIfNotExists();

        IConfigurationRoot config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(Config.Path)
            .Build();

        this.Port = Convert.ToInt32(config["Server:Port"]);
        this.MaxConnections = Convert.ToInt32(config["MaxClients"]);
    }

    private void CreateIfNotExists() {
        if (File.Exists(Config.Path)) return;

        var defaults = new {
            Server = new { Port = 8080 },
            MaxClients = 100
        };

        File.WriteAllText(Config.Path, JsonSerializer.Serialize(defaults, new JsonSerializerOptions { WriteIndented = true }));
    }
}