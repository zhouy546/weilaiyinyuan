Shader "Hidden/Blending" {
	Properties  {
		_MainTex ("Texture", 2D) = "white" {}
		_MaskTex ("Mask", 2D) = "white" {}
	}
	SubShader {
		Cull Off ZWrite Off ZTest Always

		Pass {
			CGPROGRAM
			#pragma target 4.0
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile ___ OUTPUT_CORNER_UV OUTPUT_BLEND_COLOR
			#pragma multi_compile ___  WIREFRAME_GRID WIREFRAME_CORNER
			#pragma multi_compile ___ TEXTURE_WITH_MASK TEXTURE_MASK_ONLY
			
			#include "UnityCG.cginc"
			 
			struct appdata {
				uint vid : SV_VertexID;
				uint iid : SV_InstanceID;
			};

			struct v2f {
				float4 uv : TEXCOORD0;
				float2 edge : TEXCOORD1;
				float4 bary : TEXCOORD2;
				float4 color : COLOR0;
				float4 vertex : SV_POSITION;
			};

			struct h2d_const {
				float tess[4] : SV_TessFactor;
				float inside[2] : SV_InsideTessFactor;
			};

			static const int _Indices[54] = {
				0,5,1,		0,4,5,
				1,6,2,		1,5,6,
				2,7,3,		2,6,7,
				4,9,5,		4,8,9,
				5,10,6,		5,9,10,
				6,11,7,		6,10,11,
				8,13,9,		8,12,13,
				9,14,10,	9,13,14,
				10,15,11,	10,14,15
			};
			static const float2 _Uvs[16] = { 
				float2(0,0), float2(0,0), float2(1,0), float2(1,0),
				float2(0,0), float2(0,0), float2(1,0), float2(1,0),
				float2(0,1), float2(0,1), float2(1,1), float2(1,1),
				float2(0,1), float2(0,1), float2(1,1), float2(1,1)
			};
			static const float2 _Edges[16] = {
				float2(0,0), float2(1,0), float2(1,0), float2(0,0),
				float2(0,1), float2(1,1), float2(1,1), float2(0,1),
				float2(0,1), float2(1,1), float2(1,1), float2(0,1),
				float2(0,0), float2(1,0), float2(1,0), float2(0,0)
			};
			static const float4 _Colors[9] = {
				float4(0.5,0.5,0,1),	float4(1,0,1,1),	float4(0,1,0.5,1),
				float4(0,1,1,1),		float4(0,0,0,0),	float4(1,0,0,1),
				float4(1,0,0.5,1),		float4(0,1,0,1),	float4(0.5,0.5,1,1)
			};
			static const float4 _Barycentric[6] = {
				float4(0,1,0,1), float4(1,0,1,0), float4(1,0,0,1), float4(0,1,0,1),	float4(0,1,1,0), float4(1,0,1,0)
			};
			static const float _Gain = 2;
			
			sampler2D _MainTex;
			sampler2D _MaskTex;
			float4 _GridDensity;
			StructuredBuffer<float4x4> _UVToWorldMatrices;
			StructuredBuffer<float4x4> _EdgeToLocalUVMatrices;
			StructuredBuffer<float4x4> _LocalToWorldUVMatrices;

			float wireframe(float4 bary) {
				float4 d = fwidth(bary);
				float4 l = smoothstep(0, _Gain * d, bary);
				float lmin = min(min(l.x, l.y), min(l.z, l.w));
				return saturate(1 - lmin);
			}

			v2f vert (appdata v) {
				v2f o;
				
				int vindex = _Indices[v.vid];
				float2 uv = _Uvs[vindex];
				float2 edge = _Edges[vindex];

				float4x4 edgeMat = _EdgeToLocalUVMatrices[v.iid];
				float4x4 uvMat = _LocalToWorldUVMatrices[v.iid];
				float4x4 worldMat = _UVToWorldMatrices[v.iid];

				uv = lerp(uv, mul(edgeMat, float4(uv, 0, 1)).xy, edge);
				float2 worldUv = mul(uvMat, float4(uv, 1, 1)).xy;
				float2 worldUvWoOffset = mul(uvMat, float4(uv, 0, 1)).xy;

				float2 tensionx = float2(1 - uv.x, uv.x);
				float4 tension = float4(tensionx * (1 - uv.y), tensionx * uv.y);

				float4 pos = float4(mul(worldMat, tension).xyz, 1);
				pos = float4(pos.xy, 0, pos.z);
				if (_ProjectionParams.x < 0)
					pos.y *= -1;

				o.uv = float4(worldUv, worldUvWoOffset);
				o.edge = edge;
				o.bary = _Barycentric[v.vid % 6];
				o.color = _Colors[v.vid / 6];
				o.vertex = pos;
				return o;
			}

			float4 frag (v2f i) : SV_Target {
				float2 uv = i.uv.xy;
				float2 uvWoOffset = i.uv.zw;;

				float4 cmain = tex2D(_MainTex, uv);
				float4 cmask = tex2D(_MaskTex, uvWoOffset);
				float4 cout = cmain;
				#if defined(TEXTURE_WITH_MASK)
				cout *= cmask;
				#elif defined(TEXTURE_MASK_ONLY)
				cout = cmask;
				#endif

				#if defined(OUTPUT_CORNER_UV)
					cout = float4(frac(uv), 0, 1);
				#elif defined(OUTPUT_BLEND_COLOR)
					cout = lerp(cout, i.color, i.color.w);
				#else
					float g = (i.edge.x * i.edge.y);
					#ifdef UNITY_COLORSPACE_GAMMA
						g = LinearToGammaSpace(g);
					#endif
					cout *= g;
				#endif

				float cwire = 0;
				#if defined(WIREFRAME_GRID)
					float2 tile = frac(_GridDensity.xy * uv);
					cwire = wireframe(float4(tile, 1 - tile));
				#elif defined(WIREFRAME_CORNER)
					cwire = wireframe(i.bary);
				#endif

				return lerp(cout, 1, cwire);
			}
			ENDCG
		}
	}
}
