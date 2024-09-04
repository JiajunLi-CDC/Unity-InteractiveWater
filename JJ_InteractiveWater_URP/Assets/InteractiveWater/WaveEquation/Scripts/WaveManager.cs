using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public Material wavematerial;
    public Texture2D waveTexture;
    public bool reflectiveBoundary = false;

    private float[][] waveN, waveNm1, waveNp1;  //状态信息
    
    private float Lx = 10;  //width
    private float Ly = 10;  //height
    
    [SerializeField]private float dx = 0.1f;   //x轴密度
    private float dy     //y轴密度
    {
        get => dx;
    }
    private int nx, ny; //分辨率

    public float CFL = 0.5f;
    public float c = 1;
    private float dt;  //时间步长
    private float t;  //当前时间
    [SerializeField] private float FloatToColorItensity = 2.0f;
    [SerializeField] private float  pulseFrequency = 1.0f;
    [SerializeField] private float  pulseMagnitude = 1.0f;

    [SerializeField] private Vector2Int pulsePosition = new Vector2Int(0,0);
    [SerializeField] private float elasticity = 0.98f;  //摩擦力，控制水平消散

    private void Start()
    {
        //计算图片分辨率
        nx = Mathf.FloorToInt(Lx / dx);
        ny = Mathf.FloorToInt(Ly / dy);   
        waveTexture = new Texture2D(nx, ny, TextureFormat.RGBA32, false);
        Debug.Log("nx="+nx+".....ny="+ny);

        //初始化
        waveN = new float[nx][];
        waveNm1 = new float[nx][];
        waveNp1 = new float[nx][];
        for (int i = 0; i < nx; i++)
        {
            waveN[i] = new float[ny];
            waveNm1[i] = new float[ny];
            waveNp1[i] = new float[ny];
        }
        
        wavematerial.SetTexture("_MainTex",waveTexture);  //Coloring
        wavematerial.SetTexture("_DisplacementTex",waveTexture);  //displacement texture
    }

    private void Update()
    {
        MousePositionOnTex( ref pulsePosition );
        WaveStep();
        ApplyMatricToTxture(waveN, ref waveTexture, FloatToColorItensity );
    }

    void ApplyMatricToTxture(float[][] state, ref Texture2D tex ,float  floatToColorItensity )
    {
        for (int i = 0; i < nx; i++)
        {
            for (int j = 0; j < ny; j++)
            {
                float val = waveN[i][j] * floatToColorItensity  + 0.5f;  //当前传播到下一刻
                tex.SetPixel(i,j,new Color(val,val,val,1f));
            }
        }
        tex.Apply();
    }
    
    void WaveStep()
    {
        dt = CFL * dx / c;  //重新计算dt
        t += dx;    //增加时间

        if (reflectiveBoundary)
        {
            ApplyReflectiveBoundary();
        }
        else
        {   
            ApplyAbsorptiveBoundary();
        }
        
        for (int i = 0; i < nx; i++)
        {
            for (int j = 0; j < ny; j++)
            {
                waveNm1[i][j] = waveN[i][j];  //当前传播到下一刻
                waveN[i][j] = waveNp1[i][j];  //上一刻传递到当前
            }
        }
        
        //添加波动
        waveN[pulsePosition.x][pulsePosition.y] = dt * dt * 20 * pulseMagnitude * Mathf.Cos(t * Mathf.Rad2Deg * pulseFrequency);
        
        //核心计算
        for (int i = 1; i < nx-1; i++)
        {
            for (int j = 1; j < ny-1; j++)
            {
                float n_ij = waveN[i][j];
                float n_ip1j = waveN[i+1][j];
                float n_im1j = waveN[i-1][j];
                float n_ijp1 = waveN[i][j+1];
                float n_ijm1 = waveN[i][j-1];
                
                float nm1_ij = waveNm1[i][j];

                //波动方程
                waveNp1[i][j] = 2f * n_ij - nm1_ij + CFL * CFL * (n_ijm1 + n_ijp1 + n_im1j + n_ip1j - 4f * n_ij);
                waveNp1[i][j] *= elasticity;
            }
        }
    }


    void ApplyAbsorptiveBoundary()
    {
        float v = (CFL - 1f) / (CFL + 1f);
        for (int i = 0; i < nx; i++)
        {
            waveNp1[i][0] =  waveN[i][1] + v*(waveNp1[i][1]-waveN[i][0]);  
            waveNp1[i][ny-1] = waveN[i][ny-2] + v*(waveNp1[i][ny-2]-waveN[i][ny-1]);  
        }
        
        for (int j = 0; j < ny; j++)
        {
            waveNp1[0][j] =  waveN[1][j] + v*(waveNp1[1][j]-waveN[0][j]);  
            waveNp1[nx-1][j] = waveN[nx-2][j] + v*(waveNp1[nx-2][j]-waveN[nx-1][j]);  
        }
    }
    
    void ApplyReflectiveBoundary()
    {
        for (int i = 0; i < nx; i++)
        {
            waveN[i][0] = 0;  
            waveN[i][ny-1] = 0;  
        }
        
        for (int j = 0; j < ny; j++)
        {
            waveN[0][j] = 0;  
            waveN[nx-1][j] = 0;  
        }
    }
    
    void MousePositionOnTex(ref Vector2Int pos)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            pos =  new Vector2Int((int)(hit.textureCoord.x * nx), (int)(hit.textureCoord.y * ny));
        }
    }
}
