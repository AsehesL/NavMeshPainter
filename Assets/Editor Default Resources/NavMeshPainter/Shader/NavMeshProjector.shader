Shader "Unlit/NavMeshProjector"
{
	Properties
	{
		_CellSize("CellSize", float) = 2.5
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }

		Pass //CheckerBoard
		{
			zwrite off
			offset -1,-1
			blend srcalpha oneminussrcalpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 uvShadow : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			float4x4 unity_Projector;
			float _CellSize;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.uvShadow = mul(unity_Projector, v.vertex);
				return o;
			}
			
			float4 frag (v2f i) : SV_Target
			{
				float2 c = i.worldPos.xz*_CellSize;
				c = floor(c) / 2;
				float checker = frac(c.x + c.y) * 2 * 0.5 + 0.5;

				float4 col = float4(checker, checker, checker, 0.5);

				return col;
			}
			ENDCG
		}
	}
}
