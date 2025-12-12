Shader "TPABounce"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _BounceAmplitude ("Bounce Amplitude", Float) = 1.0
        _BounceFrequency ("Bounce Frequency", Float) = 10.0
        _MaxContactDistance ("Max Contact Distance", Float) = 1.0
        _MaxContactTime ("Max Contact Time", Float) = 1.0
        _Tess ("Tess", Float) = 10.0
        _ContactMagnitude ("Contact Magnitude", Float) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert tessellate:tess

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        float _ContactTime;
        float3 _ContactPoint;
        float3 _ContactPointLocal;
        float3 _ContactDirection;
        float _ContactMagnitude;

        float _Tess;
        float _MaxContactTime;

        float4 tess (appdata_full v0, appdata_full v1, appdata_full v2)
        {
            // Subdivision only if there is a contact in _ContactDuration interval
            float tessValue = _Tess * step(_Time.y - _ContactTime, _MaxContactTime);
            return (1+tessValue);
        }

        float _BounceFrequency;
        float _BounceAmplitude;
        float _MaxContactDistance;
        
        void vert(inout appdata_full v)
        {
            float _BounceAmplitude = _ContactMagnitude * 0.1;
            float timeSinceContact = (_Time.y - _ContactTime);
            float _Bounce = sin(timeSinceContact * _BounceFrequency) * _BounceAmplitude;
            
            float3 geometryPoint = mul(unity_ObjectToWorld, v.vertex).xyz;

            // Compute Distance
            float dist = length(geometryPoint - _ContactPoint.xyz);
            
            // Normalize to _ContactDistance
            // Closest point is 1
            // Farest point is 0
            float normalizedDist = dist / _MaxContactDistance;

            float _MaxContactTime = 1.0;

            // Compute Time
            float normalizedTimeSinceContact = timeSinceContact / _MaxContactTime;

            float _BounceAtenuation = 1;
            _BounceAtenuation -= normalizedDist;
            _BounceAtenuation -= normalizedTimeSinceContact;
            // The bounce attenunation must be in [0..1] interval
            _BounceAtenuation = saturate(_BounceAtenuation);
            
            // Move vertex in _ContactDirection
            v.vertex.xyz += (_Bounce * _BounceAtenuation * _ContactDirection);
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}