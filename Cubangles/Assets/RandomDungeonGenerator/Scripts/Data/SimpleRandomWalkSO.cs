using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SimpleRandomWalkParameters",menuName = "PCG/Simple RandomWalk Data")]
public class SimpleRandomWalkSO : ScriptableObject
{
    public int iterations = 10 , walkLength = 10;
    public bool startRandomlyEachIteration = true;
}
