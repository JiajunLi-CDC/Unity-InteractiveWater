using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class WaveteComputeManager : MonoBehaviour
{
    public ComputeShader waveCompute;
    public Material waveMaterial;
    public RenderTexture NState, Nm1State, Np1State, OffsetState01,OffsetState02;
   
    public Vector2Int resolution; //分辨率
    public Vector3 effect; //x coord,y coord, strength
    public float dispersion = 0.98f;
    
    public RenderTexture HumanPositionTex;
    public RenderTexture ObstacleTex;

    public Vector2 uvOffset = new Vector2(0, 0);
    
    private void Start()
    {
        InitializeTexture(ref NState);
        InitializeTexture(ref Nm1State);
        InitializeTexture(ref Np1State);
        InitializeTexture(ref OffsetState01);
        InitializeTexture(ref OffsetState02);
        ObstacleTex.enableRandomWrite = true;
        HumanPositionTex.enableRandomWrite = true;
            
        if (ObstacleTex.width != resolution.x || ObstacleTex.height != resolution.y)
        {
            Debug.Log("需要调整障碍物RT分辨率");
        }
        if (HumanPositionTex.width != resolution.x || HumanPositionTex.height != resolution.y)
        {
            Debug.Log("需要调整人物轨迹RT分辨率");
        }
        waveMaterial.SetTexture("_MainTex",NState);
    }

    private void InitializeTexture(ref RenderTexture tex)
    {
        tex = new RenderTexture(resolution.x, resolution.y, 1, GraphicsFormat.R16G16B16A16_SNorm);
        tex.enableRandomWrite = true;
        tex.Create();
    }

    private void Update()
    {
        Graphics.CopyTexture(OffsetState01,Np1State);
        Graphics.CopyTexture(OffsetState02,NState);
        Graphics.CopyTexture(NState,Nm1State);
        Graphics.CopyTexture(Np1State,NState);

        Vector2 pos = Vector2.one;
        MousePositionOnTex(ref pos);
        effect.x = pos.x;
        effect.y = pos.y;
         
        for(int i=0;i<2;i++)
        {
            waveCompute.SetTexture(i, "NState", NState);
            waveCompute.SetTexture(i, "Nm1State", Nm1State);
            waveCompute.SetTexture(i, "Np1State", Np1State);
            waveCompute.SetTexture(i, "ObstacleTex", ObstacleTex);
            waveCompute.SetTexture(i, "HumanPositionTex", HumanPositionTex);
            waveCompute.SetTexture(i, "OffsetState01", OffsetState01);
            waveCompute.SetTexture(i, "OffsetState02", OffsetState02);
        }
        waveCompute.SetVector("effect",effect);
        waveCompute.SetFloat("dispersion",dispersion);
        waveCompute.SetVector("pixalOffset", uvOffset*resolution);
        waveCompute.SetVector("resolution",new Vector2(resolution.x,resolution.y));
        
        waveCompute.Dispatch(0,resolution.x/8,resolution.y/8,1);
        
        waveCompute.Dispatch(1,resolution.x/8,resolution.y/8,1);
        
    }
    
    void MousePositionOnTex(ref Vector2 pos)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            pos =  new Vector2((int)(hit.textureCoord.x * resolution.x), (int)(hit.textureCoord.y * resolution.y));
        }
    }
}