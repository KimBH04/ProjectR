using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    public float damage;
    
    private float _moveSpeed;
    private float _alphaSpeed;
    private float _destroyTime;
    

    private TextMeshPro _text;
    private Color _alphaColor;

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        transform.Translate(new Vector3(0,_moveSpeed * Time.deltaTime,0));
        
        _alphaColor.a = Mathf.Lerp(_alphaColor.a,0,_alphaSpeed * Time.deltaTime);
        _text.color = _alphaColor;

    }

    private void Init()
    {
        _moveSpeed = 2.0f;
        _alphaSpeed = 2.0f;
        _destroyTime = 2.0f;

        _text = GetComponent<TextMeshPro>();
        _alphaColor = _text.color;
        _text.text = damage.ToString();
        
        Invoke(nameof(DestroyObject),_destroyTime);

    }
    
    private void DestroyObject()
    {
        Destroy(gameObject);
    }
        
}
