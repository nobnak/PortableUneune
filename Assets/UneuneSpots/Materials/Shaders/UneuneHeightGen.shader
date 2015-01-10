Shader "Custom/UneuneHeightGen" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Size ("Noise Size", Vector) = (0.25, 0.5, 1, 2)
		_Speed ("Noise Speed", Vector) = (30, 30, 30, 30)
		_Gain ("Noise Gain", Vector) = (0.7, 1.74, 3.62, 1.29)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		ZTest Always ZWrite Off Cull Off Fog { Mode Off }
		
		Pass {
			CGPROGRAM
			#pragma target 5.0
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "Noise.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _MainTex_TexelSize;
			float4 _Size;
			float4 _Speed;
			float4 _Gain;
			float4 _DateTime;
			
			struct vsin {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};
			struct vs2ps {
				float4 vertex : POSITION;
				float3 pos0 : TEXCOORD0;
				float3 pos1 : TEXCOORD1;
				float3 pos2 : TEXCOORD2;
				float3 pos3 : TEXCOORD3;
			};
			
			vs2ps vert(vsin IN) {
				IN.uv = TRANSFORM_TEX(IN.uv, _MainTex);
				#if UNITY_UV_STARTS_AT_TOP
				IN.uv.y = 1 - IN.uv.y;
				#endif
				float3 pos = float3(IN.uv * float2(_ScreenParams.x / _ScreenParams.y, 1), 0) + _DateTime.xyz;
				
				vs2ps OUT;
				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				OUT.pos0 = float3(_Size.x, _Size.x, _Speed.x) * pos;
				OUT.pos1 = float3(_Size.y, _Size.y, _Speed.y) * pos;
				OUT.pos2 = float3(_Size.z, _Size.z, _Speed.z) * pos;
				OUT.pos3 = float3(_Size.w, _Size.w, _Speed.w) * pos;
				return OUT;
			}
			float4 frag(vs2ps IN) : COLOR {
				float4 c = float4(snoise(IN.pos0), snoise(IN.pos1), snoise(IN.pos2), snoise(IN.pos3));
				return dot(c, _Gain);
			}
			ENDCG
		}
	} 
	FallBack off
}
