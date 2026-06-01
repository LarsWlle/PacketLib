using Microsoft.Extensions.Configuration;

namespace PacketLib.Configuration;

public class Config {
    public int Port { get; private set; }
    public int MaxConnections { get; private set; }

    public Config() {
        IConfigurationRoot config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("settings.json")
            .Build();

        this.Port = Convert.ToInt32(config["port"]);
        this.MaxConnections = Convert.ToInt32(config["max_connections"]);
    }
}