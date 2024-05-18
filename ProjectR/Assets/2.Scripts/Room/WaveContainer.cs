using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wave Container")]
public class WaveContainer : ScriptableObject
{
    [SerializeField] private GameObject[] monsters;
    
    public GameObject this[int index] => monsters[index];   // 1
    public int Count => monsters.Length;
}
