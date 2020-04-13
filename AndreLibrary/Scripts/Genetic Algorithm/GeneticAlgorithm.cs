using System;
using System.Collections.Generic;

namespace Andre.AI
{
    public class GeneticAlgorithm<T>
    {
        public List<DNA<T>> Population { get; private set; }
        public int Generation { get; private set; }
        public float BestFitness { get; private set; }
        public T[] BestGenes { get; private set; }


        public float MutationRate;

        private Random random;
        private float fitnessSum = 0;
        private int elitist;
        private int mercy;
        private List<DNA<T>> newPopulation;
        private List<DNA<T>> MatingPool;

        private int dnaSize = 0;
        private Func<int, T> getRandomGene;
        private Func<int, float> fitnessFunction;
        private Func<DNA<T>, float, bool> mutateGene;

        public GeneticAlgorithm(int populationSize, int dnaSize, Random random, int elitist, int mercy,
            Func<int, T> getRandomGene, Func<int, float> fitnessFunction,
            Func<DNA<T>, float, bool> mutateGene = null, float mutationRate = 0.01f)
        {
            Generation = 1;
            MutationRate = mutationRate;
            this.random = random;
            this.elitist = elitist;
            this.mercy = mercy;

            Population = new List<DNA<T>>(populationSize);
            newPopulation = new List<DNA<T>>(populationSize);
            MatingPool = new List<DNA<T>>();
            BestGenes = new T[dnaSize];
            this.dnaSize = dnaSize;
            this.getRandomGene = getRandomGene;
            this.fitnessFunction = fitnessFunction;
            this.mutateGene = mutateGene;


            for (int i = 0; i < populationSize; i++)
            {
                Population.Add(new DNA<T>(dnaSize, random, getRandomGene, fitnessFunction));
            }
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
            newPopulation.Clear();

            for (int i = 0; i < finalCount; i++)
            {
                if ((i < elitist && Elitism) && i < Population.Count)
                    newPopulation.Add(Population[i]);
                else if (i < Population.Count || crossoverNewDNA)
                {
                    DNA<T> parent1 = null;
                    DNA<T> parent2 = null;
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

                    DNA<T> child = parent1.Crossover(parent2);

                    if (mutateGene == null)
                        child.Mutate(MutationRate);
                    else
                        mutateGene(child, MutationRate);

                    newPopulation.Add(child);
                }
                else
                {
                    newPopulation.Add(new DNA<T>(dnaSize, random, getRandomGene, fitnessFunction, true));
                }
            }

            List<DNA<T>> tmpList = Population;
            Population = newPopulation;
            newPopulation = tmpList;

            Generation++;
        }

        public void SaveGeneration(string filePath, bool Encript)
        {
            SaveData.GeneticSaveData<T> save = new SaveData.GeneticSaveData<T>
            {
                Generation = Generation,
                PopulationGenes = new List<T[]>(Population.Count),
            };

            for (int i = 0; i < Population.Count; i++)
            {
                save.PopulationGenes.Add(new T[dnaSize]);
                Array.Copy(Population[i].Genes, save.PopulationGenes[i], dnaSize);
            }

            if (Encript)
                Utils.FileReadWrite.WriteEncryptedToBinaryFile(filePath, save);
            else
                Utils.FileReadWrite.WriteToBinaryFile(filePath, save);
        }

        public bool LoadGeneration(string filePath, bool Encript)
        {
            if (!System.IO.File.Exists(filePath))
                return false;
            SaveData.GeneticSaveData<T> save = null;
            if (Encript)
                save = Utils.FileReadWrite.ReadEncryptedFromBinaryFile<SaveData.GeneticSaveData<T>>(filePath);
            else
                save = Utils.FileReadWrite.ReadFromBinaryFile<SaveData.GeneticSaveData<T>>(filePath);
            Generation = save.Generation;
            for (int i = 0; i < save.PopulationGenes.Count; i++)
            {
                if (i >= Population.Count)
                {
                    Population.Add(new DNA<T>(dnaSize, random, getRandomGene, fitnessFunction, false));
                }
                Array.Copy(save.PopulationGenes[i], Population[i].Genes, dnaSize);
            }
            return true;
        }

        public int CompareDNA(DNA<T> a, DNA<T> b)
        {
            if (a.Fitness > b.Fitness)
                return -1;
            else if (a.Fitness < b.Fitness)
                return 1;
            else
                return 0;
        }

        public void CalculateFitness()
        {
            fitnessSum = 0;
            DNA<T> best = Population[0];

            for (int i = 0; i < Population.Count; i++)
            {
                fitnessSum += Population[i].CalculateFitness(i);

                if (Population[i].Fitness > best.Fitness)
                    best = Population[i];
            }

            BestFitness = best.Fitness;
            best.Genes.CopyTo(BestGenes, 0);
        }

        private DNA<T> ChooseParent()
        {
            double randomNumber = random.NextDouble() * fitnessSum;

            for (int i = 0; i < Population.Count; i++)
            {
                if (randomNumber < Population[i].Fitness)
                    return Population[i];

                randomNumber -= Population[i].Fitness;
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

        private DNA<T> MixedChooseParent(DNA<T> parent)
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
}
