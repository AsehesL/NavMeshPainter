Shader "Unlit/NavMeshDebug"
{
	Properties
	{
		_CellSize("CellSize", float) = 2.5
		_BrushPos ("BrushPos", vector) = (0,0,0,0)
		_BrushSize ("BrushSize", vector) = (0,0,0,0)
		_BrushColor ("BrushColor", color) = (0,0.5,1,0.5)
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
		LOD 100

		Pass
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
				float3 worldPos : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			float _CellSize;
			float4 _BrushPos;
			float4 _BrushSize;
			float4 _BrushColor;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				return o;
			}
			
			float4 frag (v2f i) : SV_Target
			{
				float2 c = i.worldPos.xz*_CellSize;
				c = floor(c) / 2;
				float checker = frac(c.x + c.y) * 2 * 0.5 + 0.5;
				
				float4 col = float4(checker, checker, checker, 0.5);
				float dis = length(i.worldPos.xz - _BrushPos.xz);
				float deltaH = abs(i.worldPos.y - _BrushPos.y);
				float attenH = step(deltaH, _BrushSize.y);
				float attenC = step(dis, _BrushSize.x);
				float2 attenS = step(abs(i.worldPos.xz - _BrushPos.xz), _BrushSize.xx);
				float atten = lerp(attenC, attenS.x*attenS.y, _BrushSize.z)*attenH;
				col.rgb = lerp(col.rgb, _BrushColor.rgb, _BrushColor.a*atten);

				return col;
			}
			ENDCG
		}
	}
}
