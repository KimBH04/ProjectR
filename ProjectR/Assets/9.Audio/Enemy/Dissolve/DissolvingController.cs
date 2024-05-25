using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolvingController : MonoBehaviour
{
    public MeshRenderer ObjectMesh;                           //지금은 Cube를 사용하고 있어서 사용 
    public SkinnedMeshRenderer SkinnedMeshRenderer;     //캐릭터 or 커스텀 오브젝트 사용할때

    public float dissolveRate = 0.0125f;
    public float refreshRate = 0.025f;

    public Material[] ObjectMeshs;                      //여러개의 Material 를 사용 하는 경우 

    // Start is called before the first frame update
    void Start()
    {
        if(ObjectMesh != null)
        {
            ObjectMeshs = ObjectMesh.materials;             //가지고 있는 메테리얼 리스트를 넣는다. 
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            StartCoroutine(CoDissolve());
        }
        
    }

    IEnumerator CoDissolve()
    {
        if(ObjectMeshs.Length > 0)
        {
            float counter = 0;

            while (ObjectMeshs[0].GetFloat("_DissolveAmount") < 1)
            {
                counter += dissolveRate;
                for(int i = 0; i < ObjectMeshs.Length; i++)
                {
                    ObjectMeshs[i].SetFloat("_DissolveAmount", counter);
                }

                yield return new WaitForSeconds(refreshRate);
            }
        }
    }
}
