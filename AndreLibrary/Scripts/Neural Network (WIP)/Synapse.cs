
public class Synapse 
{
    public Neuron inputNeuron { get; set; }
    public Neuron outputNeuron { get; set; }
    public double weight { get; set; }
    public double weightDelta { get; set; }

    public Synapse(Neuron _inputNeuron, Neuron _outputNeuron)
    {
        inputNeuron = _inputNeuron;
        outputNeuron = _outputNeuron;
        weight = NeuralNetwork.GetRandom();
    }

}
