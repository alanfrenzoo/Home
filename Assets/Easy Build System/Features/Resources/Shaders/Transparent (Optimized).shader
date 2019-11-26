Shader "Easy Build System/Transparent (Optimized)" {
Properties {
}

Category {
    Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
    Blend DstColor One
    ColorMask RGB
    Cull Off Lighting Off ZWrite Off

    SubShader {
        Pass {

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_particles
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            fixed4 _Color;

            struct appdata_t {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                #ifdef SOFTPARTICLES_ON
                float4 projPos : TEXCOORD2;
                #endif
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float4 _MainTex_ST;

            v2f vert (appdata_t v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);

                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = UnityObjectToClipPos(v.vertex);

                #ifdef SOFTPARTICLES_ON

                o.projPos = ComputeScreenPos (o.vertex);

                COMPUTE_EYEDEPTH(o.projPos.z);

                #endif

                o.color = v.color;

                UNITY_TRANSFER_FOG(o,o.vertex);

                return o;
            }

            UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = _Color / 6;

                UNITY_APPLY_FOG_COLOR(i.fogCoord, col, fixed4(0,0,0,0));

                return col;
            }
            ENDCG
        }
    }
}
}
