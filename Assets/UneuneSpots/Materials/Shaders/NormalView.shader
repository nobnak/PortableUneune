﻿Shader "Custom/NormalView" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		ZTest Always Cull Off ZWrite Off Fog { Mode Off }
		
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			
			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};
			struct vs2ps {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};
			
			vs2ps vert(appdata IN) {
				vs2ps OUT;
				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				OUT.uv = IN.uv;
				return OUT;
			}
			float4 frag(vs2ps IN) : COLOR {
				float4 c = tex2D(_MainTex, IN.uv);
				return float4(0.5 * (c.rgb + 1), 1);
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
