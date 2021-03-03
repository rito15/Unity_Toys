using UnityEngine;

[ExecuteInEditMode]
public class DepthRenderer : MonoBehaviour
{
    [Range(0f, 3f)]
    public float depthLevel = 1.0f;

    [Range(1f, 100f)]
    public float depthMul = 10.0f;

    private Shader _shader;
    private Shader RdShader
        => _shader != null ? _shader : (_shader = Shader.Find("Custom/RenderDepth"));

    private Material _material;
    private Material RdMaterial
    {
        get
        {
            if (_material == null)
            {
                _material = new Material(RdShader);
                _material.hideFlags = HideFlags.HideAndDontSave;
            }
            return _material;
        }
    }

    private void OnEnable()
    {
        if (RdShader == null || !RdShader.isSupported)
        {
            enabled = false;
            print("Shader " + RdShader.name + " is not supported");
            return;
        }

        Camera.main.depthTextureMode = DepthTextureMode.None;
    }

    private void OnDisable()
    {
        if (_material != null)
            DestroyImmediate(_material);
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (RdShader != null)
        {
            RdMaterial.SetFloat("_DepthLevel", depthLevel);
            RdMaterial.SetFloat("_DepthMul", depthMul);
            Graphics.Blit(src, dest, RdMaterial);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}