Shader "ld40/TransparentTexture"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1, 1, 1, 1)
	}
	SubShader
	{
		Tags
		{
			"RenderType" = "Transparent"
			"Queue" = "Transparent"
		}

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct VertIn
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 color : COLOR;
			};

			struct FragIn
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 color : COLOR;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			float4 _Color;
			
			FragIn vert(VertIn i)
			{
				FragIn o;
				o.vertex = UnityObjectToClipPos(i.vertex);
				o.uv = TRANSFORM_TEX(i.uv, _MainTex);
				o.color = i.color * _Color;
				return o;
			}
			
			fixed4 frag(FragIn i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				col = clamp(col * i.color, 0, 1);
				return col;
			}
			ENDCG
		}
	}
}
