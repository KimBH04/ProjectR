using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDisable : MonoBehaviour
{
    private new SphereCollider collider;

    private void Awake()
    {
        collider = GetComponent<SphereCollider>();
    }

    private void OnEnable()
    {
        collider.enabled = true;
    }

    private void OnParticleSystemStopped()
    {
        transform.parent.gameObject.SetActive(false);
    }
}
