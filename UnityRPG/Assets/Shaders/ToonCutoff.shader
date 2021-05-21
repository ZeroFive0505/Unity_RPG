Shader "Custom/ToonCutoff"
{
	Properties
	{
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Color("Main Color", Color) = (1, 1, 1, 1)
		_CutOff("Alpha Cutoff", Range(0, 1)) = 0.5
		_OutlineColor("Outline Color", Color) = (1, 1, 1, 1)
		_OutlineWidth("Outline Width", Range(0, 0.01)) = 0.0001
		_BumpMap("Normal Map", 2D) = "bump" {}
		_SpecCol("Specular Color", Color) = (1, 1, 1, 1)
		_SpecPow("Specular Power", Range(100, 500)) = 300
	}
		SubShader
		{
			Tags { "RenderType" = "TransparentCutout" 
			"Queue" = "AlphaTest" 
			"PassFlags" = "OnlyDirectional" 
			"LightMode" = "ForwardBase" }

			Cull Front
			LOD 200


			//1st pass
			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Nolight vertex:vert

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			sampler2D _MainTex;
			sampler2D _BumpMap;
			fixed4 _OutlineColor;
			fixed _OutlineWidth;


			void vert(inout appdata_full v)
			{
				v.vertex.xyz = v.vertex.xyz + v.normal.xyz * _OutlineWidth;
			}

			struct Input
			{
				float2 uv_MainTex;
				float2 uv_BumpMap;
			};


			void surf(Input IN, inout SurfaceOutput o)
			{
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
				o.Albedo = c.rgb;
				o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
				o.Alpha = c.a;
			}

			float4 LightingNolight(SurfaceOutput s, float lightDir, float atten)
			{
				return float4(_OutlineColor.rgb, s.Alpha);
			}
			ENDCG


			//2nd pass
			cull back
			CGPROGRAM
			#pragma surface surf Toon alphatest:_CutOff

			sampler2D _MainTex;


			fixed _SpecPow;
			fixed4 _SpecCol;
			struct Input
			{
				float2 uv_MainTex;
			};

			void surf(Input IN, inout SurfaceOutput o)
			{
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
				o.Albedo = c.rgb;
				o.Alpha = c.a;
			}

			float4 LightingToon(SurfaceOutput s, float3 lightDir, float3 viewDir, float atten)
			{

				float4 final;
				float3 diffColor;
				float nDotl = dot(s.Normal, lightDir) * 0.5 + 0.5;

				nDotl *= 5;
				nDotl = ceil(nDotl) / 5;
				diffColor = nDotl * s.Albedo * _LightColor0.rgb * atten;

				float3 specColor;
				float3 H = normalize(lightDir + viewDir);
				float spec = saturate(dot(H, s.Normal));
				spec = pow(spec, _SpecPow);
				specColor = spec * _SpecCol.rgb;


				final.rgb = diffColor.rgb + specColor.rgb;
				final.a = s.Alpha;
				return final;
			}

			ENDCG
		}
		Fallback "Diffuse"
}
