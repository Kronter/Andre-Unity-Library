using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ColorPicking : MonoBehaviour
{

    //Neural Network Variables
    [SerializeField]
    private double MinimumError = 0.007;
    [SerializeField]
    private int numEpochs = 100;
    [SerializeField]
    private TrainingType TrType = TrainingType.MinimumError;
    private static NeuralNetwork net;
    //private static NeatNeuralNetwork net;
    private static List<NeuralDataSet> dataSets;
    [SerializeField]
    private NeuralActivationFunction[] neuralActivationFunctions;
    [SerializeField]
    private float manualTrainTime = 27;

    public Image I1;
    public Image I2;
    public Text t1;
    public Text t2;
    public Text timesToTrain;

    public GameObject pointer1;
    public GameObject pointer2;
    public GameObject pointer3;
    public GameObject pointer4;

    bool trained;

    int i = 0;

    // Use this for initialization
    void Start()
    {
        //Input - 3 (r,g,b) + text(r,g,b) -- Output - 1 (Black/White)
        net = new NeuralNetwork(neuralActivationFunctions, 3, 4, 1);
        //net = new NeatNeuralNetwork(neuralActivationFunctions, 3, 1);
        //NeuralGeneNode outputNode = net.genome.OutputLayer[0];

        //foreach (var connection in outputNode.inputSynapses)
        //{
        //    connection.connectionIsEnabled = false;
        //}
        //NeuralGeneNode tmpHiddenNode;
        //for (int i = 0; i < 4; i++)
        //{
        //    tmpHiddenNode = net.genome.AddHiddenNode(neuralActivationFunctions[1]);
        //    net.genome.AddConnection(net.genome.InputLayer, tmpHiddenNode, true);
        //}

        //net.genome.AddConnection(net.genome.HiddenLayers, outputNode, true);

        //int rand = Random.Range(0, net.genome.HiddenLayers.Count);
        //NeuralGeneNode changeNode = net.genome.GetNode(net.genome.HiddenLayers[rand].nodeNumber);
        //tmpHiddenNode = net.genome.AddHiddenNode(neuralActivationFunctions[1]);
        //foreach (var connection in outputNode.inputSynapses)
        //{
        //    if (connection.inputNeuron == changeNode)
        //        connection.connectionIsEnabled = false;
        //}
        //net.genome.AddConnection(tmpHiddenNode, outputNode, true);
        //net.genome.AddConnection(changeNode, tmpHiddenNode, true);


        dataSets = new List<NeuralDataSet>();
        if (manualTrainTime < 27)
            manualTrainTime = 27;

        timesToTrain.text = "Manual Train Times Left: " + (manualTrainTime - i).ToString();
        Next();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N) && trained)
            Next();
    }

    void Next()
    {
        Color c = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
        I1.color = c;
        I2.color = c;
        t1.color = c;
        t2.color = c;
        double[] C = { (double)I1.color.r, (double)I1.color.g, (double)I1.color.b};
        if (trained)
        {
            double d = tryValues(C);
            if (d > 0.75f)
            {
                pointer1.SetActive(false);
                pointer2.SetActive(false);
                pointer3.SetActive(false);
                pointer4.SetActive(true);
            }
            else if(d > 0.5f)
            {
                pointer1.SetActive(false);
                pointer2.SetActive(false);
                pointer3.SetActive(true);
                pointer4.SetActive(false);
            }
            else if(d > 0.25f)
            {
                pointer1.SetActive(false);
                pointer2.SetActive(true);
                pointer3.SetActive(false);
                pointer4.SetActive(false);
            }
            else
            {
                pointer1.SetActive(true);
                pointer2.SetActive(false);
                pointer3.SetActive(false);
                pointer4.SetActive(false);
            }
        }
    }

    public void Train(float _val)
    {
        double[] C = { (double)I1.color.r, (double)I1.color.g, (double)I1.color.b};
        float tmp = (_val / 4) - 0.25f;
        double[] v = { (double)tmp};
        dataSets.Add(new NeuralDataSet(C, v));

        i++;
        if(i < manualTrainTime)
            timesToTrain.text = "Manual Train Times Left: " + (manualTrainTime - i).ToString();

        if (!trained && i == manualTrainTime)
        {
            timesToTrain.text = "Training";
            Train();
        }

        Next();

    }

    private void Train()
    {
        if(TrType == TrainingType.MinimumError)
            net.Train(dataSets, MinimumError);
        else
            net.Train(dataSets, numEpochs);
        trained = true;
        timesToTrain.text = "Ready Press N For Next Guess";
    }

    double tryValues(double[] _vals)
    {
        double[] result = net.Compute(_vals);
        return result[0];
    }

    Vector3 ScreenToWorld(float x, float y)
    {
        Camera camera = Camera.current;
        Vector3 s = camera.WorldToScreenPoint(transform.position);
        return camera.ScreenToWorldPoint(new Vector3(x, camera.pixelHeight - y, s.z));
    }
    Rect ScreenRect(int x, int y, int w, int h)
    {
        Vector3 tl = ScreenToWorld(x, y);
        Vector3 br = ScreenToWorld(x + w, y + h);
        return new Rect(tl.x, tl.y, br.x - tl.x, br.y - tl.y);
    }

    private void OnDrawGizmos()
    {
        Rect rect = ScreenRect(5, 40, 150, 100);
        UnityEditor.Handles.DrawSolidRectangleWithOutline(rect, Color.black, Color.white);
        Gizmos.color = Color.white;
        int xInput = 10;
        int yInput = 55;

        int xHidden = 70;
        int yHidden = 45;

        int xOutput = 140;
        int yOutput = 80;

        for (int z = 0; z < 4; z++)
        {
            for (int i = 0; i < 3; i++)
            {
                UnityEditor.Handles.DrawLine(ScreenToWorld(xInput, yInput + 5), ScreenToWorld(xHidden, yHidden + 5));
                yInput = yInput + 25;
            }
            yHidden = yHidden + 25;
            yInput = 55;
        }
         yInput = 55;
        yHidden = 45;
        for (int i = 0; i < 3; i++)
        {
            UnityEditor.Handles.DrawLine(ScreenToWorld(xInput, yInput + 5), ScreenToWorld(xHidden, yHidden + 5));
            rect = ScreenRect(xInput, yInput, 10, 10);
            UnityEditor.Handles.DrawSolidRectangleWithOutline(rect, Color.green, Color.white);
            yInput = yInput + 25;
        }

        for (int z = 0; z < 4; z++)
        {
            UnityEditor.Handles.DrawLine(ScreenToWorld(xHidden, yHidden + 5), ScreenToWorld(xOutput, yOutput + 5));
            rect = ScreenRect(xHidden, yHidden, 10, 10);
            UnityEditor.Handles.DrawSolidRectangleWithOutline(rect, Color.grey, Color.white);
            yHidden = yHidden + 25;
        }
        rect = ScreenRect(xOutput, yOutput, 10, 10);
        UnityEditor.Handles.DrawSolidRectangleWithOutline(rect, Color.red, Color.white);
    }
}