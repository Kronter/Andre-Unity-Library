using System;
using System.Collections;
using System.Collections.Generic;

public class NeatGeneticAlgorithm
{

    public List<NeatNeuralNetwork> Population { get; private set; }
    public int Generation { get; private set; }
    public float BestFitness { get; private set; }
    public NeuralGenome BestGenes { get; private set; }


    public float MutationRate;
    public float[] mutationRates = new float[7];

    private Random random;
    private float fitnessSum = 0;
    private int elitist;
    private int mercy;
    private List<NeatNeuralNetwork> newPopulation;
    private List<NeatNeuralNetwork> MatingPool;
    private double learnRate;
    private double momentum;
    private NeuralActivationFunction[] neuralActivationFunctions;
    private int inputSize;
    private int outputSize;
    public int globalInnovation = 1;

    private Func<int, float> fitnessFunction;
    private Func<NeatNeuralNetwork, float, bool> mutateGene;

    public NeatGeneticAlgorithm(int populationSize, Random random, int elitist, int mercy, NeuralActivationFunction[] _neuralActivationFunctions,
        int _inputSize, int _outputSize, Func<int, float> _fitnessFunction, double _learnRate = -1, double _momentum = -1, float mutationRate = 0.01f,
        float[] _mutationRates = null, Func<NeatNeuralNetwork, float, bool> mutateGene = null)
    {
        globalInnovation = 1;
        Generation = 1;
        MutationRate = mutationRate;
        this.random = random;
        this.elitist = elitist;
        this.mercy = mercy;
        learnRate = _learnRate;
        momentum = _momentum;
        inputSize = _inputSize;
        outputSize = _outputSize;
        neuralActivationFunctions = _neuralActivationFunctions;
        if (_mutationRates != null)
            mutationRates = _mutationRates;
        else
        {
            for (int i = 0; i < mutationRates.Length; i++)
            {
                mutationRates[i] = (float)(i+1)/(float)(mutationRates.Length+1);
            }
        }

        fitnessFunction = _fitnessFunction;
        Population = new List<NeatNeuralNetwork>(populationSize);
        newPopulation = new List<NeatNeuralNetwork>(populationSize);
        MatingPool = new List<NeatNeuralNetwork>();
        BestGenes = new NeuralGenome();

        Population.Add(new NeatNeuralNetwork(neuralActivationFunctions, inputSize, outputSize, globalInnovation, true, fitnessFunction, mutationRates, IncreaseGlobalInnovation, learnRate, momentum));
        for (int i = 0; i < populationSize - 1; i++)
        {
            Population.Add(new NeatNeuralNetwork(neuralActivationFunctions, inputSize, outputSize, globalInnovation, false, fitnessFunction, mutationRates, IncreaseGlobalInnovation, learnRate, momentum));
        }

        for (int i = 0; i < populationSize; i++)
        {
            Population[i].genome.Mutate(mutationRate);
        }
    }

    private int IncreaseGlobalInnovation (int _currInnovation)
    {
        globalInnovation++;
        foreach (var network in Population)
        {
            network.genome.currInnovation = globalInnovation;
        }
        return 0;
    }

    public void NewGeneration(int numNewDNA = 0, int elitism = 0, int mercy = 0, bool Elitism = true, bool bestWorst = true, bool crossoverNewDNA = true)
    {
        int finalCount = Population.Count + numNewDNA;

        if (finalCount <= 0)
            return;

        elitist = elitism;
        this.mercy = mercy;

        if (Population.Count > 0)
        {
            CalculateFitness();
            Population.Sort(CompareDNA);

            if (elitist > 0)
                GetBestAndWorstParents();
            else
                bestWorst = false;
        }
        newPopulation = new List<NeatNeuralNetwork>();

        for (int i = 0; i < finalCount; i++)
        {
            if ((i < elitist && Elitism) && i < Population.Count)
                newPopulation.Add(Population[i]);
            else if (i < Population.Count || crossoverNewDNA)
            {
                NeatNeuralNetwork parent1 = null;
                NeatNeuralNetwork parent2 = null;
                if (bestWorst)
                {
                    parent1 = MixedChooseParent(null);
                    parent2 = MixedChooseParent(parent1);
                }
                else
                {
                    parent1 = ChooseParent();
                    parent2 = ChooseParent();
                }

                NeatNeuralNetwork child = parent1.Crossover(parent2);

                if (mutateGene == null)
                    child.genome.Mutate(MutationRate);
                else
                    mutateGene(child, MutationRate);

                newPopulation.Add(child);
            }
            else
            {
                newPopulation.Add(new NeatNeuralNetwork(neuralActivationFunctions, inputSize, outputSize, globalInnovation, false, fitnessFunction, mutationRates, IncreaseGlobalInnovation, learnRate, momentum));
            }
        }

       // List<NeatNeuralNetwork> tmpList = Population;
        Population = newPopulation;
        newPopulation = new List<NeatNeuralNetwork>();
        Generation++;
    }

