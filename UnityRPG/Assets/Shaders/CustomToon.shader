Shader "Custom/CustomToon"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
		_Cutoff("Alpha Cutoff", Range(0, 1)) = 0
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		[HDR]
		_AmbientColor("Ambient Color", Color) = (0.2, 0.2, 0.2, 0)
		[HDR]
		_SpecularColor("Specular Color", Color) = (0.9, 0.9, 0.9, 1)
		_Glossiness("Glossiness", Float) = 32
		[HDR]
		_RimColor("Rim Color", Color) = (1, 1, 1, 1)
		_RimAmount("Rim Amount", Range(0, 1)) = 0.7
		_RimThreshold("Rim Threshold", Range(0, 1)) = 0.1
		_BumpMap("Normal Map", 2D) = "bump" {}
    }

	SubShader
	{
		Tags { "Queue" = "AlphaTest" 
		"RenderType" = "TransparentCutout" 
		"RenderPipeLine" = "UniversalPipeLine"
		"LightMode" = "UniversalForward"}

		CGPROGRAM
	
		#pragma surface surf ToonShading noambient alphatest:_Cutoff
		

		sampler2D _MainTex;
		sampler2D _BumpMap;

		struct Input
		{
			float2 uv_MainTex;
			float2 uv_BumpMap;
		};

		float4 _Color;
		float4 _AmbientColor;
		float4 _SpecularColor;
		float4 _RimColor;

		float _Glossiness;
		float _RimAmount;
		float _RimThreshold;



		void surf(Input IN, inout SurfaceOutput o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			o.Alpha = c.a;
		}

		float4 LightingToonShading(SurfaceOutput s, float3 lightDir, float3 viewDir, float atten)
		{
			//Diffuse Color
			float nDotl = dot(lightDir, s.Normal);
			float lightIntensity = smoothstep(0, 0.01, nDotl * atten);
			float4 lightColor = lightIntensity * _LightColor0;

			//Half Vector
			float3 h = normalize(lightDir + viewDir);
			float nDoth = dot(s.Normal, h);

			float specularIntensity = pow(nDoth * lightIntensity, _Glossiness * _Glossiness);
			float specularIntensitySmoothStep = smoothstep(0.005, 0.01, specularIntensity);
			float4 specular = specularIntensitySmoothStep * _SpecularColor;

			float4 rimDot = 1 - dot(viewDir, s.Normal);
			float rimIntensity = rimDot * pow(nDotl, _RimThreshold);
			rimIntensity = smoothstep(_RimAmount - 0.01, _RimAmount + 0.01, rimIntensity);
			float4 rim = rimIntensity * _RimColor;

			float4 final;
			final.rgb = s.Albedo * (lightColor + _AmbientColor + specular + rim);
			final.a = s.Alpha;

			return _Color * final;
		}

		ENDCG
	}

    SubShader
    {
        Tags { "Queue" = "AlphaTest" "RenderType" = "TransparentCutout"}
        LOD 200

        CGPROGRAM
        #pragma surface surf ToonShading noambient alphatest:_Cutoff
   

        sampler2D _MainTex;
		sampler2D _BumpMap;

        struct Input
        {
            float2 uv_MainTex;
			float2 uv_BumpMap;
        };

        float4 _Color;
		float4 _AmbientColor;
		float4 _SpecularColor;
		float4 _RimColor;

		float _Glossiness;
		float _RimAmount;
		float _RimThreshold;
		

   
        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
            o.Alpha = c.a;
        }

		float4 LightingToonShading(SurfaceOutput s, float3 lightDir, float3 viewDir, float atten)
		{
			//Diffuse Color
			float nDotl = dot(lightDir, s.Normal);
			float lightIntensity = smoothstep(0, 0.01, nDotl * atten);
			float4 lightColor = lightIntensity * _LightColor0;

			//Half Vector
			float3 h = normalize(lightDir + viewDir);
			float nDoth = dot(s.Normal, h);

			float specularIntensity = pow(nDoth * lightIntensity, _Glossiness * _Glossiness);
			float specularIntensitySmoothStep = smoothstep(0.005, 0.01, specularIntensity);
			float4 specular = specularIntensitySmoothStep * _SpecularColor;

			float4 rimDot = 1 - dot(viewDir, s.Normal);
			float rimIntensity = rimDot * pow(nDotl, _RimThreshold);
			rimIntensity = smoothstep(_RimAmount - 0.01, _RimAmount + 0.01, rimIntensity);
			float4 rim = rimIntensity * _RimColor;
			
			float4 final;
			final.rgb = s.Albedo * (lightColor + _AmbientColor + specular + rim);
			final.a = s.Alpha;

			return _Color * final;
		}

        ENDCG
    }
    FallBack "Diffuse"
}
