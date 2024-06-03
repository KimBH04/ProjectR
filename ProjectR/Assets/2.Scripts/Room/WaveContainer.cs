using System.Collections.Generic;
using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Wave Container")]
public class WaveContainer : ScriptableObject
{
    [SerializeField] private EnemyData[] monsterNames;
    
    public EnemyData[] Enemies => monsterNames;
    
    public EnemyData this[int index] => monsterNames[index]; 
    public int Count => monsterNames.Length;
    
}
