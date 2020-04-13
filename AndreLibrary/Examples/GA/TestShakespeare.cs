using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class TestShakespeare : MonoBehaviour
{
    [Header("Genetic Algorithm")]
    [SerializeField] string targetString = "To be, or not to be, that is the question.";
    [SerializeField] string validCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ,.|!#$%&/()=? ";
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
    [SerializeField] string SaveName = "Genetic_Save";

    [Header("Other")]
    [SerializeField] int numCharsPerText = 15000;
    [SerializeField] Text targetText;
    [SerializeField] Text bestText;
    [SerializeField] Text bestFitnessText;
    [SerializeField] Text numGenerationsText;
    [SerializeField] Transform populationTextParent;
    [SerializeField] Text textPrefab;

    private Andre.AI.GeneticAlgorithm<char> ga;
    private System.Random random;
    private int CurrentPopulationSize = 200;
    bool Finshied = false;

    private string fullSavePath;

    void Start()
    {
        targetText.text = targetString;
        CurrentPopulationSize = populationSize;

        if (string.IsNullOrEmpty(targetString))
        {
            Debug.LogError("Target string is null or empty");
            this.enabled = false;
        }

        random = new System.Random();
        ga = new Andre.AI.GeneticAlgorithm<char>(populationSize, targetString.Length,
            random, elitism, mercy, GetRandomCharacter, FitnessFunction, null, mutationRate);

        //if (Encrypt)
        //    fullSavePath = Application.persistentDataPath + "/" + SaveName + "_Encrypted.GA2B";
        //else
        //    fullSavePath = Application.persistentDataPath + "/" + SaveName + ".GA2B";
        //if (Load)
        //{
        //    ga.LoadGeneration(fullSavePath, Encrypt);
        //    Load = false;
        //}
        //Load();
        UpdateText(ga.BestGenes, ga.BestFitness, ga.Generation);
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

    void Update()
    {

        //if (Load)
        //{
        //    if (Encrypt)
        //        fullSavePath = Application.persistentDataPath + "/" + SaveName + "_Encrypted.GA2B";
        //    else
        //        fullSavePath = Application.persistentDataPath + "/" + SaveName + ".GA2B";
        //    ga.LoadGeneration(fullSavePath, Encrypt);
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
        int Difference = CurrentPopulationSize - populationSize;
        ga.NewGeneration(Difference, elitism, mercy, Elitism, bestWorse, crossoverNewDNA);
        ga.MutationRate = mutationRate;

       UpdateText(ga.BestGenes, ga.BestFitness, ga.Generation);

        if (ga.BestFitness == 1)
        {
            UpdateAllText(ga.BestGenes, ga.BestFitness, ga.Generation, ga.Population.Count, (j) => ga.Population[j].Genes);
            Finshied = true;
        }

        CurrentPopulationSize = populationSize;
    }

    private char GetRandomCharacter(int index)
    {
        int i = random.Next(validCharacters.Length);
        return validCharacters[i];
    }

    private float FitnessFunction(int index)
    {
        float score = 0;
        Andre.AI.DNA<char> dna = ga.Population[index];

        for (int i = 0; i < dna.Genes.Length; i++)
        {
            if (dna.Genes[i] == targetString[i])
            {
                score += 1;
            }
        }

        score /= targetString.Length;

        score = (Mathf.Pow(2, score) - 1) / (2 - 1);

        return score;
    }

    private int numCharsPerTextObj;
    private List<Text> textList = new List<Text>();

    void Awake()
    {
        numCharsPerTextObj = numCharsPerText / validCharacters.Length;
        if (numCharsPerTextObj > populationSize) numCharsPerTextObj = populationSize;

        int numTextObjects = Mathf.CeilToInt((float)populationSize / numCharsPerTextObj);

        for (int i = 0; i < numTextObjects; i++)
        {
            textList.Add(Instantiate(textPrefab, populationTextParent));
        }
    }

    private void UpdateAllText(char[] bestGenes, float bestFitness, int generation, int populationSize, Func<int, char[]> getGenes)
    {
        bestText.text = bestGenes.CharArrayToString();
        bestFitnessText.text = bestFitness.ToString();

        numGenerationsText.text = generation.ToString();

        for (int i = 0; i < textList.Count; i++)
        {
            var sb = new StringBuilder();
            int endIndex = i == textList.Count - 1 ? populationSize : (i + 1) * numCharsPerTextObj;
            for (int j = i * numCharsPerTextObj; j < endIndex; j++)
            {
                foreach (var c in getGenes(j))
                {
                    sb.Append(c);
                }
                if (j < endIndex - 1) sb.AppendLine();
            }

            textList[i].text = sb.ToString();
        }
    }

    private void UpdateText(char[] bestGenes, float bestFitness, int generation)
    {
        bestText.text = bestGenes.CharArrayToString();
        bestFitnessText.text = bestFitness.ToString();
        numGenerationsText.text = generation.ToString();
    }
}
