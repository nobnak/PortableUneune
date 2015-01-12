Shader "Custom/NormalRotation" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_SSPos ("Screen Space Position", Vector) = (0, 0, 0, 0)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma target 5.0
		#pragma surface surf Lambert vertex:vert

		sampler2D _MainTex;
		sampler2D _UneuneNormalMap;
		float4 _SSPos;
		
		struct Input {
			float2 uv_MainTex;
			float2 uv_UneuneNormalMap;
		};
		
		void vert(inout appdata_full v) {
			float3 n = tex2Dlod(_UneuneNormalMap, float4(_SSPos.x, _SSPos.y, 0, 0)).xyz;
			float3 t = float3(1, 0, 0);
			float3 b = normalize(cross(n, t));
			t = normalize(cross(b, n));
			float3x3 rot = float3x3(
				t.x, b.x, n.x,
				t.y, b.y, n.y,
				t.z, b.z, n.z);

			v.vertex.xyz = mul(rot, v.vertex.xyz);
		}

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
