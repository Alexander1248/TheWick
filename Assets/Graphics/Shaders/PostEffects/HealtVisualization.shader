Shader "Game/Pixelation"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_PixelSize("Pixel Size", Int) = 4
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
				int _PixelSize;

				fixed4 frag(v2f i) : SV_TARGET
				{
					float2 scale = float2(1, _ScreenParams.y / _ScreenParams.x) * _PixelSize;

					// Hard
					float2 cord = (floor(i.uv * scale) + 0.5) / scale;					
					return tex2D(_MainTex, cord);
				}
				ENDCG
			}
		}
}
