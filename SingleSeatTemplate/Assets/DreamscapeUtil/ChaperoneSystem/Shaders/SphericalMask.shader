Shader "Custom/SphericalMask" {
	Properties {
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		[HDR]_Emission("Emission", Color) = (1,1,1,1)
		_MaskRadius("Mask Radius", float) = 1
		_NoiseSize("Noise size", float) = 1
	}

	SubShader {
		//Tags {"Queue" = "Transparent" "RenderType" = "Transparent" }
		//Cull off
		//LOD 200

		//CGPROGRAM
		//// Physically based Standard lighting model, and enable shadows on all light types
		//#pragma surface surf Standard

		//// Use shader model 3.0 target, to get nicer looking lighting
		//#pragma target 3.0

		Tags {"RenderType" = "Transparent" "Queue" = "Transparent"}
		LOD 200
		Pass {
			ColorMask 0
		}
		// Render normally
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask RGB

		CGPROGRAM

		#pragma surface surf Standard fullforwardshadows alpha:fade
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		fixed4 _Emission;
		float _MaskRadius;
		float _NoiseSize;

		float3 _GLOBALMaskLeftHandPosition;
		float3 _GLOBALMaskRightHandPosition;

		float3 _GLOBALMaskLeftFootPosition;
		float3 _GLOBALMaskRightFootPosition;

		float3 _GLOBALMaskHeadPosition;
		float3 _GLOBALMaskHipsPosition;

		// half _GLOBALMaskRadius;
		half _GLOBALMaskSoftness;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
		// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		float random(float2 input) {
			return frac(sin(dot(input, float2(12.9898,78.233)))* 43758.5453123);
		}

		void surf(Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

			half distLH = distance(_GLOBALMaskLeftHandPosition, IN.worldPos);
			half distRH = distance(_GLOBALMaskRightHandPosition, IN.worldPos);

			half distLF = distance(_GLOBALMaskLeftFootPosition, IN.worldPos);
			half distRF = distance(_GLOBALMaskRightFootPosition, IN.worldPos);

			half distHead = distance(_GLOBALMaskHeadPosition, IN.worldPos);
			half distHips = distance(_GLOBALMaskHipsPosition, IN.worldPos);

			half distMinHands = min(distLH, distRH);
			half distMinFeet = min(distLF, distRF);
			half distMinBody = min(distHead, distHips);

			half distMin = min(distMinHands, distMinFeet);
			distMin = min(distMin, distMinBody);

			half sphere = 1 - saturate((distMin - _MaskRadius)/ _GLOBALMaskSoftness);
			clip(sphere - 0.1);
			float squares = step(0.5, random(floor(IN.uv_MainTex * _NoiseSize)));
			//half emissionRingL = step(sphere - 0.1, 0.1) * squares;
			o.Emission = _Emission * sphere; //o.Emission = _Emission * emissionRingL;
			o.Albedo = c.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
