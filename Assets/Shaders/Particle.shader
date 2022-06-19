Shader "CrashSample/Particle"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 100

        Pass
        {
            Blend One One
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "../ComputeShaders/Include/Particle.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            StructuredBuffer<Particle> _ParticleBuffer;

            v2f vert (appdata v, uint instanceId : SV_InstanceID)
            {
                Particle p = _ParticleBuffer[instanceId];
                
                v2f o;
                float x = (p.age / p.lifetime);
                float scaleByLifetime = pow((x - 1.0), 2.0) * 0.5 + 0.5;
                float scale = p.scale * scaleByLifetime;
                float3 pos = (v.vertex.xyz * scale * p.active) + p.position;
                o.vertex = mul(UNITY_MATRIX_VP, float4(pos, 1.0));
                o.color = p.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
    }
}
