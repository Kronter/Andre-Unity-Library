using System;
using System.Collections.Generic;

namespace Andre.AI.SaveData
{
    [Serializable]
    public class GeneticSaveData<T>
    {
        public List<T[]> PopulationGenes;
        public int Generation;
    }
}
