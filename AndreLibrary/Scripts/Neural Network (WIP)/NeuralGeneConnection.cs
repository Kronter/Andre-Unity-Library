
public enum NeuralMutationRates
{
    connections,
    bias,
    node,
    enable,
    disable,
}

public enum NeuralNodeType
{
    Input,
    Output,
    Hidden
}

public class NeuralGeneConnection
{
    public bool connectionIsEnabled { get; set; }
    public int innovation { get; set; }
    public NeuralGeneNode inputNeuron { get; set; }
    public NeuralGeneNode outputNeuron { get; set; }
    public double weight { get; set; }
    public double weightDelta { get; set; }

    public NeuralGeneConnection(NeuralGeneNode _inputNeuron, NeuralGeneNode _outputNeuron, bool _connectionIsEnabled, int _innovation)
    {
        inputNeuron = _inputNeuron;
        outputNeuron = _outputNeuron;
        weight = NeatNeuralNetwork.GetRandom();
        connectionIsEnabled = _connectionIsEnabled;
        innovation = _innovation;
    }
}
