Shader"Test"
{
	Properties
	{
		[MainTexture] _MainTex("Texture", 2D) = "white" {}
		[MainColor] _BaseColor("Color", Color) = (1, 1, 1, 1)
		_Radius("Radius", Range(0,0.5)) = 0.5
		_Falloff("Falloff", Range(0,1)) = 0
		_Threshold("Threshold", Range(0,0.5)) = 0
	}

	SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType" = "Transparent"}	
		LOD 100

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM

			#pragma vertex vertexFunc
			#pragma fragment fragmentFunc
			#pragma float makeCircleMask


			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			fixed4 _BaseColor;
			sampler2D _MainTex;
			Float _Falloff;
			Float _Threshold;
			float _Radius;

			float makeCircleMask(float radius, float2 pos)
			{
				float value = distance(pos, float2(0.5,0.5));
				return step(radius, value);
			}

			float getFalloffAlpha(float radius, float2 threshold, float2 pos)
			{
				float dist = distance(pos, float2(0.5, 0.5));
				float ratio = clamp((dist - threshold) / (radius - threshold), 0, 1);
				return (1 - ratio * _Falloff);
			}

			v2f vertexFunc(appdata IN)
			{
				v2f OUT;
				OUT.position = UnityObjectToClipPos(IN.vertex);
				OUT.uv = IN.uv;
				return OUT;
			}

			fixed4 fragmentFunc(v2f IN) : SV_Target	
			{
				fixed4 col = tex2D(_MainTex, IN.uv) * _BaseColor;
				col.a = 1;
				
				float circle = 1 - makeCircleMask(0.5, IN.uv);
				col.a = getFalloffAlpha(_Radius, clamp(_Threshold,0,_Radius), IN.uv);
	
				return col * circle;
			}

			ENDCG
		}
	}
}