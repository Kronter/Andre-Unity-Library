using System;
using System.Collections.Generic;
using UnityEngine;

public class NeuralGenome
{
    public List<NeuralGeneConnection> connections { get; set; }
    public List<NeuralGeneNode> nodes { get; set; }
    public List<NeuralGeneNode> InputLayer { get; set; }
    public List<NeuralGeneNode> HiddenLayers { get; set; }
    public List<NeuralGeneNode> OutputLayer { get; set; }
    public int currInnovation { get; set; }
    public int localInnovation { get; set; }
    public int numInputs { get; set; }
    public int numOutputs { get; set; }
    public int numNodes { get; set; }
    public float[] mutationRates = new float[7];
    public double globalRank { get; set; }
    public double fitness { get; set; }
    public double learnRate { get; set; }
    public double momentum { get; set; }
    public Func<int, float> fitnessFunction { get; set; }
    public Func<int,int>  IncreaseGlobalInnovation{ get; set; }
    public System.Random random { get; set; }
    public NeuralActivationFunction[] neuralActivationFunctions { get; set; }

    public void InitNewGenome(int _inputSize, int _outputSize, int _innovation, 
        NeuralActivationFunction[] _neuralActivationFunctions, Func<int, int> _increaseGlobalInnovation = null,
        bool _increaseInnovation = true)
    {
        neuralActivationFunctions = _neuralActivationFunctions;
        currInnovation = _innovation;
        AddInputsNode(_inputSize, _neuralActivationFunctions[0]);
        AddOutputsNode(_outputSize, _neuralActivationFunctions[2]);
        IncreaseGlobalInnovation = _increaseGlobalInnovation;
        localInnovation = 1;

        foreach (NeuralGeneNode outputNode in OutputLayer)
        {
            AddConnection(InputLayer, outputNode, true, _increaseInnovation);
        }
    }

    public void InitNewGenome(int _innovation,
      NeuralActivationFunction[] _neuralActivationFunctions, Func<int, int> _increaseGlobalInnovation = null,
      bool _increaseInnovation = true)
    {
        neuralActivationFunctions = _neuralActivationFunctions;
        currInnovation = _innovation;
        IncreaseGlobalInnovation = _increaseGlobalInnovation;
        localInnovation = 1;
    }

    public void AddInputsNode(int _inputSize, NeuralActivationFunction _neuralActivationFunctions)
    {
        if (InputLayer == null)
            InputLayer = new List<NeuralGeneNode>();
        if (nodes == null)
            nodes = new List<NeuralGeneNode>();
        for (int i = 0; i < _inputSize; i++)
        {
            NeuralGeneNode tmpNode = new NeuralGeneNode(_neuralActivationFunctions, i +1, NeuralNodeType.Input);
            InputLayer.Add(tmpNode);
            nodes.Add(tmpNode);
        }
    }

    public NeuralGeneNode AddInputNode(NeuralActivationFunction _neuralActivationFunctions)
    {
        if (InputLayer == null)
            InputLayer = new List<NeuralGeneNode>();
        if (nodes == null)
            nodes = new List<NeuralGeneNode>();
            NeuralGeneNode tmpNode = new NeuralGeneNode(_neuralActivationFunctions, nodes.Count + 1, NeuralNodeType.Input);
            InputLayer.Add(tmpNode);
            nodes.Add(tmpNode);
        return tmpNode;
    }

    public void AddOutputsNode(int _outputSize, NeuralActivationFunction _neuralActivationFunctions)
    {
        if (OutputLayer == null)
            OutputLayer = new List<NeuralGeneNode>();
        if (nodes == null)
            nodes = new List<NeuralGeneNode>();
        for (int i = 0; i < _outputSize; i++)
        {
            NeuralGeneNode tmpNode = new NeuralGeneNode(_neuralActivationFunctions, nodes.Count + 1, NeuralNodeType.Output);
            OutputLayer.Add(tmpNode);
            nodes.Add(tmpNode);
        }
    }

