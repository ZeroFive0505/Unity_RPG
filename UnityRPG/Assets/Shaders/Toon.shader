Shader "Roystan/Toon"
{
	Properties
	{
		_Color("Color", Color) = (0.5, 0.65, 1, 1)
		_Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.0
		_MainTex("Main Texture", 2D) = "white" {}	
		[HDR]
		_AmbientColor("Ambient Color", Color) = (0.4, 0.4, 0.4, 1)
		[HDR]
		_SpecularColor("Specular Color", Color) = (0.9, 0.9, 0.9, 1)
		_Glossiness("Glossiness", Float) = 32
		[HDR]
		_RimColor("Rim Color", Color) = (1, 1, 1, 1)
		_RimAmount("Rim Amount", Range(0, 1)) = 0.716
		_RimThreshold("Rim Threshold", Range(0, 1)) = 0.1
	}
	SubShader
	{
		Tags{  
			"RenderPipeLine" = "UniversalPipeLine"
			"PassFlags" = "OnlyDirectional" 
			"LightMode" = "UniversalForward"
		}
		Pass
		{
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag 
			#pragma multi_compile_fwdbase
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			

			struct appdata
			{
				float3 normal : NORMAL;
				float4 vertex : POSITION;				
				float4 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 worldNormal : NORMAL;
				float2 uv : TEXCOORD0;
				float3 viewDir :TEXCOORD1;
				SHADOW_COORDS(2)
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _AmbientColor;
			float4 _SpecularColor;
			float4 _RimColor;


			float _RimAmount;
			float _Glossiness;
			float _RimThreshold;
			float _Cutoff;

			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.viewDir = WorldSpaceViewDir(v.vertex);
				TRANSFER_SHADOW(o)
				return o;
			}
			
			float4 _Color;

			float4 frag (v2f i) : SV_Target
			{
				float3 normal = normalize(i.worldNormal);
				float nDotl = dot(_WorldSpaceLightPos0, normal);
				float shadow = SHADOW_ATTENUATION(i);
				float lightIntensity = smoothstep(0, 0.01, nDotl * shadow);
				float4 light = lightIntensity * _LightColor0;

				float3 viewDir = i.viewDir;
				float3 halfVector = normalize(_WorldSpaceLightPos0 + viewDir);
				float nDoth = dot(normal, halfVector);

				float specularIntensity = pow(nDoth * lightIntensity, _Glossiness * _Glossiness);
				float specularIntensitySmooth = smoothstep(0.005, 0.01, specularIntensity);
				float4 specular = specularIntensitySmooth * _SpecularColor;

				float4 rimDot = 1 - dot(viewDir, normal);
				float rimIntensity = rimDot * pow(nDotl, _RimThreshold);
				rimIntensity = smoothstep(_RimAmount - 0.01, _RimAmount + 0.01, rimIntensity);
				float4 rim = rimIntensity * _RimColor;

				float4 sample = tex2D(_MainTex, i.uv);

				if (sample.a < _Cutoff)
					discard;

				return _Color * sample * (light + _AmbientColor + specular + rim);
			}
			ENDCG
	}
		UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
	}
}