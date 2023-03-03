using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace AnimeStylePostProcess
{
    [Serializable]
    public class FlareParaPassSettings
    {
        public Material material;
        // public bool rotateWithMainLight;
    }

    public class FlareAndParaRendererFeature : ScriptableRendererFeature
    {
        public FlareParaPassSettings settings = new();
        FlareAndParaPass _flareAndParaPass;
        
        public override void Create()
        {
            _flareAndParaPass = new FlareAndParaPass(settings);

            // Configures where the render pass should be injected.
            _flareAndParaPass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
        }

        // Here you can inject one or multiple render passes in the renderer.
        // This method is called when setting up the renderer once per-camera.
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(_flareAndParaPass);
        }
    }
}



