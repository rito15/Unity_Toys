using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 설명 : 
public class Afterimage : MonoBehaviour
{
    public Material material;
    MeshFilter mf = null;
    // GameObject afterImageObj = null;
    Coroutine fadeoutCoroutine = null;
    float originAlpha = 0f;
    public Mesh mesh { get { return mf.mesh; } }

    public void InitAfterImage(Material material)
    {
        // afterImageObj = new GameObject();
        MeshRenderer mr = gameObject.AddComponent<MeshRenderer>();

        this.material = new Material(material);
        originAlpha = this.material.GetFloat("_Alpha");
        mr.material = this.material;
        mf = gameObject.AddComponent<MeshFilter>();

        gameObject.SetActive(false);
    }

    public void CreateAfterImage(Vector3 position, Quaternion rot, float time)
    {
        if (fadeoutCoroutine == null)
        {
            gameObject.SetActive(true);
            gameObject.transform.position = position;
            gameObject.transform.rotation = rot;

            mf.mesh = mesh;
            fadeoutCoroutine = StartCoroutine(FadeOut(time));
        }
    }

    // 잔상 점점 사라지기
    IEnumerator FadeOut(float time)
    {
        while (time > 0f)
        {
            time -= Time.deltaTime;
            material.SetFloat("_Alpha", originAlpha * time);
            //material.color = new Color(material.color.r, material.color.g, material.color.b, originAlpha * time);
            yield return null;
        }

        gameObject.SetActive(false);
        fadeoutCoroutine = null;
    }
}
