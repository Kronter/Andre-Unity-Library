using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NeatNeuralNetwork
{
    public NeuralGenome genome { get; set; }
    private static readonly System.Random Random = new System.Random();
    NeuralActivationFunction[] neuralActivationFunctions;

    public NeatNeuralNetwork(NeuralGenome _genome)
    {
        genome = _genome;
        neuralActivationFunctions = _genome.neuralActivationFunctions;
    }

    public NeatNeuralNetwork(NeuralActivationFunction[] _neuralActivationFunctions, int _inputSize, int _outputSize, int _globalInnovation, bool _firstGenome,
        Func<int, float> fitnessFunction, float[] _mutationRates, Func<int,int> _increaseGlobalInnovation, double _learnRate = -1, double _momentum = -1)
    {
        genome = new NeuralGenome();
        neuralActivationFunctions = _neuralActivationFunctions;
        genome.learnRate = _learnRate == -1 ? .1 : _learnRate;
        genome.momentum = _momentum == -1 ? .4 : _momentum;
        genome.random = Random;
        genome.InputLayer = new List<NeuralGeneNode>();
        genome.HiddenLayers = new List<NeuralGeneNode>();
        genome.OutputLayer = new List<NeuralGeneNode>();
        genome.nodes = new List<NeuralGeneNode>();
        genome.connections = new List<NeuralGeneConnection>();
        genome.mutationRates = _mutationRates;
        genome.fitnessFunction = fitnessFunction;
        genome.InitNewGenome(_inputSize, _outputSize, _globalInnovation, _neuralActivationFunctions, _increaseGlobalInnovation, _firstGenome);
    }

    public NeatNeuralNetwork(NeuralActivationFunction[] _neuralActivationFunctions, int _inputSize, int _outputSize, double _learnRate = -1, double _momentum = -1)
    {
        genome = new NeuralGenome();
        neuralActivationFunctions = _neuralActivationFunctions;
        genome.learnRate = _learnRate == -1 ? .1 : _learnRate;
        genome.momentum = _momentum == -1 ? .4 : _momentum;
        genome.InputLayer = new List<NeuralGeneNode>();
        genome.HiddenLayers = new List<NeuralGeneNode>();
        genome.OutputLayer = new List<NeuralGeneNode>();
        genome.nodes = new List<NeuralGeneNode>();
        genome.connections = new List<NeuralGeneConnection>();
        genome.InitNewGenome(_inputSize, _outputSize, 0, _neuralActivationFunctions);
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
        genome.InputLayer.ForEach(a => a.value = inputs[i++]);
        genome.HiddenLayers.ForEach(a => a.CalculateValue());
        genome.OutputLayer.ForEach(a => a.CalculateValue());
    }

    private void BackPropagate(params double[] targets)
    {
        var i = 0;
        genome.OutputLayer.ForEach(a => a.CalculateGradient(targets[i++]));
        genome.HiddenLayers.AsEnumerable<NeuralGeneNode>().Reverse();
        genome.HiddenLayers.ForEach(a => a.CalculateGradient());
        genome.HiddenLayers.ForEach(a => a.UpdateWeights(genome.learnRate, genome.momentum));
        genome.OutputLayer.ForEach(a => a.UpdateWeights(genome.learnRate, genome.momentum));
        genome.HiddenLayers.AsEnumerable<NeuralGeneNode>().Reverse();
    }

    public double[] Compute(params double[] inputs)
    {
        ForwardPropagate(inputs);
        return genome.OutputLayer.Select(a => a.value).ToArray();
    }

    private double CalculateError(params double[] targets)
    {
        var i = 0;
        return genome.OutputLayer.Sum(a => Mathf.Abs((float)a.CalculateError(targets[i++])));
    }

    public static double GetRandom()
    {
        return 2 * Random.NextDouble() - 1;
    }

    public NeatNeuralNetwork Crossover(NeatNeuralNetwork otherParent)
    {
        NeuralGenome childGenome = new NeuralGenome();
        childGenome.learnRate = genome.learnRate;
        childGenome.momentum = genome.momentum;
        childGenome.random = Random;
        childGenome.InputLayer = new List<NeuralGeneNode>();
        childGenome.HiddenLayers = new List<NeuralGeneNode>();
        childGenome.OutputLayer = new List<NeuralGeneNode>();
        childGenome.nodes = new List<NeuralGeneNode>();
        childGenome.connections = new List<NeuralGeneConnection>();
        childGenome.mutationRates = genome.mutationRates;
        childGenome.fitnessFunction = genome.fitnessFunction;
        childGenome.InitNewGenome(genome.currInnovation, neuralActivationFunctions, genome.IncreaseGlobalInnovation, false);
        // no best parent
        int bestGenome = 0;
        bool takeNotOptimum = false;
        double randRate = Random.NextDouble();
        if (randRate < 0.01f)
            takeNotOptimum = true;
        // find parent with best fitness
        if (otherParent.genome.fitness > genome.fitness)
            // best parent is other parent
            bestGenome = 1;
        else if (otherParent.genome.fitness < genome.fitness)
            // best parent is thisparent
            bestGenome = 2;
        int index = 0;
        if (this.genome.connections.Count > otherParent.genome.connections.Count)
        {
            foreach (var thisParentConnection in this.genome.connections)
            {
                //foreach (var otherParentConnection in otherParent.genome.connections)
                //{
                if (index > otherParent.genome.connections.Count - 1)
                    HandleExtraNodes(thisParentConnection, null, childGenome, bestGenome, takeNotOptimum);
                else
                    CrossOverParents(thisParentConnection, otherParent.genome.connections[index], childGenome, bestGenome);
                //}
                index++;
            }
        }
        else
        {
            foreach (var otherParentConnection in otherParent.genome.connections)
            {
                //foreach (var thisParentConnection in this.genome.connections)
                //{
                if(index > this.genome.connections.Count-1)
                    HandleExtraNodes(null, otherParentConnection, childGenome, bestGenome, takeNotOptimum);
                else
                    CrossOverParents(this.genome.connections[index], otherParentConnection, childGenome, bestGenome);
                //}
                index++;
            }
        }

        NeatNeuralNetwork child = new NeatNeuralNetwork(childGenome);
        return child;
    }

    void CrossOverParents(NeuralGeneConnection thisParentConnection, NeuralGeneConnection otherParentConnection, NeuralGenome childGenome, int bestGenome)
    {
        if (thisParentConnection.innovation == otherParentConnection.innovation)
        {
            //choose 50/50
            int parent = Random.NextDouble() < 0.5f ? 1 : 2;
            //other parent
            if (parent == 1)
            {
                AddNodesAndConnectionToChild(otherParentConnection, childGenome);
            }
            else // this parent
            {
                AddNodesAndConnectionToChild(thisParentConnection, childGenome);
            }
        }
        else
        {
            if (bestGenome == 0)
            {
                //choose 50/50
                int parent = Random.NextDouble() < 0.5f ? 1 : 2;
                //other parent
                if (parent == 1)
                {
                    AddNodesAndConnectionToChild(otherParentConnection, childGenome);
                }
                else // this parent
                {
                    AddNodesAndConnectionToChild(thisParentConnection, childGenome);
                }
            }
            else if (bestGenome == 1)
            {
                // choose from other parent
                AddNodesAndConnectionToChild(otherParentConnection, childGenome);
            }
            else
            {
                // choose from this parent
                AddNodesAndConnectionToChild(thisParentConnection, childGenome);
            }
        }
    }

    void HandleExtraNodes(NeuralGeneConnection thisParentConnection, NeuralGeneConnection otherParentConnection, NeuralGenome childGenome, int bestGenome, bool takeNotOptimum)
    {
        if (bestGenome == 0)
        {
            //other parent
            if (otherParentConnection != null)
            {
                AddNodesAndConnectionToChild(otherParentConnection, childGenome);
            }
            else if (thisParentConnection != null) // this parent
            {
                AddNodesAndConnectionToChild(thisParentConnection, childGenome);
            }
        }
        else if (bestGenome == 1 && otherParentConnection != null)
        {
            // choose from other parent
            AddNodesAndConnectionToChild(otherParentConnection, childGenome);
        }
        else if (bestGenome == 2 && thisParentConnection != null)
        {
            // choose from this parent
            AddNodesAndConnectionToChild(thisParentConnection, childGenome);
        }
        else if (takeNotOptimum && thisParentConnection != null)
        {
            AddNodesAndConnectionToChild(thisParentConnection, childGenome);
        }
        else if (takeNotOptimum && otherParentConnection != null)
        {
            AddNodesAndConnectionToChild(otherParentConnection, childGenome);
        }
    }

    void AddNodesAndConnectionToChild(NeuralGeneConnection ParentGenome, NeuralGenome childGenome)
    {
        NeuralGeneNode tmpInputNode = null;
        NeuralGeneNode tmpOutputNode = null;
        NeuralGeneConnection tmpConnection = null;
        if (childGenome.HasNode(ParentGenome.inputNeuron.nodeNumber))
            tmpInputNode = childGenome.GetNode(ParentGenome.inputNeuron.nodeNumber);
        else
        {
            tmpInputNode = AddNodeToGenome(ParentGenome.inputNeuron.nodeType, childGenome);
            tmpInputNode.bias = ParentGenome.inputNeuron.bias;
            tmpInputNode.nodeNumber = ParentGenome.inputNeuron.nodeNumber;
        }
        if (childGenome.HasNode(ParentGenome.outputNeuron.nodeNumber))
            tmpOutputNode = childGenome.GetNode(ParentGenome.outputNeuron.nodeNumber);
        else
        {
            tmpOutputNode = AddNodeToGenome(ParentGenome.outputNeuron.nodeType, childGenome);
            tmpOutputNode.bias = ParentGenome.outputNeuron.bias;
            tmpOutputNode.nodeNumber = ParentGenome.outputNeuron.nodeNumber;
        }
        if(!childGenome.HasConnection(ParentGenome.inputNeuron.nodeNumber, ParentGenome.outputNeuron.nodeNumber))
            childGenome.AddConnection(tmpInputNode, tmpOutputNode, ParentGenome.connectionIsEnabled, ParentGenome.innovation);
    }

    NeuralGeneNode AddNodeToGenome(NeuralNodeType type, NeuralGenome genome)
    {
        switch (type)
        {
            case NeuralNodeType.Input:
                return genome.AddInputNode(neuralActivationFunctions[0]);
                break;
            case NeuralNodeType.Output:
                return genome.AddOutputNode(neuralActivationFunctions[2]);
                break;
            case NeuralNodeType.Hidden:
                return genome.AddHiddenNode(neuralActivationFunctions[1]);
                break;
            default:
                break;
        }
        return null;
    }
}
