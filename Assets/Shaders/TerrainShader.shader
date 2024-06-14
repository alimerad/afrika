Shader "Custom/TerrainShader"
{
    Properties
    {
        _HeightMap ("Height Map", 2D) = "black" {}
        _Splat0 ("Splat 0", 2D) = "white" {}
        _Splat1 ("Splat 1", 2D) = "white" {}
        _Splat2 ("Splat 2", 2D) = "white" {}
        _Splat3 ("Splat 3", 2D) = "white" {}
        _MinHeight0 ("Min Height 0", Float) = 0
        _MaxHeight0 ("Max Height 0", Float) = 0.25
        _MinHeight1 ("Min Height 1", Float) = 0.25
        _MaxHeight1 ("Max Height 1", Float) = 0.5
        _MinHeight2 ("Min Height 2", Float) = 0.5
        _MaxHeight2 ("Max Height 2", Float) = 0.75
        _MinHeight3 ("Min Height 3", Float) = 0.75
        _MaxHeight3 ("Max Height 3", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard

        sampler2D _HeightMap;
        sampler2D _Splat0;
        sampler2D _Splat1;
        sampler2D _Splat2;
        sampler2D _Splat3;
        float _MinHeight0;
        float _MaxHeight0;
        float _MinHeight1;
        float _MaxHeight1;
        float _MinHeight2;
        float _MaxHeight2;
        float _MinHeight3;
        float _MaxHeight3;

        struct Input
        {
            float2 uv_HeightMap;
            float2 uv_Splat0;
            float2 uv_Splat1;
            float2 uv_Splat2;
            float2 uv_Splat3;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float height = tex2D(_HeightMap, IN.uv_HeightMap).r;

            half4 splat0 = tex2D(_Splat0, IN.uv_Splat0);
            half4 splat1 = tex2D(_Splat1, IN.uv_Splat1);
            half4 splat2 = tex2D(_Splat2, IN.uv_Splat2);
            half4 splat3 = tex2D(_Splat3, IN.uv_Splat3);

            half4 baseColor = splat0;

            if (height >= _MinHeight0 && height < _MaxHeight0)
                baseColor = splat0;
            else if (height >= _MinHeight1 && height < _MaxHeight1)
                baseColor = splat1;
            else if (height >= _MinHeight2 && height < _MaxHeight2)
                baseColor = splat2;
            else if (height >= _MinHeight3 && height <= _MaxHeight3)
                baseColor = splat3;

            o.Albedo = baseColor.rgb;
            o.Alpha = baseColor.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
