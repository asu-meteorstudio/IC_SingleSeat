﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/Colors_Brightness"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Val("Value", Float) = 1
	}
	
	SubShader
	{
		Pass
		{
			ZTest Always
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			uniform float _Val;
			half4 _MainTex_ST;

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				half2 texcoord  : TEXCOORD0;
				float4 vertex   : SV_POSITION;
				fixed4 color : COLOR;
			};

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color;

				return OUT;
			};

			float4 frag(v2f i) : COLOR
			{
				fixed4 c = tex2D(_MainTex, UnityStereoScreenSpaceUVAdjust(i.texcoord, _MainTex_ST));
				if (_Val<1.) c *= _Val; else c += _Val - 1;
				return float4(c);
			}

			ENDCG
		}
	}
}