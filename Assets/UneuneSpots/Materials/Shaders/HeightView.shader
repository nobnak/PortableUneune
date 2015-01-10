Shader "Custom/HeightView" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		ZTest Always Cull Off ZWrite Off Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha
		
		Pass {
			CGPROGRAM
			#pragma multi_compile FLIP_Y_ON FLIP_Y_OFF
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			
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
				#if FLIP_Y_ON
				IN.uv.y = 1.0 - IN.uv.y;
				#endif
				OUT.uv = IN.uv;
				return OUT;
			}
			float4 frag(vs2ps IN) : COLOR {
				return tex2D(_MainTex, IN.uv);
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
