using UnityEngine;

[CreateAssetMenu(fileName = "Wave Container")]
public class WaveContainer : ScriptableObject
{
    [SerializeField] private string[] monsterNames;
    
    public string this[int index] => monsterNames[index];   // 1
    public int Count => monsterNames.Length;
}