/******************************************************************************
 * File: MeshVisualization.shader
 * Copyright (c) 2022 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
 * 
 ******************************************************************************/

Shader "Unlit/Mesh Visualization"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "Queue" = "AlphaTest" "RenderType"="Transparent" "IgnoreProjector" = "True" }
        ZWrite Off
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 worldPos : TEXCOORD2;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            float4 _Color;

            fixed4 frag (v2f i) : SV_Target
            {
                float3 uv_dx = ddx(i.worldPos);
                float3 uv_dy = ddy(i.worldPos);
                float3 normal = normalize(cross(uv_dx, uv_dy));

                float3 normalizedRay = normalize(i.worldPos - _WorldSpaceCameraPos);
#if defined(SHADER_API_VULKAN)
                normalizedRay = -normalizedRay;                
#endif
                // Clamping between 0.05 and 1.0 to match the min and max slider value in the sample UI
                float light = clamp(dot(normal, normalizedRay), 0.05, 1);

                return float4(light, light, light, _Color.a) * _Color;
            }
            ENDCG
        }
    }
}
