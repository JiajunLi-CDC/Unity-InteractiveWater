using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ObstacleRenderPass : ScriptableRenderPass
{
    public string m_ProfilerTag = "ObstacleRenderer";
    public LayerMask LayerMask = 1;
    public RenderPassEvent PassEvent = RenderPassEvent.AfterRenderingSkybox;
    public Material overrideMaterial; // 指定的材质

    [Range(1000, 5000)] public int QueueMin = 2000;
    [Range(1000, 5000)] public int QueueMax = 5000;

    private FilteringSettings filter;

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
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);

        // 每帧获取相机的颜色缓冲区和深度缓冲区
        cmd.SetRenderTarget(renderingData.cameraData.renderer.cameraColorTarget); // 存储深度缓冲区

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

        // 释放命令缓冲区
        CommandBufferPool.Release(cmd);
    }




    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        // 这里不需要清理 RenderTarget，因为我们直接写入相机的缓冲区
    }
}
