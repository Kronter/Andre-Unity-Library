using System;

namespace Andre.AI
{
    public class DNA<T>
    {
        public T[] Genes { get; private set; }
        public float Fitness { get; private set; }
        private Random random;
        private Func<int, T> getRandomGene;
        private Func<int, float> fitnessFunction;

        public DNA(int size, Random random, Func<int, T> getRandomGene, Func<int, float> fitnessFunction, bool Init = true)
        {
            Genes = new T[size];
            this.random = random;
            this.getRandomGene = getRandomGene;
            this.fitnessFunction = fitnessFunction;

            if (Init)
            {
                for (int i = 0; i < Genes.Length; i++)
                {
                    Genes[i] = getRandomGene(i);
                }
            }
        }

        public float CalculateFitness(int index)
        {
            Fitness = fitnessFunction(index);
            return Fitness;
        }

        public DNA<T> Crossover(DNA<T> otherParent)
        {
            DNA<T> child = new DNA<T>(Genes.Length, random, getRandomGene, fitnessFunction, false);

            for (int i = 0; i < Genes.Length; i++)
            {
                child.Genes[i] = random.NextDouble() < 0.5f ? Genes[i] : otherParent.Genes[i];
            }

            return child;
        }

        public void Mutate(float mutationRate)
        {
            for (int i = 0; i < Genes.Length; i++)
            {
                if (random.NextDouble() < mutationRate)
                {
                    Genes[i] = getRandomGene(i);
                }
            }
        }
    }
}
