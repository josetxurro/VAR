using UnityEngine;

public class SnowAccumulation : MonoBehaviour
{
    public Material[] snowMaterials;  // Asigna los materiales con el shader aquí
    public float snowFactorIncrement = 0.008f;
    public float maxSnowFactor = 0.7f;

    private void Start()
    {
        InvokeRepeating("IncreaseSnowFactor", 0f, 0.5f);
    }

    private void IncreaseSnowFactor()
    {
        foreach (Material snowMaterial in snowMaterials)
        {
            if (snowMaterial != null)
            {
                float newSnowFactor = Mathf.Lerp(snowMaterial.GetFloat("_SnowFactor"), maxSnowFactor, snowFactorIncrement);
                snowMaterial.SetFloat("_SnowFactor", newSnowFactor);
            }
        }
    }
}