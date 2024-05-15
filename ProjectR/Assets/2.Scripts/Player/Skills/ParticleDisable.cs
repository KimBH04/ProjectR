using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDisable : MonoBehaviour
{
    private void OnParticleSystemStopped()
    {
        transform.parent.gameObject.SetActive(false);
    }
}
