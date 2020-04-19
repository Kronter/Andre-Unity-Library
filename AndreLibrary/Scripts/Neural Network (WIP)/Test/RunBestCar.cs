using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunBestCar : MonoBehaviour
{
    [Range(0.1f, 10f)] public float Gamespeed = 1f;
    Bot Car;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Car == null)
            return;
        Time.timeScale = Gamespeed;
        if (Car.collided)
        {
            Car.transform.position = new Vector3(0, 1.6f, -16);
            Car.transform.rotation = new Quaternion(0, 0, 1, 0);
            Car.collided = false;
        }
    }
    public void SpawnBestCar(GameObject prefab, NeatNeuralNetwork network)
    {
        Car = (Instantiate(prefab, new Vector3(0, 1.6f, -16), new Quaternion(0, 0, 1, 0))).GetComponent<Bot>();//create botes
        Car.network = network;
        Car.gameObject.name = "SimplyTheBest";
        Car.GetComponent<MeshRenderer>().material.color = Color.green;
    }
}
