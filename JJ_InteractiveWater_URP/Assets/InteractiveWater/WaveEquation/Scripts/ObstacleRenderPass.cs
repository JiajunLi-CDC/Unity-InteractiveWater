using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ObstacleRenderPass : ScriptableRenderPass
{
    public string m_ProfilerTag = "ObstacleRenderer";
    public LayerMask LayerMask = 1;
    public RenderPassEvent PassEvent = RenderPassEvent.AfterRenderingSkybox;
    public Material overrideMaterial; // 指定的材质
    public Material TrailMaterial; // 指定的材质
    public Material DebugMaterial;
    public int RenderTextureSize = 256;
    public float speed;
    public Vector2 uvOffset;

    [Range(1000, 5000)] public int QueueMin = 2000;
    [Range(1000, 5000)] public int QueueMax = 5000;

    private FilteringSettings filter;
    
    private RenderTargetIdentifier Source;
    private RenderTexture HumanTrailTex;
    private RenderTexture HumanPosTex;

    [System.Serializable]
    public class Settings
    {

   
        public int RenderTextureSize = 256;

    }
    
    public  ObstacleRenderPass()
    {
        HumanPosTex = new RenderTexture(RenderTextureSize, RenderTextureSize, 0, RenderTextureFormat.RFloat);
        HumanTrailTex = new RenderTexture(RenderTextureSize, RenderTextureSize, 0, RenderTextureFormat.RFloat);
    }
    
    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        // 创建过滤器，基于队列和层
        RenderQueueRange queue = new RenderQueueRange
        {
            lowerBound = QueueMin,
            upperBound = QueueMax
        };
        filter = new FilteringSettings(queue, LayerMask);

        // 设置 RenderPassEvent
        this.renderPassEvent = PassEvent;

        Source = renderingData.cameraData.renderer.cameraColorTarget;
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);

        // 每帧获取相机的颜色缓冲区和深度缓冲区
        cmd.SetRenderTarget(Source); // 存储深度缓冲区

        // 清除颜色缓冲区，但不清除深度缓冲区
        cmd.ClearRenderTarget(false, true, Color.clear); // 只清除颜色缓冲区，保留深度缓冲区
        // 设置绘制设置
        DrawingSettings drawingSettings = CreateDrawingSettings(new ShaderTagId("UniversalForward"), ref renderingData, renderingData.cameraData.defaultOpaqueSortFlags);

        // 使用覆盖材质
        if (overrideMaterial != null)
        {
            drawingSettings.overrideMaterial = overrideMaterial;
        }
        
        // 执行渲染
        context.ExecuteCommandBuffer(cmd);
        context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filter);
        
        
        //-------------------------------------------------------------------------------
        CommandBuffer cmd02 = CommandBufferPool.Get("TrailDrawer");
        cmd02.Blit(Source,HumanPosTex);  //copy
        TrailMaterial.SetFloat("_HumanSpeed",speed);
        DebugMaterial.SetTexture("_HumanPosTex", HumanPosTex);
        TrailMaterial.SetTexture("_HumanPosTex", HumanPosTex);
        TrailMaterial.SetVector("_uvOffset", uvOffset);
        
        cmd02.Blit(HumanTrailTex,Source,TrailMaterial);
        
        cmd02.Blit(Source,HumanTrailTex);
        context.ExecuteCommandBuffer(cmd02);
        
        // 释放命令缓冲区
        CommandBufferPool.Release(cmd);
        CommandBufferPool.Release(cmd02);
    }




    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        // 不清理 HumanTrailTex，只清理 HumanPosTex
        if (HumanPosTex != null)
        {
            HumanPosTex.Release();
        }
    }
}
