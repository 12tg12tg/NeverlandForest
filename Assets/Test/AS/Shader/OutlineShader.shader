// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/OutlineShader"
{
	Properties{
		_MainColor("Main Color", Color) = (.5,.5,.5,1)
		_OutlineColor("Outline Color", Color) = (0,0,0,1)

		_Outline("Outline width", Range(0, 10)) = 1
		_MainTex("Base (RGB)", 2D) = "white" { }
		_Rim("Rim", Float) = 0.0
		_scale("Z Position(Only Plane)", FLoat) = 1.0

			//Sorting
			_ZTest("ZTest Less = 4 / Always = 6 ", FLoat) = 1
	}

		CGINCLUDE
#include "UnityCG.cginc"
#pragma target 3.0

			struct v2f {
			fixed4 pos : SV_POSITION;
			fixed4 color : COLOR;

		};

		uniform fixed _Outline;
		uniform fixed4 _OutlineColor;
		uniform fixed _Rim;
		uniform fixed _scale;

		v2f vert(appdata_base v) {

			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			v.vertex.xyz += v.normal.xyz * _scale;
			fixed3 norm = mul((fixed3x3)UNITY_MATRIX_IT_MV, v.normal + v.vertex * _Outline);
			fixed2 offset = TransformViewToProjection(norm.xyz);

			o.pos.xy += (offset * o.pos.z * _Outline) * 0.001;

			o.pos.z += 0.01 * _Outline;


			o.color = _OutlineColor;
			return o;
		}
		ENDCG

			SubShader{
				Tags {"Queue" = "Geometry+200" "IgnoreProjector" = "True" "RenderType" = "Opaque"}
				 Blend One Zero
			//         ZTest NotEqual
			//         ZWrite on
					Pass
					{
						ColorMask 0
					}
			//
					Pass
					{
						Name "OUTLINE"
						Tags { "LightMode" = "Always" }
						Cull off
						Blend SrcColor OneMinusSrcColor
						ZTest Always
			//            ZWrite off

						CGPROGRAM
						#pragma vertex vert
						#pragma fragment frag

						fixed4 frag(v2f i) : COLOR
						{
							fixed4 c = i.color;

							return i.color;
						}
						ENDCG
					}

						CGPROGRAM
						#pragma surface surf Lambert

						struct Input {
							fixed2 uv_MainTex;
							fixed3 worldPos;
						};


						sampler2D _MainTex;
						//sampler2D _BumpMap;
						uniform fixed3 _MainColor;
						void surf(Input IN, inout SurfaceOutput o) {
							//                IN.worldPOS.z;
											o.Emission = tex2D(_MainTex, IN.uv_MainTex).rgb * _MainColor * _Rim;
											//    o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
											}
											ENDCG

		}

}