    //public void SaveGeneration(string filePath, bool Encript)
    //{
    //    SaveData.GeneticSaveData<T> save = new SaveData.GeneticSaveData<T>
    //    {
    //        Generation = Generation,
    //        PopulationGenes = new List<T[]>(Population.Count),
    //    };

    //    for (int i = 0; i < Population.Count; i++)
    //    {
    //        save.PopulationGenes.Add(new T[dnaSize]);
    //        Array.Copy(Population[i].Genes, save.PopulationGenes[i], dnaSize);
    //    }

    //    if (Encript)
    //        Utils.FileReadWrite.WriteEncryptedToBinaryFile(filePath, save);
    //    else
    //        Utils.FileReadWrite.WriteToBinaryFile(filePath, save);
    //}

    //public bool LoadGeneration(string filePath, bool Encript)
    //{
    //    if (!System.IO.File.Exists(filePath))
    //        return false;
    //    SaveData.GeneticSaveData<T> save = null;
    //    if (Encript)
    //        save = Utils.FileReadWrite.ReadEncryptedFromBinaryFile<SaveData.GeneticSaveData<T>>(filePath);
    //    else
    //        save = Utils.FileReadWrite.ReadFromBinaryFile<SaveData.GeneticSaveData<T>>(filePath);
    //    Generation = save.Generation;
    //    for (int i = 0; i < save.PopulationGenes.Count; i++)
    //    {
    //        if (i >= Population.Count)
    //        {
    //            Population.Add(new DNA<T>(dnaSize, random, getRandomGene, fitnessFunction, false));
    //        }
    //        Array.Copy(save.PopulationGenes[i], Population[i].Genes, dnaSize);
    //    }
    //    return true;
    //}

    public int CompareDNA(NeatNeuralNetwork a, NeatNeuralNetwork b)
    {
        if (a.genome.fitness > b.genome.fitness)
            return -1;
        else if (a.genome.fitness < b.genome.fitness)
            return 1;
        else
            return 0;
    }

    public void CalculateFitness()
    {
        fitnessSum = 0;
        NeatNeuralNetwork best = Population[0];

        for (int i = 0; i < Population.Count; i++)
        {
            fitnessSum += Population[i].genome.CalculateFitness(i);

            if (Population[i].genome.fitness > best.genome.fitness)
                best = Population[i];
        }

        BestFitness = (float)best.genome.fitness;
        BestGenes = best.genome;
    }



    private NeatNeuralNetwork ChooseParent()
    {
        double randomNumber = random.NextDouble() * fitnessSum;

        for (int i = 0; i < Population.Count; i++)
        {
            if (randomNumber < Population[i].genome.fitness)
                return Population[i];

            randomNumber -= Population[i].genome.fitness;
        }

        return null;
    }

    private void GetBestAndWorstParents()
    {
        MatingPool.Clear();

        for (int i = 0; i < elitist * 2; i++)
        {
            MatingPool.Add(Population[i]);
        }

        int h = mercy;
        int mercyNumber = random.Next(Population.Count - (mercy * 2), Population.Count);
        while (h != 0)
        {
            int times = 0;
            while (MatingPool.Contains(Population[mercyNumber]))
            {
                if (times >= 500)
                    break;
                mercyNumber = random.Next(Population.Count - (mercy * 2), Population.Count);
                times++;
            }
            if (times < 500)
                MatingPool.Add(Population[mercyNumber]);
            h--;
        }
        //for (int h = Population.Count - mercy; h < Population.Count; h++)
        //{
        //    MatingPool.Add(Population[h]);
        //}
    }

    private NeatNeuralNetwork MixedChooseParent(NeatNeuralNetwork parent)
    {
        int randomNumber = random.Next(0, MatingPool.Count);

        if (parent != null)
        {
            while (MatingPool[randomNumber] == parent)
            {
                randomNumber = random.Next(0, MatingPool.Count);
            }
        }
        return MatingPool[randomNumber];
    }
}
