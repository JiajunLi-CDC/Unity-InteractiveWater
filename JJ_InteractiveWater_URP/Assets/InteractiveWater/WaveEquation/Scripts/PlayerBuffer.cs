using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuffer : MonoBehaviour
{
    public Material TrailMaterial;
    public float TexWorldSize = 10f;

    struct MyPlayerStruct
    {
        public Vector3 objectPosition;
        public float speed;
        public Vector2 uvOffset;
    }

    private ComputeBuffer playerBuffer;
    private MyPlayerStruct[] _myPlayerStructs;
    private Transform[] _myTransforms;
    private Vector3[] _previousPositions;
    private int _PlayerNum;

    void Start()
    {
        _myTransforms = this.GetComponentsInChildren<Transform>();
        _PlayerNum = _myTransforms.Length;

        _myPlayerStructs = new MyPlayerStruct[_PlayerNum];
        _previousPositions = new Vector3[_PlayerNum];

        for (int i = 0; i < _PlayerNum; i++)
        {
            _previousPositions[i] = _myTransforms[i].position;
        }

        //playerBuffer = new ComputeBuffer(_PlayerNum, 24);
        
        int structSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(MyPlayerStruct));  //结构体大小
        playerBuffer = new ComputeBuffer(_PlayerNum, structSize);
        
        TrailMaterial.SetBuffer("_my_PlayerBuffer", playerBuffer);
    }

    private void Update()
    {
        for (int i = 0; i < _PlayerNum; i++)
        {
            Vector3 currentPosition = _myTransforms[i].position;
            
            Vector3 cha = currentPosition - _previousPositions[i];
            float dis = cha.magnitude;
      
            if (dis > 0.01)
            {
                Vector3 velocity = cha / Time.deltaTime;
                _myPlayerStructs[i].speed  = velocity.magnitude; // 获取速度大小并传入渲染器
                _myPlayerStructs[i].uvOffset = new Vector2(cha.x,cha.z) / TexWorldSize; // 获取uv偏移
            }
            else
            {
                _myPlayerStructs[i].speed = 0; // 获取速度大小并传入渲染器
                _myPlayerStructs[i].uvOffset = new Vector2(0,0);
            }

            _myPlayerStructs[i].objectPosition = currentPosition;
            _previousPositions[i] = currentPosition; // Update previous position
        }

        // Update the buffer with new data
        playerBuffer.SetData(_myPlayerStructs);
    }

    private void OnDestroy()
    {
        if (playerBuffer != null)
        {
            playerBuffer.Release();
        }
    }
}