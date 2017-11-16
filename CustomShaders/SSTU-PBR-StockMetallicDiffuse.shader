Shader "SSTU/PBR/StockMetallicDiffuse"
{
	Properties 
	{
		_MainTex("_MainTex (RGB)", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_MetallicGlossMap("_MetallicGlossMap (RGB)", 2D) = "white" {}
		_AOMap("_AOMap (Grayscale)", 2D) = "white" {}
		_Emissive("Emission", 2D) = "white" {}
		_EmissiveColor("EmissionColor", Color) = (0,0,0)
		_Opacity("Emission Opacity", Range(0,1) ) = 1
		_RimFalloff("_RimFalloff", Range(0.01,5) ) = 0.1
		_RimColor("_RimColor", Color) = (0,0,0,0)
		_TemperatureColor("_TemperatureColor", Color) = (0,0,0,0)
		_BurnColor ("Burn Color", Color) = (1,1,1,1)
	}
	
	SubShader
	{
		Tags {"RenderType"="Opaque"}
		ZWrite On
		ZTest LEqual
		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM

		#pragma surface surf Standard keepalpha
		#pragma target 3.0
		#include "SSTUShaders.cginc"
				
		sampler2D _MainTex;
		sampler2D _Emissive;
		sampler2D _MetallicGlossMap;
		sampler2D _AOMap;

		float _Opacity;
		float4 _Color;
		float4 _EmissiveColor;
		float4 _TemperatureColor;
		float4 _RimColor;
		float _RimFalloff;
		
		struct Input
		{
			float2 uv_MainTex;
			float3 viewDir;
		};

		// struct SurfaceOutputStandard
        // {
            // fixed3 Albedo;		// base (diffuse or specular) color
            // fixed3 Normal;		// tangent space normal, if written
            // half3 Emission;
            // half Metallic;		// 0=non-metal, 1=metal
            // half Smoothness;	// 0=rough, 1=smooth
            // half Occlusion;		// occlusion (default 1)
            // fixed Alpha;		// alpha for transparencies
        // };
		
		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 color = tex2D(_MainTex,(IN.uv_MainTex));
			fixed4 spec = tex2D(_MetallicGlossMap, (IN.uv_MainTex));
			fixed4 ao = tex2D(_AOMap, (IN.uv_MainTex));
			fixed4 glow = tex2D(_Emissive, (IN.uv_MainTex));
			
			o.Albedo = color.rgb * _Color.rgb;
			o.Emission = glow.rgb * glow.aaa * _EmissiveColor.rgb *_EmissiveColor.aaa + stockEmit(IN.viewDir, float3(0,0,1), _RimColor, _RimFalloff, _TemperatureColor) * _Opacity;
			o.Metallic = spec.r;
			o.Smoothness = color.a;
			o.Occlusion = ao.r;
			o.Alpha = _Opacity;
		}
		ENDCG
	}
	Fallback "Bumped Specular"
}