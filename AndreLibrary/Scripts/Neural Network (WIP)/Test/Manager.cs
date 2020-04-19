using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{

    public float timeframe;
    [Header("Genetic Algorithm")]
    [SerializeField] int populationSize = 200;
    [SerializeField] [Range(0.0001f, 1f)] float mutationRate = 0.01f;
    [SerializeField] int elitism = 5;
    [SerializeField] int mercy = 5;
    [SerializeField] bool bestWorse = true;
    [SerializeField] bool Elitism = true;
    [SerializeField] bool crossoverNewDNA = true;
    [SerializeField] int fitnessToStop = 40;
    [SerializeField] float turnPenalty = 0.01f;
    public int minFitnessStegnationStop = 70;
    public int maxConsecutiveGensSagnent = 50;
    public bool ShowGenBest = false;
    public bool ShowOnlyGenBest = false;
    public GameObject prefab;//holds bot prefab
    public RunBestCar bestCarRun;

    private static readonly System.Random Random = new System.Random();
    [SerializeField]
    private NeuralActivationFunction[] neuralActivationFunctions = new NeuralActivationFunction[3];
    //public int[] layers = new int[3] { 5, 3, 2 };//initializing network to the right size


    [Range(0.1f, 10f)] public float Gamespeed = 1f;

    //public List<Bot> Bots;
    private List<Bot> cars;
    private NeatGeneticAlgorithm neatGen;
    [SerializeField] private bool done = false;
    float timeSinceLastSpawn = 0;
    double bestFitness = 0;
    int Generations = 0;
    int ConsecutiveGensSagnent = 0;
    void Start()// Start is called before the first frame update
    {
        if (populationSize % 2 != 0)
            populationSize = 50;//if population size is not even, sets it to fifty
        neatGen = new NeatGeneticAlgorithm(populationSize, Random, elitism, mercy, neuralActivationFunctions, 5, 2, null,-1,-1, mutationRate);
       InvokeRepeating("CreateBots", 0.1f, timeframe);//repeating function
    }
    bool changeColoursOnce = true;
    private void Update()
    {
        if (ShowGenBest && Generations > 0)
        {
            changeColoursOnce = false;
            Bot BestCar = cars[0];
            for (int i = 0; i < populationSize; i++)
            {
                if (cars[i].network.genome.fitness == BestCar.network.genome.fitness)
                    BestCar = cars[i];
                cars[i].GetComponent<MeshRenderer>().material.color = new Color(0.3349057f, 0.9562678f, 1.0f, 07843138f);
            }
            BestCar.GetComponent<MeshRenderer>().material.color = Color.red;
        }
        else if(ShowOnlyGenBest && Generations > 0)
        {
            changeColoursOnce = false;
            Bot BestCar = cars[0];
            for (int i = 0; i < populationSize; i++)
            {
                if (cars[i].network.genome.fitness == BestCar.network.genome.fitness)
                    BestCar = cars[i];
                cars[i].GetComponent<MeshRenderer>().enabled = false;
            }
            BestCar.GetComponent<MeshRenderer>().enabled = true;
            BestCar.GetComponent<MeshRenderer>().material.color = Color.red;
        }
        else if(!changeColoursOnce && Generations > 0)
        {
            if (cars.Count > 0)
            {
                for (int i = 0; i < populationSize; i++)
                {
                    cars[i].GetComponent<MeshRenderer>().enabled = true;
                    cars[i].GetComponent<MeshRenderer>().material.color = Color.white;
                }
                changeColoursOnce = true;
            }
        }
    }
    //public void InitNetworks()
    //{
    //    networks = new List<NeuralNetwork>();
    //    for (int i = 0; i < populationSize; i++)
    //    {
    //        NeuralNetwork net = new NeuralNetwork(layers);
    //        net.Load("Assets/Save.txt");//on start load the network save
    //        networks.Add(net);
    //    }
    //}

    public void CreateBots()
    {
        Time.timeScale = Gamespeed;//sets gamespeed, which will increase to speed up training

        if (done)
        {
            if (cars.Count > 0)
            {
                for (int i = 0; i < cars.Count; i++)
                {
                    if (cars[i] != null)
                        Destroy(cars[i].gameObject);//if there are Prefabs in the scene this will get rid of them
                }
            }
            cars = new List<Bot>();
            enabled = false;
            return;
        }
        //if (Time.time - timeSinceLastSpawn < timeframe)
        //    return;

        if (cars != null)
        {
            for (int i = 0; i < cars.Count; i++)
            {
                Destroy(cars[i].gameObject);//if there are Prefabs in the scene this will get rid of them
            }

            SortNetworks();//this sorts networks and mutates them
        }

        cars = new List<Bot>();
        for (int i = 0; i < populationSize; i++)
        {
            Bot car = (Instantiate(prefab, new Vector3(0, 1.6f, -16), new Quaternion(0, 0, 1, 0))).GetComponent<Bot>();//create botes
            car.network = neatGen.Population[i];//deploys network to each learner
            car.turnPenalty = turnPenalty;
            cars.Add(car);
        }
        changeColoursOnce = false;
        // timeSinceLastSpawn = Time.time;
    }

    public void SortNetworks()
    {
        for (int i = 0; i < populationSize; i++)
        {
            cars[i].UpdateFitness();//gets bots to set their corrosponding networks fitness
        }
        cars.Sort((c1, c2) => c1.position.CompareTo(c2.position));
        neatGen.Population.Sort(neatGen.CompareDNA);
        Debug.Log("Best Fitness: " + neatGen.Population[0].genome.fitness);
        Generations++;
        if (bestFitness == neatGen.Population[0].genome.fitness || (bestFitness <= neatGen.Population[0].genome.fitness + (2f* turnPenalty)  && bestFitness >= neatGen.Population[0].genome.fitness - (2f * turnPenalty)))
            ConsecutiveGensSagnent++;
        else
            ConsecutiveGensSagnent = 0;
        bestFitness = neatGen.Population[0].genome.fitness;
        if (neatGen.Population[0].genome.fitness >= fitnessToStop
            && !cars[0].collided)
        {
            bestCarRun.SpawnBestCar(prefab, neatGen.Population[0]);
            if (cars.Count > 0)
            {
                for (int i = 0; i < cars.Count; i++)
                {
                    if (cars[i] != null)
                        Destroy(cars[i].gameObject);//if there are Prefabs in the scene this will get rid of them
                }
            }
            cars = new List<Bot>();
            done = true;
            Gamespeed = 1;
            return;
        }
        if (neatGen.Population[0].genome.fitness >= minFitnessStegnationStop && ConsecutiveGensSagnent >= maxConsecutiveGensSagnent
            && !cars[0].collided)
        {
            bestCarRun.SpawnBestCar(prefab, neatGen.Population[0]);
            if (cars.Count > 0)
            {
                for (int i = 0; i < cars.Count; i++)
                {
                    if (cars[i] != null)
                        Destroy(cars[i].gameObject);//if there are Prefabs in the scene this will get rid of them
                }
            }
            cars = new List<Bot>();
            done = true;
            Gamespeed = 1;
            return;
        }
        neatGen.NewGeneration(0, elitism, mercy, bestWorse, crossoverNewDNA);
    }
}