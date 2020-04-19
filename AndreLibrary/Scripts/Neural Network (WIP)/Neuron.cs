using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum NeuralActivationFunction
{
    Sigmoid,
    Tanh,
    Relu,
    LeakyRelu
}

public class Neuron 
{
    public List<Synapse> inputSynapses { get; set; }
    public List<Synapse> outputSynapses { get; set; }
    public double bias { get; set; }
    public double biasDelta { get; set; }
    public double error { get; set; }
    public double value { get; set; }
    public delegate double activationFunction(double x);
    activationFunction actFunction;
    public delegate double activationFunctionDerivative(double x);
    activationFunctionDerivative actFunctionDer;

    public Neuron(NeuralActivationFunction _neuralActivationFunction)
    {
        inputSynapses = new List<Synapse>();
        outputSynapses = new List<Synapse>();
        SetActivationFunction(_neuralActivationFunction);
        bias = NeuralNetwork.GetRandom();
    }

    public Neuron(NeuralActivationFunction _neuralActivationFunction, IEnumerable<Neuron> _inputNeurons)
    {
        inputSynapses = new List<Synapse>();
        outputSynapses = new List<Synapse>();
        SetActivationFunction(_neuralActivationFunction);
        bias = NeuralNetwork.GetRandom();

        foreach (var inputNeuron in _inputNeurons)
        {
            var synapse = new Synapse(inputNeuron, this);
            inputNeuron.outputSynapses.Add(synapse);
            inputSynapses.Add(synapse);
        }
    }

    public  void AddNewConnection(IEnumerable<Neuron> _inputNeurons)
    {
        bias = NeuralNetwork.GetRandom();

        foreach (var inputNeuron in _inputNeurons)
        {
            var synapse = new Synapse(inputNeuron, this);
            inputNeuron.outputSynapses.Add(synapse);
            inputSynapses.Add(synapse);
        }
    }

    public void AddConnection(IEnumerable<Neuron> _inputNeurons, float _bias)
    {
        bias = _bias;
        foreach (var inputNeuron in _inputNeurons)
        {
            var synapse = new Synapse(inputNeuron, this);
            inputNeuron.outputSynapses.Add(synapse);
            inputSynapses.Add(synapse);
        }
    }

    public void SetActivationFunction(NeuralActivationFunction _neuralActivationFunction)
    {
        switch (_neuralActivationFunction)
        {
            case NeuralActivationFunction.Sigmoid:
                actFunction = ActivationFunctions.Sigmoid;
                actFunctionDer= ActivationFunctions.SigmoidDer;
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
        return value = actFunction(inputSynapses.Sum(a => a.weight * a.inputNeuron.value) + bias);
    }

    public double CalculateError(double _target)
    {
        return _target - value;
    }

    public double CalculateGradient(double _target = -1)
    {
        if (_target == -1)
            return error = outputSynapses.Sum(a => a.outputNeuron.error * a.weight) * actFunctionDer(value);

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
