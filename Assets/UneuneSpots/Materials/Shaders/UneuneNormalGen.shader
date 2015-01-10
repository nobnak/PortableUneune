Shader "Custom/UneuneNormalGen" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_UneuneHeight ("Height", Float) = 0.5
	}
	SubShader {
		ZTest Always ZWrite Off Cull Off Fog { Mode Off }
		
		Pass {
			CGPROGRAM
			#pragma target 5.0
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			float _UneuneHeight;
			float4 _GroundTexelSize;
			
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
				float4 duv = _MainTex_TexelSize;
			
				float h00 = tex2D(_MainTex, IN.uv + float2(-duv.x, -duv.y)).r;
				float h10 = tex2D(_MainTex, IN.uv + float2(     0, -duv.y)).r;
				float h20 = tex2D(_MainTex, IN.uv + float2( duv.x, -duv.y)).r;
				
				float h01 = tex2D(_MainTex, IN.uv + float2(-duv.x,      0)).r;
				float h11 = tex2D(_MainTex, IN.uv + float2(     0,      0)).r;
				float h21 = tex2D(_MainTex, IN.uv + float2( duv.x,      0)).r;
				
				float h02 = tex2D(_MainTex, IN.uv + float2(-duv.x,  duv.y)).r;
				float h12 = tex2D(_MainTex, IN.uv + float2(     0,  duv.y)).r;
				float h22 = tex2D(_MainTex, IN.uv + float2( duv.x,  duv.y)).r;
				
				// y
				// 02 12 22
				// 01 11 21
				// 00 10 20 x
				
				float dx = _UneuneHeight * (0.125 * (h22 + h20 - (h02 + h00)) + 0.25 * (h21 - h01));
				float dy = _UneuneHeight * (0.125 * (h02 + h22 - (h00 + h20)) + 0.25 * (h12 - h10));
				
				float3 n = normalize(float3(dx, dy, 1));
				
				return float4(n, 1.0);
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
