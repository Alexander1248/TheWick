using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ScreenScale : MonoBehaviour
{
    public float desiredHeight = 256;
    private UniversalRenderPipelineAsset pipelineAsset;

    void Start()
    {
        pipelineAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;

        float sclae = desiredHeight / Screen.height;
        Debug.Log(sclae);
        pipelineAsset.renderScale = sclae;
    }

}
