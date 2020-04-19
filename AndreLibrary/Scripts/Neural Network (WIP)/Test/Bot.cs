using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Bot : MonoBehaviour
{
    public float speed;//Speed Multiplier
    public float rotation;//Rotation multiplier
    public LayerMask raycastMask;//Mask for the sensors

    private double[] input = new double[5];//input to the neural network
    public NeatNeuralNetwork network;

    public float position;//Checkpoint number on the course
    public bool collided;//To tell if the car has crashed
    public float numberOfTurnsPenalty = 0;
    public float turnPenalty = 0.01f;

    void FixedUpdate()//FixedUpdate is called at a constant interval
    {
        if (!collided)//if the car has not collided with the wall, it uses the neural network to get an output
        {
            for (int i = 0; i < 5; i++)//draws five debug rays as inputs
            {
                Vector3 newVector = Quaternion.AngleAxis(i * 45 - 90, new Vector3(0, 1, 0)) * transform.right;//calculating angle of raycast
                RaycastHit hit;
                Ray Ray = new Ray(transform.position, newVector);

                if (Physics.Raycast(Ray, out hit, 10, raycastMask))
                {
                    input[i] = (10 - hit.distance) / 10;//return distance, 1 being close
                }
                else
                {
                    input[i] = 0;//if nothing is detected, will return 0 to network
                }
            }

            double[] output = network.Compute(input);//Call to network to feedforward

            if ((float)output[0] > 0.1 || (float)output[0] < -0.1)
                numberOfTurnsPenalty = numberOfTurnsPenalty + turnPenalty;
            transform.Rotate(0, (float)output[0] * rotation, 0, Space.World);//controls the cars movement
            transform.position += this.transform.right * (float)output[1] * speed;//controls the cars turning
        }
    }


    void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.gameObject.layer == LayerMask.NameToLayer("CheckPoint"))//check if the car passes a gate
        {
            GameObject[] checkPoints = GameObject.FindGameObjectsWithTag("CheckPoint");
            for (int i=0; i < checkPoints.Length; i++)
            {
                if(collision.collider.gameObject == checkPoints[i] && i == (position + 1 + checkPoints.Length) % checkPoints.Length)
                {
                    position++;//if the gate is one ahead of it, it increments the position, which is used for the fitness/performance of the network
                    break;
                }
            }
        }
        else if(collision.collider.gameObject.layer != LayerMask.NameToLayer("Learner"))
        {
            collided = true;//stop operation if car has collided
        }
    }

    public float UpdateFitness()
    {
        if (collided)
            position = position - 1;
        position = position - numberOfTurnsPenalty;
        network.genome.fitness = position;//updates fitness of network for sorting
        return position;
    }
}
