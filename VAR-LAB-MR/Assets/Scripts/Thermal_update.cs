using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class termal : MonoBehaviour
{
    public Terrain terrain;
    public float T_THERMAL = 0.01f;
    public float C_THERMAL = 0.01f;
    private float timeCounter = 0.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeCounter += Time.deltaTime;

        if (timeCounter >= 1.0f)
        {
            // Realiza la erosión térmica
            thermalErosion(terrain);

            // Restablece el contador de tiempo
            timeCounter = 0.0f;
        }
    }
    
    void thermalErosion (Terrain terrain)
    {
        TerrainData terrainData = terrain.terrainData;
        int resolution = terrainData.heightmapResolution;
        float[,] h = terrainData.GetHeights(0, 0, resolution, resolution);
        for (int i = 0; i < resolution; i++)
        {
            for (int j = 0; j < resolution; j++)
            {
                float[] d = new float[4];
                d[0] = (i > 0) ? h[i, j] - h[i - 1, j] : 0;  // Izquierda
                d[1] = (j > 0) ? h[i, j] - h[i, j - 1] : 0;  // Arriba
                d[2] = (j < resolution - 1) ? h[i, j] - h[i, j + 1] : 0;  // Abajo
                d[3] = (i < resolution - 1) ? h[i, j] - h[i + 1, j] : 0;  // Derecha

                float d_total = 0.0f;
                for (int k = 0; k < 4; k++)
                    if (d[k] > T_THERMAL)
                        d_total += d[k];

                float dmax = 0.0f;
                for (int k = 0; k < 4; k++)
                    if (dmax < d[k])
                        dmax = d[k];

                if (d_total > 0)
                {
                    h[i, j] -= C_THERMAL * (dmax - T_THERMAL);
                    if (i > 0 && d[0] > T_THERMAL) h[i - 1, j] += C_THERMAL * (dmax - T_THERMAL) * (d[0] / d_total);
                    if (j > 0 && d[1] > T_THERMAL) h[i, j - 1] += C_THERMAL * (dmax - T_THERMAL) * (d[1] / d_total);
                    if (j < resolution - 1 && d[2] > T_THERMAL) h[i, j + 1] += C_THERMAL * (dmax - T_THERMAL) * (d[2] / d_total);
                    if (i < resolution - 1 && d[3] > T_THERMAL) h[i + 1, j] += C_THERMAL * (dmax - T_THERMAL) * (d[3] / d_total);
                }
            }
        }

        terrainData.SetHeights(0, 0, h);
    }
}
