Shader "Unlit/MonsterShader2"
{
	Properties
	{
		_Color("Color", Color) = (0.32,0.34,1,1)
		H("Hue", Range(0.0, 6.28)) = 0
		S("Saturation", Range(0.0, 1.0)) = 0.3
		V("Value", Range(0.0, 1.0)) = 1.0
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_Value("Value", Range(0,1)) = 1.0
		[KeywordEnum(Off, On, Black)] _UseGray("Set State", Float) = 0.0
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 300

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				// make fog work
				#pragma multi_compile_fog

				#pragma shader_feature _USEGRAY_OFF _USEGRAY_ON _USEGRAY_BLACK  

				#include "UnityCG.cginc"
				float4 _Color;

				float H;
				float S;
				float V;
				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					UNITY_FOG_COORDS(1)
					float4 vertex : SV_POSITION;
				};

				struct Input
				{
					float2 uv_MainTex;

				};


				sampler2D _MainTex;
				float4 _MainTex_ST;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{

					//cos and sin was on (H*PI/180)
					float VSU = float(V * S * cos(H));
					float VSW = float(V * S * sin(H));

					float4 BaseColor = tex2D(_MainTex, i.uv) * _Color;

					float retr = float((0.299 * V + 0.701 * VSU + 0.168 * VSW) * BaseColor.r + (0.587 * V - 0.587 * VSU + 0.330 * VSW) * BaseColor.g + (0.114 * V - 0.114 * VSU - 0.497 * VSW) * BaseColor.b);
					float retg = float((0.299 * V - 0.299 * VSU - 0.328 * VSW) * BaseColor.r + (0.587 * V + 0.413 * VSU + 0.035 * VSW) * BaseColor.g + (0.114 * V - 0.114 * VSU + 0.292 * VSW) * BaseColor.b);
					float retb = float((0.299 * V - 0.3 * VSU + 1.25 * VSW) * BaseColor.r + (0.587 * V - .588 * VSU - 1.05 * VSW) * BaseColor.g + (0.114 * V + .886 * VSU - 0.203 * VSW) * BaseColor.b);

					fixed4 col = fixed4(retr, retg, retb, 1);


					// apply fog
					UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
				ENDCG
			}
		}
}
