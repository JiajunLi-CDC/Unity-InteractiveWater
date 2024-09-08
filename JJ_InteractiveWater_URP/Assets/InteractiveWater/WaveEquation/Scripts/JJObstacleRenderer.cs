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
    public Material HumanPosMaterial;
    public Material TrailMaterial;
    public Material DebugMaterial;
    public Camera targetCamera;
    public int RenderTextureSize = 256;
    private ObstacleRenderPass _obstacleRenderPass;

    public GameObject HumanController;

    private Vector3 previousPosition; // 保存上一帧的位置
    private Vector3 currentPosition; // 当前帧的位置

    void OnEnable()
    {
        _obstacleRenderPass = new ObstacleRenderPass();
        _obstacleRenderPass.m_ProfilerTag = this.m_ProfilerTag;
        _obstacleRenderPass.LayerMask = this.LayerMask;
        _obstacleRenderPass.QueueMin = this.QueueMin;
        _obstacleRenderPass.QueueMax = this.QueueMax;
        _obstacleRenderPass.PassEvent = this.PassEvent;
        _obstacleRenderPass.overrideMaterial = this.HumanPosMaterial;
        _obstacleRenderPass.RenderTextureSize = this.RenderTextureSize;
        _obstacleRenderPass.TrailMaterial = this.TrailMaterial;
        _obstacleRenderPass.DebugMaterial = this.DebugMaterial;

        previousPosition = HumanController.transform.position; // 初始化位置

        RenderPipelineManager.beginCameraRendering += OnBeginCamera;
    }

    private void Update()
    {
        currentPosition = HumanController.transform.position;

        // 计算 HumanController 的速度（基于位置变化）
        float cha = (currentPosition - previousPosition).magnitude;
        Vector3 velocity = new Vector3(0,0,0);
        if (cha > 0.01)
        {
            velocity = (currentPosition - previousPosition) / Time.deltaTime;
            _obstacleRenderPass.speed = velocity.magnitude; // 获取速度大小并传入渲染器
        }
        else
        {
            velocity = new Vector3(0,0,0);
            _obstacleRenderPass.speed = 0; // 获取速度大小并传入渲染器
        }
        
        // Debug.Log("速度大小为"+velocity.magnitude);
        // 更新上一帧的位置
        previousPosition = currentPosition;
        

        // 更新其他属性
        _obstacleRenderPass.m_ProfilerTag = this.m_ProfilerTag;
        _obstacleRenderPass.LayerMask = this.LayerMask;
        _obstacleRenderPass.overrideMaterial = this.HumanPosMaterial;
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

