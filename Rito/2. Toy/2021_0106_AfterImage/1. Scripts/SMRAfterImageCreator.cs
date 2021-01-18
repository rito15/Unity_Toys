using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMRAfterImageCreator : MonoBehaviour
{
    [SerializeField]
    Material afterImageMaterial;

    SkinnedMeshRenderer smr;
    Afterimage[] afterImages;

    int afterImageCount;
    int currentAfterImageIndex;
    float remainAfterImageTime;
    float createAfterImagedelay;
    Coroutine createAfterImageCoroutine = null;

    public bool isCreating = false;
    public float colorTransitionSpeed = 0.01f;

    public void Setup(SkinnedMeshRenderer smr, int maxNumber, float remainTime, Material afterImageMat = null)
    {
        this.smr = smr;
        afterImageCount = maxNumber;
        remainAfterImageTime = remainTime;
        createAfterImagedelay = remainAfterImageTime / afterImageCount + 0.01f;

        if (afterImageMat != null)
            afterImageMaterial = afterImageMat;

        CreateAfterImages();
    }

    void CreateAfterImages()
    {
        GameObject parentGO = GameObject.Find("AfterImage Container");
        if (parentGO == null)
            parentGO = new GameObject("AfterImage Container");

        afterImages = new Afterimage[afterImageCount];
        for (int i = 0; i < afterImages.Length; ++i)
        {
            GameObject newObj = new GameObject("After Image Object");
            afterImages[i] = newObj.AddComponent<Afterimage>();
            afterImages[i].InitAfterImage(afterImageMaterial);

            newObj.transform.SetParent(parentGO.transform);
        }
    }

    public void Create()
    {
        isCreating = true;
        if (createAfterImageCoroutine == null)
            createAfterImageCoroutine = StartCoroutine(CreateAfterImageCoroution());
    }

    IEnumerator CreateAfterImageCoroution()
    {
        float t = 0f;
        float colorFactor = 0.0f;
        while (true)
        {
            t += Time.deltaTime;

            if (t >= createAfterImagedelay && isCreating)
            {
                float sineFactor = Mathf.Sin(colorFactor);
                sineFactor = sineFactor * 0.5f + 0.5f; // 0.0~1.0 범위로 리맵

                smr.BakeMesh(afterImages[currentAfterImageIndex].mesh);
                afterImages[currentAfterImageIndex].CreateAfterImage(transform.position, transform.rotation, remainAfterImageTime);

                afterImages[currentAfterImageIndex].material.SetFloat("_ColorFactor", sineFactor);

                currentAfterImageIndex = (currentAfterImageIndex + 1) % afterImageCount;
                t -= createAfterImagedelay;

                colorFactor += colorTransitionSpeed;
                //if (colorFactor > 1.0f) colorFactor = 0.0f;

                //Debug.Log(sineFactor);
            }
            yield return null;
        }

        //createAfterImageCoroutine = null;
    }
}