Shader "Game/Health"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		
		_VignetteBrightColor("Vignette Bright Color", Color) = (1, 0, 0)
		_VignetteDarkColor("Vignette Dark Color", Color) = (0.7, 0, 0)
		_VignettePower("Vignette Power", Float) = 1
		
		
		_PatternTex("Pattern", 2D) = "white" {}
		_PatternShift("Pattern Shift", Float) = 1
		_PatternScale("Pattern Scale", Float) = 1
		
		
		_Shift("Shift", Float) = 1
		_Scale("Scale", Float) = 1
		_Radius("Radius", Range(0, 1)) = 1
		_RadiusShift("Radius Shift", Float) = 1
		_RadiusScale("Radius Scale", Float) = 1
		_Frequency("Frequency", Float) = 1
	}

		SubShader
		{
			Cull Off ZWrite Off ZTest Always

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					float4 vertex : SV_POSITION;
				};

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					return o;
				}

				sampler2D _MainTex;
				fixed4 _VignetteBrightColor;
				fixed4 _VignetteDarkColor;
				float _VignettePower;
				
				sampler2D _PatternTex;
				float4 _PatternTex_ST;
				float _PatternShift;
				float _PatternScale;
				
				float _Shift;
				float _Scale;
				float _Radius;
				float _RadiusShift;
				float _RadiusScale;
				float _Frequency;

				fixed4 frag(v2f i) : SV_TARGET
				{
					float2 radial = i.uv - 0.5;

					
					const float t = 0.5f * (1 + sin(_Time.y * _Frequency));
					float radius = _RadiusShift + _Radius * _RadiusScale;
					radius = radial.x * radial.x + radial.y * radial.y - radius * radius - _Shift;
					radius = clamp(radius * _Scale, 0, 1);

					radius *= (tex2D(_PatternTex, i.uv * _PatternTex_ST.xy + _PatternTex_ST.zw) - _PatternShift) * _PatternScale;
					
					radius = clamp(radius, 0, 1);
					
					const float4 color = _VignetteDarkColor + (_VignetteBrightColor - _VignetteDarkColor) * t * _VignettePower;
					return color + (tex2D(_MainTex, i.uv) - color) * (1 - radius);
				}
				ENDCG
			}
		}
}