    public NeuralGeneNode AddOutputNode(NeuralActivationFunction _neuralActivationFunctions)
    {
        if (OutputLayer == null)
            OutputLayer = new List<NeuralGeneNode>();
        if (nodes == null)
            nodes = new List<NeuralGeneNode>();
        NeuralGeneNode tmpNode = new NeuralGeneNode(_neuralActivationFunctions, nodes.Count + 1, NeuralNodeType.Output);
        OutputLayer.Add(tmpNode);
        nodes.Add(tmpNode);
        return tmpNode;
    }

    public NeuralGeneNode AddHiddenNode(NeuralActivationFunction _neuralActivationFunctions)
    {
        if (HiddenLayers == null)
            HiddenLayers = new List<NeuralGeneNode>();
        if (nodes == null)
            nodes = new List<NeuralGeneNode>();
        NeuralGeneNode tmpNode = new NeuralGeneNode(_neuralActivationFunctions, nodes.Count +1, NeuralNodeType.Hidden);
        HiddenLayers.Add(tmpNode);
        nodes.Add(tmpNode);
        return tmpNode;
    }

    public void AddConnection(IEnumerable<NeuralGeneNode> _inputNeurons, NeuralGeneNode _outputNeuron, bool _connectionIsEnabled, 
        bool _increaseInnovation = true)
    {
        if (connections == null)
            connections = new List<NeuralGeneConnection>();
        foreach (var inputNeuron in _inputNeurons)
        {
            NeuralGeneConnection tmpSynapse = new NeuralGeneConnection(inputNeuron, _outputNeuron, _connectionIsEnabled, _increaseInnovation ? currInnovation : localInnovation);
            _outputNeuron.AddConnection(inputNeuron, tmpSynapse, _connectionIsEnabled, _increaseInnovation ? currInnovation : localInnovation);
            connections.Add(tmpSynapse);
            if (_increaseInnovation)
            {
                currInnovation++;
                if (IncreaseGlobalInnovation != null)
                    IncreaseGlobalInnovation(currInnovation);
            }
            else
                localInnovation++;
        }
    }

    public void AddConnection(IEnumerable<NeuralGeneNode> _inputNeurons, NeuralGeneNode _outputNeuron, float _bias, bool _connectionIsEnabled, 
        bool _increaseInnovation = true)
    {
        if (connections == null)
            connections = new List<NeuralGeneConnection>();
        foreach (var inputNeuron in _inputNeurons)
        {
            NeuralGeneConnection tmpSynapse = new NeuralGeneConnection(inputNeuron, _outputNeuron, _connectionIsEnabled, _increaseInnovation ? currInnovation : localInnovation);
            _outputNeuron.AddConnection(inputNeuron, tmpSynapse, _bias, _connectionIsEnabled, _increaseInnovation ? currInnovation : localInnovation);
            connections.Add(tmpSynapse);
            if (_increaseInnovation)
            {
                currInnovation++;
                if (IncreaseGlobalInnovation != null)
                    IncreaseGlobalInnovation(currInnovation);
            }
            else
                localInnovation++;

        }
    }

    public void AddConnection(NeuralGeneNode _inputNeuron, NeuralGeneNode _outputNeuron, bool _connectionIsEnabled, 
        bool _increaseInnovation = true)
    {
        if (connections == null)
            connections = new List<NeuralGeneConnection>();

        NeuralGeneConnection tmpSynapse = new NeuralGeneConnection(_inputNeuron, _outputNeuron, _connectionIsEnabled, _increaseInnovation ? currInnovation : localInnovation);
        _outputNeuron.AddConnection(_inputNeuron, tmpSynapse, _connectionIsEnabled, _increaseInnovation ? currInnovation : localInnovation);
        connections.Add(tmpSynapse);
        if (_increaseInnovation)
        {
            currInnovation++;
            if (IncreaseGlobalInnovation != null)
                IncreaseGlobalInnovation(currInnovation);
        }
        else
            localInnovation++;
    }

