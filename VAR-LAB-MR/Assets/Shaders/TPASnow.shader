Shader "Custom/TPASnow"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        
        // Snow
        _Snow ("Snow", Range(0,1)) = 0.9
        _SnowFactor ("Snow Factor", Range(0,2)) = 0.05
        _SnowUmbral ("Umbral Snow", Range(0,1)) = 0.5
        _SnowTex ("Snow Texture", 2D) = "white" {}
        _TeselationLevel ("Tesselation Level", Int) = 1

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows tessellate:tess vertex:vert addshadow

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _SnowTex;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_VerticalTex;
            float3 worldPos;
            float3 worldNormal;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        half _SnowFactor;
        half _SnowUmbral;
        half _Snow;
        half _TeselationLevel;
    
        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        float tess() {
            return _TeselationLevel;
        }
        
        void vert(inout appdata_full v)
        {
            float4 worldVertex = mul(unity_ObjectToWorld, v.vertex);
            float3 worldNormalV = mul(unity_ObjectToWorld, float4(v.normal, 0)).xyz;

            // Snow vert shader
            float productoEscalar = saturate(dot(worldNormalV, float3(0, 1, 0)));

            // Ajusta la función smoothstep para hacer la transición más suave
            float smoothStepValue = smoothstep(_SnowUmbral, _Snow, productoEscalar);

            // Ajusta la cantidad de nieve agregada de manera más suave
            float snowAmount = smoothStepValue * (_SnowFactor - _SnowUmbral) / (1.0 - _SnowUmbral);

            // Limita la cantidad de nieve agregada
            snowAmount = saturate(snowAmount);

            // Modifica el vertice siempre hacia arriba
            worldVertex.xyz += snowAmount * worldNormalV;

            v.vertex = mul(unity_WorldToObject, worldVertex);
        }


        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float productoEscalar = dot(IN.worldNormal, float3(0, 1, 0));
            fixed4 cMain = tex2D(_MainTex, IN.uv_MainTex);
            fixed4 cSnow = tex2D(_SnowTex, IN.uv_VerticalTex);

            // Hace una smoothstep para hacer la transición más suave
            float smoothStepValue = smoothstep(0.0, _Snow, productoEscalar);

            if (smoothStepValue > 0)
            {
                // Usa la interpolación lineal para mezclar los colores
                float smoothFactor = smoothstep(0.0, 1.0, productoEscalar);
                fixed4 c = lerp(cMain, cSnow, clamp(smoothFactor * (_SnowFactor * 5), 0, 1));
                o.Albedo = c.rgb;
            }
            else
            {
                o.Albedo = cMain.rgb;
            }

            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = cMain.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
