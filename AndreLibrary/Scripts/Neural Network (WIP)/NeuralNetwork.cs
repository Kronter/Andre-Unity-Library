using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NeuralNetwork
{

    public double learnRate { get; set; }
    public double momentum { get; set; }
    public List<Neuron> InputLayer { get; set; }
    public List<List<Neuron>> HiddenLayers { get; set; }
    public List<Neuron> OutputLayer { get; set; }

    private static readonly System.Random Random = new System.Random();

    public NeuralNetwork(NeuralActivationFunction[] _neuralActivationFunctions, int _inputSize, int _hiddenSize, int _outputSize, int _numHiddenLayers = 1, double _learnRate = -1, double _momentum = -1)
    {
        learnRate = _learnRate == -1 ? .1 : _learnRate;
        momentum = _momentum == -1 ? .4 : _momentum;
        InputLayer = new List<Neuron>();
        HiddenLayers = new List<List<Neuron>>();
        OutputLayer = new List<Neuron>();

        for (var i = 0; i < _inputSize; i++)
            InputLayer.Add(new Neuron(_neuralActivationFunctions[0]));

        for (int i = 0; i < _numHiddenLayers; i++)
        {
            HiddenLayers.Add(new List<Neuron>());
            for (var j = 0; j < _hiddenSize; j++)
                HiddenLayers[i].Add(new Neuron(_neuralActivationFunctions[_neuralActivationFunctions.Length > 1 ? (_numHiddenLayers > _neuralActivationFunctions.Length - 2?  1 : i + 1) : 0],
                    i == 0 ? InputLayer : HiddenLayers[i - 1]));
        }

        for (var i = 0; i < _outputSize; i++)
            OutputLayer.Add(new Neuron(_neuralActivationFunctions[_neuralActivationFunctions.Length > 1 ? _neuralActivationFunctions.Length-1 : 0], HiddenLayers[_numHiddenLayers - 1]));
    }

    public void Train(List<NeuralDataSet> dataSets, int numEpochs)
    {
        for (var i = 0; i < numEpochs; i++)
        {
            foreach (var dataSet in dataSets)
            {
                ForwardPropagate(dataSet.values);
                BackPropagate(dataSet.targets);
            }
        }
    }

    public void Train(List<NeuralDataSet> dataSets, double minimumError)
    {
        var error = 1.0;
        var numEpochs = 0;

        while (error > minimumError && numEpochs < int.MaxValue)
        {
            var errors = new List<double>();
            foreach (var dataSet in dataSets)
            {
                ForwardPropagate(dataSet.values);
                BackPropagate(dataSet.targets);
                errors.Add(CalculateError(dataSet.targets));
            }
            error = errors.Average();
            numEpochs++;
        }
    }

    private void ForwardPropagate(params double[] inputs)
    {
        var i = 0;
        InputLayer.ForEach(a => a.value = inputs[i++]);
        foreach (var layer in HiddenLayers)
            layer.ForEach(a => a.CalculateValue());
        OutputLayer.ForEach(a => a.CalculateValue());
    }

    private void BackPropagate(params double[] targets)
    {
        var i = 0;
        OutputLayer.ForEach(a => a.CalculateGradient(targets[i++]));
        foreach (var layer in HiddenLayers.AsEnumerable<List<Neuron>>().Reverse())
        {
            layer.ForEach(a => a.CalculateGradient());
            layer.ForEach(a => a.UpdateWeights(learnRate, momentum));
        }
        OutputLayer.ForEach(a => a.UpdateWeights(learnRate, momentum));
    }

    public double[] Compute(params double[] inputs)
    {
        ForwardPropagate(inputs);
        return OutputLayer.Select(a => a.value).ToArray();
    }

    private double CalculateError(params double[] targets)
    {
        var i = 0;
        return OutputLayer.Sum(a => Mathf.Abs((float)a.CalculateError(targets[i++])));
    }

    public static double GetRandom()
    {
        return 2 * Random.NextDouble() - 1;
    }
}

public enum TrainingType
{
    Epoch,
    MinimumError
}

