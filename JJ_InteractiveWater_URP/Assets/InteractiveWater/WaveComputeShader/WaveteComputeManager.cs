using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class WaveteComputeManager : MonoBehaviour
{
    public ComputeShader waveCompute;

    public RenderTexture NState, Nm1State, Np1State;
    public Vector2Int resolution; //分辨率

    private void Start()
    {
        InitializeTexture(ref NState);
        InitializeTexture(ref Nm1State);
        InitializeTexture(ref Np1State);
    }

    private void InitializeTexture(ref RenderTexture tex)
    {
        tex = new RenderTexture(resolution.x, resolution.y, 1, GraphicsFormat.R16G16B16A16_SNorm);
        tex.enableRandomWrite = true;
        tex.Create();
    }

    private void Update()
    {
        waveCompute.SetTexture(0, "NState", NState);
        waveCompute.SetTexture(0, "Nm1State", Nm1State);
        waveCompute.SetTexture(0, "Np1State", Np1State);
        waveCompute.SetInts("resolution", new int[] { resolution.x, resolution.y });
        
        waveCompute.Dispatch(0,resolution.x/8,resolution.y/8,1);
    }
}