using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TestSpeedyGonzales : MonoBehaviour
{
    [Header("Genetic Algorithm")]
    [SerializeField] int populationSize = 200;
    [SerializeField] float mutationRate = 0.01f;
    [SerializeField] int elitism = 5;
    [SerializeField] int mercy = 5;
    [SerializeField] bool bestWorse = true;
    [SerializeField] bool Elitism = true;
    [SerializeField] bool crossoverNewDNA = true;

    [Header("Save And Load")]
    [SerializeField] bool Encrypt = false;
    //[SerializeField] bool Save = false;
    //[SerializeField] bool Load = false;
    [SerializeField] string SaveName = "Genetic_Save_Speedy";
    [SerializeField] bool RunTest = true;

    [Header("Speedy")]
    [SerializeField] private float GenerationTime = 5.0f;
    [Header("Other")]
    [SerializeField] TestSpeedy SpeedyPrefab;
    [SerializeField] Text BestText;
    [SerializeField] Text GenerationText;
    [SerializeField] Text PopulationText;

    private Andre.AI.GeneticAlgorithm<float> ga;
    private System.Random random;
    private int CurrentPopulationSize = 200;
    bool Finshied = false;
    List<TestSpeedy> SpeedyGeneration;

    private string fullSavePath;
    private float GenerationTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        CurrentPopulationSize = populationSize;
        SpeedyGeneration = new List<TestSpeedy>();
        random = new System.Random();
        ga = new Andre.AI.GeneticAlgorithm<float>(populationSize, 3, random, elitism, mercy, GetRandomFloat, FitnessFunction, Mutate, mutationRate);

        //if (Encrypt)
        //    fullSavePath = Application.persistentDataPath + "/" + SaveName + "_Encrypted.GA2B";
        //else
        //    fullSavePath = Application.persistentDataPath + "/" + SaveName + ".GA2B";
        //if (Load)
        //{
        //    ga.LoadGeneration(fullSavePath, Encrypt);
        //    Load = false;
        //}
        SpawnSpeedies(populationSize);
        Run(true);
        UpdateUI();
    }

    void Load()
    {
        if (Encrypt)
            fullSavePath = Application.persistentDataPath + "/" + SaveName + "_Encrypted.GA2B";
        else
            fullSavePath = Application.persistentDataPath + "/" + SaveName + ".GA2B";
        ga.LoadGeneration(fullSavePath, Encrypt);
        //Load = false;

        Finshied = false;
    }

    void Save()
    {
        if (Encrypt)
            fullSavePath = Application.persistentDataPath + "/" + SaveName + "_Encrypted.GA2B";
        else
            fullSavePath = Application.persistentDataPath + "/" + SaveName + ".GA2B";
        ga.SaveGeneration(fullSavePath, Encrypt);

    }

    // Update is called once per frame
    void Update()
    {
        //if (Load)
        //{
        //    if (Encrypt)
        //        fullSavePath = Application.persistentDataPath + "/" + SaveName + "_Encrypted.GA2B";
        //    else
        //        fullSavePath = Application.persistentDataPath + "/" + SaveName + ".GA2B";
        //    ga.LoadGeneration(fullSavePath, Encrypt);
        //    SpawnSpeedies(populationSize);
        //    Load = false;
        //    Finshied = false;
        //}

        //if (Save)
        //{
        //    if (Encrypt)
        //        fullSavePath = Application.persistentDataPath + "/" + SaveName + "_Encrypted.GA2B";
        //    else
        //        fullSavePath = Application.persistentDataPath + "/" + SaveName + ".GA2B";
        //    ga.SaveGeneration(fullSavePath, Encrypt);
        //    Save = false;
        //}

        if (Finshied)
            return;


        if (GenerationTimer >= GenerationTime)
        {
            int Difference = populationSize - CurrentPopulationSize;
            GenerationTimer = 0;
            ga.NewGeneration(Difference, elitism, mercy, Elitism, bestWorse, crossoverNewDNA);
            if (Difference != 0)
            {
                if (Difference < 0)
                    RemoveSpeedies(Difference * -1);
                else
                    SpawnSpeedies(Difference);
            }
            Run(false);
            ResetSpeedies();
            Run(true);
            CurrentPopulationSize = populationSize;
            UpdateUI();
        }
        else
            GenerationTimer += Time.deltaTime;
        ga.MutationRate = mutationRate;

        if (!RunTest)
        {
            UpdateUI();
            Finshied = true;
        }
    }

    private void Run(bool start)
    {
        for (int i = 0; i < populationSize; i++)
        {
            SpeedyGeneration[i].Run = start;
        }
    }


    private void SpawnSpeedies(int dif)
    {
        for (int i = 0; i < dif; i++)
        {
            SpeedyGeneration.Add(Instantiate(SpeedyPrefab, this.transform));
            SpeedyGeneration[i].Speed = ga.Population[i].Genes[0];
            SpeedyGeneration[i].Acceleration = ga.Population[i].Genes[1];
            SpeedyGeneration[i].Weight = ga.Population[i].Genes[2];
            SpeedyGeneration[i].BeginingPos = this.transform.position;
        }
    }

    private void RemoveSpeedies(int dif)
    {
        for (int i = 0; i < dif; i++)
        {
            GameObject obj = SpeedyGeneration[i].gameObject;
            SpeedyGeneration.Remove(SpeedyGeneration[i]);
            Destroy(obj);
        }
    }

    private void ResetSpeedies()
    {
        for (int i = 0; i < populationSize; i++)
        {
            SpeedyGeneration[i].transform.position = this.transform.position;
            SpeedyGeneration[i].Speed = ga.Population[i].Genes[0];
            SpeedyGeneration[i].Acceleration = ga.Population[i].Genes[1];
            SpeedyGeneration[i].Weight = ga.Population[i].Genes[2];
            SpeedyGeneration[i].BeginingPos = this.transform.position;
            SpeedyGeneration[i].resetA();
        }
    }

    private float GetRandomFloat(int index)
    {
        float i = 0;
        switch (index)
        {
            case 0:
                i = (float)random.NextDouble() *9 + 2;
                break;
            case 1:
                i = (float)random.NextDouble() * 9 + 2;
                break;
            case 2:
                i = (float)random.NextDouble() * 19 + 1;
                break;
            default:
                break;
        }
        return i = i < 0.5f ? 0.5f : i;
    }

    private bool Mutate (Andre.AI.DNA<float> child, float MutationRate)
    {
        double PositiveOrNegative = random.NextDouble();
        for (int i = 0; i < 3; i++)
        {
            if (random.NextDouble() < mutationRate)
            {
                child.Genes[i] += (float)random.NextDouble() * (PositiveOrNegative >= 0.5 ? 1 : -1);
                switch (i)
                {
                    case 0:
                        if (child.Genes[i] <= 0)
                            child.Genes[i] = 0.01f;
                        break;
                    case 1:
                        if (child.Genes[i] <= 0.5f)
                            child.Genes[i] = 0.5f;
                        break;
                    case 2:
                        if (child.Genes[i] <= 0.5f)
                            child.Genes[i] = 0.5f;
                        break;
                    default:
                        break;
                }
            }
        }
        return true;
    }

    private float FitnessFunction(int index)
    {
        float score = 0;
        Andre.AI.DNA<float> dna = ga.Population[index];

        score = SpeedyGeneration[index].DistanceTraveled;

        score = (Mathf.Pow(2, score) - 1) / (2 - 1);

        return score;
    }

    private void UpdateUI()
    {
        GenerationText.text = "Generation: " + ga.Generation;
        BestText.text = "Best Genes: Speed : " +ga.BestGenes[0] +" Acceleration : " + ga.BestGenes[1] + " Weight : " + ga.BestGenes[2];
        PopulationText.text = "Population : " + ga.Population.Count;
    }
}
