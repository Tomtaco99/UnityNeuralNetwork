using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class NN_Sensor : MonoBehaviour
{

    [SerializeField] public NN_Neuron[] inputs;
    [SerializeField] public NN_Neuron[] outputs;
    [SerializeField] public CarPhysics carController;


    [SerializeField] int lm;


    [SerializeField] public float score;
    [SerializeField] public float hitPenalty = 125;
    [SerializeField] public float frictionPenalty = 100;
    [SerializeField] public float collisions;

    private Vector3 previosPos;
    private Vector3 currentPos;

    public int progress;
    public float tiempoTocando;

    float velocidadMedia;
    float tiempofrenando;
    float steps;
    float laps;
    private void Start()
    {
        lm=LayerMask.GetMask("Circuit");
        score = 0;
        collisions = 0;
        tiempoTocando = 0;
        currentPos = previosPos = transform.position;
        //Asegurarse de que pasamos el primer checkpoint ok
        progress = -1;
        laps = 0;
        //brakeTime = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        currentPos = transform.position;
        /*Set neural network input*/
        float[] distances = new float[inputs.Length];
        for(int c =0;c<inputs.Length;c++)
        {
            distances[c] = 1;
            RaycastHit rch;
            bool hit = Physics.Raycast(transform.position,transform.forward+(transform.right*(c-4.0f+0.5f)),out rch,10, lm, QueryTriggerInteraction.Ignore);
            if (hit)
            {
                distances[c] = rch.distance / 10;
            }
            inputs[c].SetInput(distances[c]);
        }


        /*Evaluate output*/

        carController.SetThrust(outputs[0].GetOutput());
        carController.SetBrake(outputs[1].GetOutput());
        carController.SetSteering(outputs[2].GetOutput());

        

        /*Compute grade*/
        CalculateScore();
        previosPos = currentPos;
    }
    
    //suma puntuacion segun la distancia que recorre
    public void CalculateScore()
    {
        score += Vector3.Distance(currentPos, previosPos);
        velocidadMedia += outputs[0].GetOutput();
        tiempofrenando += outputs[1].GetOutput();
        steps++;
    }

    public float GetScore()
    {
        if (progress < 0)
        {
            return -9999999999999999;
        }
        score -= ((collisions * hitPenalty) + (tiempoTocando * frictionPenalty));
        if (collisions == 0 && laps != 0)
        {
            score += 2000;
            score += velocidadMedia / steps * 2500;
        }
        score += tiempofrenando / steps * 100;
        return score;
    }

    public void ManageCheckPoint(Checkpoint c)
    {
        if (c.order > progress + 2 || c.order < progress)
        {
            //direccion contraria
            score -= 1000;
        }
        else if (progress == c.order - 1)
        {
            progress = c.order;
            score += 500;
        }
        if(progress == c.total && c.order == 0) 
        {
            progress = c.order;
            score += c.total*650;
            laps++;
        }
    }

    public void Choque()
    {
        collisions++;
    }

    public void TocandoPared() 
    {
        tiempoTocando += Time.fixedDeltaTime;
    }

    //pasar por checkpoints suma mas puntuacion todavia
    
}
