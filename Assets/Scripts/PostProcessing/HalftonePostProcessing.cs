using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class HalftonePostProcessing : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public Material material;
        public RenderPassEvent passEvent = RenderPassEvent.AfterRenderingPostProcessing;
    }

    class SharpenPosterizePass : ScriptableRenderPass
    {
        private Material material;
        private RTHandle tempRT;

        public SharpenPosterizePass(Material material, RenderPassEvent passEvent)
        {
            this.material = material;
            this.renderPassEvent = passEvent;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            if (material == null) return;

            RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.depthBufferBits = 0;

            RenderingUtils.ReAllocateIfNeeded(ref tempRT, desc, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_HalftoneTempRT");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (material == null) return;
            if (renderingData.cameraData.isPreviewCamera) return;

            CommandBuffer cmd = CommandBufferPool.Get("Halftone Pass");

            RTHandle cameraColorTarget = renderingData.cameraData.renderer.cameraColorTargetHandle;

            Blitter.BlitCameraTexture(cmd, cameraColorTarget, tempRT);
            Blitter.BlitCameraTexture(cmd, tempRT, cameraColorTarget, material, 0);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public void Dispose()
        {
            tempRT?.Release();
        }
    }

    public Settings settings = new Settings();
    private SharpenPosterizePass pass;

    public override void Create()
    {
        pass = new SharpenPosterizePass(settings.material, settings.passEvent);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.material == null) return;
        renderer.EnqueuePass(pass);
    }

    protected override void Dispose(bool disposing)
    {
        pass?.Dispose();
    }
}