    public void AddConnection(NeuralGeneNode _inputNeuron, NeuralGeneNode _outputNeuron, float _bias, bool _connectionIsEnabled,
        bool _increaseInnovation = true)
    {
        if (connections == null)
            connections = new List<NeuralGeneConnection>();

        NeuralGeneConnection tmpSynapse = new NeuralGeneConnection(_inputNeuron, _outputNeuron, _connectionIsEnabled, _increaseInnovation ? currInnovation : localInnovation);
        _outputNeuron.AddConnection(_inputNeuron, tmpSynapse, _bias, _connectionIsEnabled, _increaseInnovation ? currInnovation : localInnovation);
        connections.Add(tmpSynapse);
        if (_increaseInnovation)
        {
            currInnovation++;
            if (IncreaseGlobalInnovation != null)
                IncreaseGlobalInnovation(currInnovation);
        }
        else
            localInnovation++;
    }

    public void AddConnection(NeuralGeneNode _inputNeuron, NeuralGeneNode _outputNeuron, bool _connectionIsEnabled,
          int _innovation)
    {
        if (connections == null)
            connections = new List<NeuralGeneConnection>();

        NeuralGeneConnection tmpSynapse = new NeuralGeneConnection(_inputNeuron, _outputNeuron, _connectionIsEnabled, _innovation);
        _outputNeuron.AddConnection(_inputNeuron, tmpSynapse, _connectionIsEnabled, _innovation);
        connections.Add(tmpSynapse);
        localInnovation++;
    }

    public bool HasConnection(NeuralGeneConnection _synapse)
    {
        return connections.Contains(_synapse);
    }

    public bool HasConnection(int _inputNodeNumber, int _outputNodeNumber)
    {
        if (connections == null)
            connections = new List<NeuralGeneConnection>();
        foreach (var connection in connections)
        {
            if (connection.inputNeuron.nodeNumber == _inputNodeNumber &&
                connection.outputNeuron.nodeNumber == _outputNodeNumber)
                return true;
        }

        return false;
    }

    public NeuralGeneConnection GetConnection(NeuralGeneConnection _synapse)
    {
        return connections.Find(x => x == _synapse);
    }

    public void ChangeConnectionStatus(NeuralGeneConnection _synapse, bool _connectionIsEnabled)
    {
        if(HasConnection(_synapse))
            GetConnection(_synapse).connectionIsEnabled = _connectionIsEnabled;
    }

    public bool HasNode(NeuralGeneNode _neuron)
    {
        return nodes.Contains(_neuron);
    }

    public bool HasNode(int _nodeNumber)
    {
        if (nodes == null)
            nodes = new List<NeuralGeneNode>();
        foreach (var node in nodes)
        {
            if (node.nodeNumber == _nodeNumber)
                return true;
        }

        return false;
    }

    public NeuralGeneNode GetNode(NeuralGeneNode _neuron)
    {
        return nodes.Find(x => x == _neuron);
    }

    public NeuralGeneNode GetNode(int _number)
    {
        return nodes.Find(x => x.nodeNumber == _number);
    }

    public float CalculateFitness(int index)
    {
        if(fitnessFunction !=null)
            fitness = fitnessFunction(index);
        return (float)fitness;
    }

    public void Mutate(float mutationRate)
    {
        double randRate = random.NextDouble();
        if (randRate < mutationRate)
        {
            //for (int i = 0; i < mutationRates.Length; i++)
            //{
                randRate = UnityEngine.Random.Range(0,5);
                //if (randRate < mutationRates[i])
                //{
                    switch (randRate)
                    {
                        case (int)NeuralMutationRates.connections:
                            MutateConnections();
                           return;
                        case (int)NeuralMutationRates.bias:
                            MutateBias();
                            return;
                        case (int)NeuralMutationRates.node:
                            MutateNodes();
                            return;
                        case (int)NeuralMutationRates.enable:
                            MutateConnectionStatus(true);
                            return;
                        case (int)NeuralMutationRates.disable:
                            MutateConnectionStatus(false);
                            return;
                        default:
                            break;
                    }
            //    }
            //}
        }
    }

