using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

using AnimeStylePostProcess.Utils;

namespace AnimeStylePostProcess
{
    public class FlareAndParaPass : ScriptableRenderPass
    {
        public FlareParaPassSettings Settings { get; }

        private Material _material;

        public FlareAndParaPass(FlareParaPassSettings settings)
        {
            Settings = settings;
            if (!settings.material)
            {
                _material = settings.material;
            }
        }


        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in a performant manner.
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType != CameraType.Game) return;

            var flareParaVolume = VolumeManager.instance.stack.GetComponent<FlareAndParaVolume>();
            
            if (!flareParaVolume.IsActive()) return;

            if (!_material) _material = CoreUtils.CreateEngineMaterial("Hidden/AnimeStylePostProcess/FlareAndPara");

           
            _material.SetFloat("_FlareIntensity", flareParaVolume.flareInstensity.value);
            _material.SetFloat("_FlareInteration", flareParaVolume.flareIteration.value);
            _material.SetColor("_FlareInnerColor", flareParaVolume.innerColor.value);
            _material.SetColor("_FlareOuterColor", flareParaVolume.outerColor.value);
            _material.SetFloat("_ColorMixedMidPoint", flareParaVolume.colorMixedMidPoint.value);
            _material.SetFloat("_ColorMixedSoftness", flareParaVolume.colorMixedSoftness.value);
            _material.SetFloat("_FlareRange", flareParaVolume.flareRange.value);
            

            var rotation = flareParaVolume.extraRotation.value;
            if (flareParaVolume.rotateWithMainLight.value)
            {
                // TODO ObjectParameter似乎有问题xd, 现在实际上只能获取主光源
                Light mainLight = flareParaVolume.targetLight == null ? RenderSettings.sun : flareParaVolume.targetLight;
                rotation += mainLight.GetLightScreenSpaceAngle();
            }
            _material.SetFloat("_RotateWithMainLightAngle", rotation);
            
            
            _material.SetFloat("_ParaIntensity", flareParaVolume.paraIntensity.value);
            _material.SetFloat("_ParaRange", flareParaVolume.paraRange.value);
            _material.SetFloat("_ParaRotation", flareParaVolume.paraRotation.value);


            CommandBuffer cmd = CommandBufferPool.Get();

            using (new ProfilingScope(cmd, new ProfilingSampler("AnimeStylePostProcess Flare/Para Pass")))
            {
                cmd.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);
                if (flareParaVolume.flareInstensity.value > 0)
                {
                    cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, _material, 0, 0);
                }

                if (flareParaVolume.paraIntensity.value > 0)
                {
                    cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, _material, 0, 1);
                }
                cmd.SetViewProjectionMatrices(renderingData.cameraData.camera.worldToCameraMatrix, renderingData.cameraData.camera.projectionMatrix);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }
    }
}