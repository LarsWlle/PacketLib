using PacketLib.Clients;

namespace PacketLib.Layering;

public class LayerPipeline {
    private readonly List<INetworkLayer> _layers = [];

    public LayerPipeline Then(INetworkLayer layer) {
        this._layers.Add(layer);
        return this;
    }

    public byte[] Perform(byte[] arr) => this._layers.Aggregate(arr, (current, layer) => layer.Handle(current));
}