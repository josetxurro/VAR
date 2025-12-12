Shader "Custom/BothTransparent"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5

        _BounceAmplitude ("Bounce Amplitude", Float) = 1.0
        _BounceFrequency ("Bounce Frequency", Float) = 10.0
        _MaxContactDistance ("Max Contact Distance", Float) = 1.0
        _MaxContactTime ("Max Contact Time", Float) = 1.0

        _ContactMagnitude ("Contact Magnitude", Float) = 0.0
        _ContactTime ("Contact Time", Float) = 0.0
        _ContactPoint ("Contact World Pos", Vector) = (0,0,0,0)
        _ContactDirection ("Contact Direction", Vector) = (0,1,0,0)
    }

    SubShader
    {
        Tags {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="TransparentCutout"
            "Queue"="AlphaTest"
        }

        Cull Off
        LOD 200

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma target 3.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // Textures
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float4 _Color;
            float _Cutoff;

            // Bounce variables
            float _BounceFrequency;
            float _BounceAmplitude;
            float _MaxContactDistance;
            float _MaxContactTime;

            float _ContactMagnitude;
            float _ContactTime;
            float3 _ContactPoint;
            float3 _ContactDirection;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings vert(Attributes IN)
            {
                UNITY_SETUP_INSTANCE_ID(IN);

                Varyings OUT;

                float timeSinceContact = (_Time.y - _ContactTime);
                
                // Bounce only in active window
                if (timeSinceContact <= _MaxContactTime)
                {
                    float3 worldPos = TransformObjectToWorld(IN.positionOS.xyz);

                    // Bounce amplitude depends on magnitude
                    float bounceAmp = _ContactMagnitude * 0.1;

                    float bounce = sin(timeSinceContact * _BounceFrequency) * bounceAmp;

                    // Distance attenuation
                    float dist = distance(worldPos, _ContactPoint);
                    float normalizedDist = saturate(dist / _MaxContactDistance);

                    // Time attenuation
                    float normalizedTime = saturate(timeSinceContact / _MaxContactTime);

                    float attenuation = 1.0 - normalizedDist - normalizedTime;
                    attenuation = saturate(attenuation);

                    // Vertex deformation
                    float3 offset = bounce * attenuation * normalize(_ContactDirection);

                    IN.positionOS.xyz += TransformWorldToObject(offset);
                }

                OUT.positionCS = TransformObjectToHClip(IN.positionOS);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv) * _Color;
                clip(tex.a - _Cutoff);
                return tex;
            }

            ENDHLSL
        }
    }

    FallBack "Diffuse"
}
