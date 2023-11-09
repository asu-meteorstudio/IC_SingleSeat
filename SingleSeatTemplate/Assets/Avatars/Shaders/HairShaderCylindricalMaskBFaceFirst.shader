﻿Shader "Artanim/HairCylindricalMaskBFaceFirst"
{
	// I wrote this shader only for using with AvatarSDK generated hair cut, attached to
	// a skeleton generated by Character Creator. 
	// It requires the vertex raw coordinates (before bones transform) to be copied
	// to uv2 channel (raw model coordinates are not available in unity vert shader, they
	// arrive in the shader pre-transformed using the bone matrix and that's not what I need here).
	// Also the up axis is Z and not Y, since the CharacterCreator character is rotated 90 deg. by default.
	// - Yves.

	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGBA)", 2D) = "white" {}
		_OpacityTex("Opacity (RGBA)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0

		_YStart("Cut start", Float) = -1
		_YHeight("Cut blend size", Float) = 1

		_XScale("X Scale", Float) = 1
		_XOffset("X Offset", Range(0,1)) = 0
		_CenterX("Center Offset X", Float) = 0
		_CenterY("Center Offset Y", Float) = 0

	}

		SubShader
		{
			Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
			LOD 200
			Cull Front

			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma surface surf Standard fullforwardshadows alpha:fade vertex:vert 
			#pragma target 3.0

			sampler2D _MainTex;
			sampler2D _OpacityTex;

			struct Input {
				float2 uv_MainTex;
				float3 worldPos;
				float3 localPos;
			};

			fixed4 _Color;
			float _CenterX,_CenterY;
			float _YStart;
			float _YHeight;
			float _XScale;
			float _XOffset;

			half _Glossiness;
			half _Metallic;


					void vert(inout appdata_full v, out Input o)
					{
						UNITY_INITIALIZE_OUTPUT(Input, o);
						o.localPos = v.texcoord1.xyz;
					}

					void surf(Input IN, inout SurfaceOutputStandard o)
					{
						const float PI = 3.1415926535897932384626433832795;

						fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
						o.Albedo = c.rgb;

						float3 localPos = IN.localPos;

						localPos.x = localPos.x - _CenterX;
						localPos.y = localPos.y - _CenterY;

						float yPos = localPos.z;
						float h = clamp((yPos - _YStart) / (_YHeight), 0.0, 1.0);

						float2 uv;

						uv = IN.uv_MainTex;
						uv.y = h;
						uv.x = _XOffset + (((atan2(localPos.x , localPos.y) + PI) / (2 * PI)) * _XScale);

						half opacity = c.a * tex2D(_OpacityTex, uv).r;
						o.Alpha = opacity;

						// Usefull for testing the mask:
						//o.Alpha = 1.0;
						//o.Albedo = float3(opacity, opacity, opacity).rgb;

						o.Metallic = _Metallic;
						o.Smoothness = _Glossiness;
					}
					ENDCG

						Cull Back

						CGPROGRAM
#pragma surface surf Standard fullforwardshadows alpha:fade vertex:vert 
#pragma target 3.0

						sampler2D _MainTex;
					sampler2D _OpacityTex;

					struct Input {
						float2 uv_MainTex;
						float3 worldPos;
						float3 localPos;
					};

					fixed4 _Color;
					float _CenterX, _CenterY;
					float _YStart;
					float _YHeight;
					float _XScale;
					float _XOffset;

					half _Glossiness;
					half _Metallic;


					void vert(inout appdata_full v, out Input o)
					{
						UNITY_INITIALIZE_OUTPUT(Input, o);
						o.localPos = v.texcoord1.xyz;
					}

					void surf(Input IN, inout SurfaceOutputStandard o)
					{
						const float PI = 3.1415926535897932384626433832795;

						fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
						o.Albedo = c.rgb;

						float3 localPos = IN.localPos;

						localPos.x = localPos.x - _CenterX;
						localPos.y = localPos.y - _CenterY;

						float yPos = localPos.z;
						float h = clamp((yPos - _YStart) / (_YHeight), 0.0, 1.0);

						float2 uv;

						uv = IN.uv_MainTex;
						uv.y = h;
						uv.x = _XOffset + (((atan2(localPos.x, localPos.y) + PI) / (2 * PI)) * _XScale);

						half opacity = c.a * tex2D(_OpacityTex, uv).r;
						o.Alpha = opacity;

						// Usefull for testing the mask:
						//o.Alpha = 1.0;
						//o.Albedo = float3(opacity, opacity, opacity).rgb;

						o.Metallic = _Metallic;
						o.Smoothness = _Glossiness;
					}
					ENDCG

		}
			FallBack "Diffuse"
}
