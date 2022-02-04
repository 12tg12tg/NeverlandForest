Shader "Custom/MonsterShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
        _Value("Value", Range(0,1)) = 1.0
        [KeywordEnum(Off,On,Black)] _UseGray("Use Gray?", Float) = 0.0
        [Toggle(GRAY)] _Gray("Gray", Float) = 1.0
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 300

            CGPROGRAM
            // Physically based Standard lighting model, and enable shadows on all light types
            #pragma surface surf Standard fullforwardshadows

            // Use shader model 3.0 target, to get nicer looking lighting
            #pragma target 3.0

            #pragma shader_feature _USEGRAY_OFF _USEGRAY_ON _USEGRAY_BLACK

            #pragma shader_feature GRAY
            

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;

        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
#ifdef GRAY
            float t;
            t = 0.3 * c.r + 0.59 * c.g + 0.11 * c.b;
            o.Albedo = t;
#endif

#if _USEGRAY_ON
            float e;
            e = 0.3 * c.r + 0.59 * c.g + 0.11 * c.b;
            o.Albedo = e;
#elif _USEGRAY_BLACK
            o.Albedo = 0;
#endif
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
