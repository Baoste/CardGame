using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BrightnessPostProcessing : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public Material blitMaterial;
        public RenderPassEvent passEvent = RenderPassEvent.AfterRendering;
    }

    public Settings settings = new Settings();

    class CustomPostProcessPass : ScriptableRenderPass
    {
        private Material blitMaterial;
        private string profilerTag = "Custom PostProcess Pass";

        // ͨ�� int �� RenderTargetIdentifier ��ϴ��� RenderTargetHandle
        private int tempRTId = Shader.PropertyToID("_TempColorTex");
        private RenderTargetIdentifier tempRTIdentifier;

        public CustomPostProcessPass(Material mat, RenderPassEvent passEvent)
        {
            this.blitMaterial = mat;
            this.renderPassEvent = passEvent;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            if (blitMaterial == null) return;

            // ��ȡCamera����
            var cameraDesc = renderingData.cameraData.cameraTargetDescriptor;
            cameraDesc.depthBufferBits = 0;  // ������ȵĺ���RT����ô��

            // ������ʱRT
            cmd.GetTemporaryRT(tempRTId, cameraDesc, FilterMode.Bilinear);
            tempRTIdentifier = new RenderTargetIdentifier(tempRTId);

        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (blitMaterial == null) return;

            var cmd = CommandBufferPool.Get(profilerTag);

            // �� Pass �л�ȡ cameraColorTarget�������� AddRenderPasses ��ȡ��
            var sourceHandle = renderingData.cameraData.renderer.cameraColorTargetHandle;
            var source = sourceHandle.rt;

            // 1) ��Դ���ݿ�������ʱRT
            cmd.Blit(source, tempRTIdentifier);
            // 2) ����ʱRTʹ�ò��� Blit ��Դ
            cmd.Blit(tempRTIdentifier, source, blitMaterial);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            // �ͷ���ʱRT
            if (blitMaterial != null)
            {
                cmd.ReleaseTemporaryRT(tempRTId);
            }
        }
    }

    CustomPostProcessPass m_Pass;

    public override void Create()
    {
        if (settings.blitMaterial == null)
            Debug.LogWarning("Blit Material is null. The pass will do nothing.");

        m_Pass = new CustomPostProcessPass(settings.blitMaterial, settings.passEvent);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // �������Ϊ�գ�������
        if (settings.blitMaterial == null)
            return;

        // ���ֻ���� Game �������ִ�У��ɼ��ж�
        if (renderingData.cameraData.cameraType != CameraType.Game) return;

        // ������Ⱦ����
        renderer.EnqueuePass(m_Pass);
    }
}