    public void MutateConnections()
    {
        List<NeuralGeneNode> tmpNodesOutputList = new List<NeuralGeneNode>();
        List<NeuralGeneNode> tmpNodesInputList = new List<NeuralGeneNode>();

        foreach (NeuralGeneNode node in InputLayer)
        {
            tmpNodesInputList.Add(node);
        }

        foreach (NeuralGeneNode node in HiddenLayers)
        {
            tmpNodesOutputList.Add(node);
            tmpNodesInputList.Add(node);
        }

        foreach (NeuralGeneNode node in OutputLayer)
        {
            tmpNodesOutputList.Add(node);
        }

        int nodeOutputIndex= UnityEngine.Random.Range(0, tmpNodesOutputList.Count);
        int breakLoop = 0;
        int nodeInputIndex = UnityEngine.Random.Range(0, tmpNodesInputList.Count);
        while (true)
        {
            if (tmpNodesInputList[nodeInputIndex] != tmpNodesOutputList[nodeOutputIndex])
            {
                if (!tmpNodesOutputList[nodeOutputIndex].HasInput(tmpNodesInputList[nodeInputIndex]))
                    break;
            }

            nodeInputIndex = UnityEngine.Random.Range(0, tmpNodesInputList.Count);
            breakLoop++;
            if (breakLoop >= 100000)
                return;
        }

        AddConnection(tmpNodesInputList[nodeInputIndex], tmpNodesOutputList[nodeOutputIndex], true);
    }

    public void MutateBias()
    {
        int nodeIndex = UnityEngine.Random.Range(0, nodes.Count);
        if (UnityEngine.Random.Range(0, 1) == 1)
            nodes[nodeIndex].bias += (learnRate * 4) * - 1;
        else
            nodes[nodeIndex].bias += (learnRate * 4) * 1;
    }

    public void MutateNodes()
    {
        List<NeuralGeneNode> tmpNodesNoinputsList = new List<NeuralGeneNode>();

        foreach (NeuralGeneNode node in HiddenLayers)
        {
            tmpNodesNoinputsList.Add(node);
        }

        foreach (NeuralGeneNode node in OutputLayer)
        {
            tmpNodesNoinputsList.Add(node);
        }

        NeuralGeneNode tmpHiddenNode = AddHiddenNode(neuralActivationFunctions[1]);

        List<NeuralGeneConnection> possibleConnectionsList = new List<NeuralGeneConnection>();

        foreach (var node in tmpNodesNoinputsList)
        {
            foreach (var connection in node.inputSynapses)
            {
                if ((connection.outputNeuron.nodeNumber > tmpHiddenNode.nodeNumber || OutputLayer.Contains(connection.outputNeuron)) && connection.connectionIsEnabled)
                    possibleConnectionsList.Add(connection);
            }
        }

        int nodeConnectionIndex = UnityEngine.Random.Range(0, possibleConnectionsList.Count);
        possibleConnectionsList[nodeConnectionIndex].connectionIsEnabled = false;

        AddConnection(possibleConnectionsList[nodeConnectionIndex].inputNeuron, tmpHiddenNode, true);
        AddConnection(tmpHiddenNode, possibleConnectionsList[nodeConnectionIndex].outputNeuron, true);
    }

    public void MutateConnectionStatus(bool _status)
    {
        List<NeuralGeneConnection> tmpConnectionsList = new List<NeuralGeneConnection>();

        foreach (NeuralGeneConnection connection in connections)
        {
            if (connection.connectionIsEnabled != _status)
            {
                if(connection.inputNeuron.outputSynapses.Count > 1)
                    tmpConnectionsList.Add(connection);
            }
        }

        if (tmpConnectionsList.Count > 0)
        {
            int connectionIndex = UnityEngine.Random.Range(0, tmpConnectionsList.Count);
            tmpConnectionsList[connectionIndex].connectionIsEnabled = _status;
        }
    }
}