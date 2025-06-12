using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointManager : MonoBehaviour
{

    Checkpoint[] checkpoints = null;
    public int CheckPointsNum;
    // Start is called before the first frame update
    void Start()
    {
        checkpoints = GetComponentsInChildren<Checkpoint>();
        int index = 0;
        int total = checkpoints.Length;
        foreach (Checkpoint checkpoint in checkpoints)
        {
            checkpoint.order = index;
            checkpoint.total = total;
            index++;
        }
        CheckPointsNum = index;
    }

    // Update is called once per frame
    void Update()
    {

    }
    
}
