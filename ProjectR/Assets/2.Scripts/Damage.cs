using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
   [SerializeField] private int damage;
   
   private void OnTriggerEnter(Collider other)
   {
      if (other.CompareTag("Player"))
      {
         other.GetComponent<PlayerController>().Hp -= damage;
         AudioManager.Instance.PlaySfx(AudioManager.ESfx.TakeDamage);
      }
   }
    
}
