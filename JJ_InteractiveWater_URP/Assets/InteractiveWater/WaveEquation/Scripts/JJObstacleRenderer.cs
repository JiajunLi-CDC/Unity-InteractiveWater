using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[ExecuteAlways]
public class JJObstacleRenderer : MonoBehaviour
{
    public string m_ProfilerTag = "ObstacleRenderer";
    public LayerMask LayerMask = 1;
    [Range(1000, 5000)] public int QueueMin = 2000;
    [Range(1000, 5000)] public int QueueMax = 5000;
    public RenderPassEvent PassEvent = RenderPassEvent.AfterRenderingSkybox;
    public Material obstacleMaterial;
    public Material TrailMaterial;
    public Material DebugMaterial;
    public Camera targetCamera; // 新增：指定渲染的相机
    public int RenderTextureSize = 256;
    private ObstacleRenderPass _obstacleRenderPass;

    void OnEnable()
    {
        _obstacleRenderPass = new ObstacleRenderPass();
        _obstacleRenderPass.m_ProfilerTag = this.m_ProfilerTag;
        _obstacleRenderPass.LayerMask = this.LayerMask;
        _obstacleRenderPass.QueueMin = this.QueueMin;
        _obstacleRenderPass.QueueMax = this.QueueMax;
        _obstacleRenderPass.PassEvent = this.PassEvent;
        _obstacleRenderPass.overrideMaterial = this.obstacleMaterial;
        _obstacleRenderPass.RenderTextureSize = this.RenderTextureSize;
        _obstacleRenderPass.TrailMaterial = this.TrailMaterial;
        _obstacleRenderPass.DebugMaterial = this.DebugMaterial;

        RenderPipelineManager.beginCameraRendering += OnBeginCamera;
    }

    private void Update()
    {
        _obstacleRenderPass.m_ProfilerTag = this.m_ProfilerTag;
        _obstacleRenderPass.LayerMask = this.LayerMask;
        _obstacleRenderPass.overrideMaterial = this.obstacleMaterial;
        _obstacleRenderPass.QueueMin = this.QueueMin;
        _obstacleRenderPass.QueueMax = this.QueueMax;
        _obstacleRenderPass.PassEvent = this.PassEvent;
        _obstacleRenderPass.RenderTextureSize = this.RenderTextureSize;
        _obstacleRenderPass.TrailMaterial = this.TrailMaterial;
        _obstacleRenderPass.DebugMaterial = this.DebugMaterial;
    }

    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= OnBeginCamera;
    }

    private void OnBeginCamera(ScriptableRenderContext context, Camera camera)
    {
        if (targetCamera != null && camera != targetCamera)
        {
            return;
        }

        // 执行渲染
        camera.GetUniversalAdditionalCameraData().scriptableRenderer.EnqueuePass(_obstacleRenderPass);
    }
}