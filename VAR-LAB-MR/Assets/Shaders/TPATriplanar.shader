Shader "Custom/TPATriplanar"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _SecondTex ("Vertical Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _TexScale ("Texture Scale", Float) = 0.0
        _MixValue ("Mix Value", Range(0,100)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _SecondTex;

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
        half _TexScale;
        half _MixValue;    
    
        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.worldPos.xz * _TexScale);
            fixed4 c2 = tex2D (_SecondTex, IN.worldPos.xy * _TexScale);
            fixed4 c3 = tex2D (_SecondTex, IN.worldPos.yz * _TexScale);

            half3 normal = pow(abs(IN.worldNormal), _MixValue);

            float auxNormal = normal.x + normal.y + normal.z;
            normal = normal / auxNormal;
            fixed4 res = c * normal.y + c2 * normal.z + c3 * normal.x;
            
            o.Albedo = res;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
