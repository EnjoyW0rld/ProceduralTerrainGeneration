using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputeShaderTest : MonoBehaviour
{
    [SerializeField] private ComputeShader shader;
    [SerializeField] private RenderTexture renderTexture;

    private void Start()
    {
        renderTexture = new RenderTexture(256, 256,24);
        renderTexture.enableRandomWrite = true;
        renderTexture.Create();

        shader.SetTexture(0,"Result",renderTexture);
        shader.Dispatch(0, renderTexture.width / 8, 1, 1);
    }
}
