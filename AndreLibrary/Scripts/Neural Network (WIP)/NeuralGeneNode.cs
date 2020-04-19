using System.Collections.Generic;

public class NeuralGeneNode
{
    public int nodeNumber { get; set; }
    public NeuralNodeType nodeType { get; set; }
    public List<NeuralGeneConnection> inputSynapses { get; set; }
    public List<NeuralGeneConnection> outputSynapses { get; set; }
    public double bias { get; set; }
    public double biasDelta { get; set; }
    public double error { get; set; }
    public double value { get; set; }
    public delegate double activationFunction(double x);
    activationFunction actFunction;
    public delegate double activationFunctionDerivative(double x);
    activationFunctionDerivative actFunctionDer;

    public NeuralGeneNode(NeuralActivationFunction _neuralActivationFunction, int _nodeNumber, NeuralNodeType _nodeType)
    {
        inputSynapses = new List<NeuralGeneConnection>();
        outputSynapses = new List<NeuralGeneConnection>();
        SetActivationFunction(_neuralActivationFunction);
        nodeNumber = _nodeNumber;
        nodeType = _nodeType;
        bias = NeatNeuralNetwork.GetRandom();
    }


    public void AddConnection(NeuralGeneNode _inputNeuron, NeuralGeneConnection _synapse, bool _connectionIsEnabled, int _innovation)
    {
        bias = NeatNeuralNetwork.GetRandom();
        _inputNeuron.outputSynapses.Add(_synapse);
        inputSynapses.Add(_synapse);
    }

    public void AddConnection(NeuralGeneNode _inputNeuron, NeuralGeneConnection _synapse, float _bias, bool _connectionIsEnabled, int _innovation)
    {
        bias = _bias;
        _inputNeuron.outputSynapses.Add(_synapse);
        inputSynapses.Add(_synapse);
    }

    public NeuralGeneConnection FindConnection(NeuralGeneConnection _synapse)
    {
        return inputSynapses.Find(x => x == _synapse);
    }

    public bool HasConnection(NeuralGeneConnection _synapse)
    {
        return inputSynapses.Contains(_synapse);
    }

    public bool HasInput(NeuralGeneNode _node)
    {
        foreach (var input in inputSynapses)
        {
            if (input.inputNeuron == _node)
                return true;
        }
        return false;
    }


    public bool HasOutput(NeuralGeneNode _node)
    {
        foreach (var output in outputSynapses)
        {
            if (output.outputNeuron == _node)
                return true;
        }
        return false;
    }

    public void SetActivationFunction(NeuralActivationFunction _neuralActivationFunction)
    {
        switch (_neuralActivationFunction)
        {
            case NeuralActivationFunction.Sigmoid:
                actFunction = ActivationFunctions.Sigmoid;
                actFunctionDer = ActivationFunctions.SigmoidDer;
                break;
            case NeuralActivationFunction.Tanh:
                actFunction = ActivationFunctions.Tanh;
                actFunctionDer = ActivationFunctions.TanhDer;
                break;
            case NeuralActivationFunction.Relu:
                actFunction = ActivationFunctions.Relu;
                actFunctionDer = ActivationFunctions.ReluDer;
                break;
            case NeuralActivationFunction.LeakyRelu:
                actFunction = ActivationFunctions.LeakyRelu;
                actFunctionDer = ActivationFunctions.LeakyReluDer;
                break;
            default:
                break;
        }
    }

    public virtual double CalculateValue()
    {
        double Sum = 0;
        foreach (var synapse in inputSynapses)
        {
            if (synapse.connectionIsEnabled)
                Sum += synapse.weight * synapse.inputNeuron.value;
        }
        return value = actFunction(Sum + bias);
    }

    public double CalculateError(double _target)
    {
        return _target - value;
    }

    public double CalculateGradient(double _target = -1)
    {
        if (_target == -1)
        {
            double Sum = 0;
            foreach (var synapse in outputSynapses)
            {
                if (synapse.connectionIsEnabled)
                    Sum += synapse.outputNeuron.error * synapse.weight;
            }
            return error = Sum * actFunctionDer(value);
        }

        return error = CalculateError(_target) * actFunctionDer(value);
    }

    public void UpdateWeights(double _learnRate, double _momentum)
    {
        var prevDelta = biasDelta;
        biasDelta = _learnRate * error;
        bias += biasDelta + _momentum * prevDelta;

        foreach (var synapse in inputSynapses)
        {
            prevDelta = synapse.weightDelta;
            synapse.weightDelta = _learnRate * error * synapse.inputNeuron.value;
            synapse.weight += synapse.weightDelta + _momentum * prevDelta;
        }
    }
}