Shader "Game/Health"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		
		_VignetteColor("Vignette Color", Color) = (1, 0, 0)
		_VignettePatternTex("Vignette Pattern", 2D) = "white" {}
		_VignettePower("Vignette Power", Float) = 1
		
		_Radius("Radius", Range(0, 1)) = 1
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
				fixed4 _VignetteColor;
				sampler2D _VignettePatternTex;
				float _VignettePower;
				float _Radius;

				fixed4 frag(v2f i) : SV_TARGET
				{
					
					float2 radial = i.uv - 0.5;

					float radius = max(0, radial.x * radial.x + radial.y * radial.y - _Radius * _Radius);
					return (1 - radius) * tex2D(_MainTex, i.uv) + radius * _VignetteColor * _VignettePower;
					
					// float2 scale = float2(1, _ScreenParams.y / _ScreenParams.x);
					//
					// // Hard
					// float2 cord = (floor(i.uv * scale) + 0.5) / scale;					
					// return tex2D(_MainTex, cord);
				}
				ENDCG
			}
		}
}
