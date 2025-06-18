// Made with Amplify Shader Editor v1.9.3.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "VFX_Liquid_Shader_01"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		[PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
		_Texture("Texture", 2D) = "white" {}
		_Texture_Reflect("Texture_Reflect", 2D) = "white" {}
		_Texture_Normal("Texture_Normal", 2D) = "bump" {}
		_Softness("Softness", Float) = 0
		_Emissive("Emissive", Float) = 1
		_Reflection("Reflection", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

	}

	SubShader
	{
		LOD 0

		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha
		
		
		Pass
		{
		CGPROGRAM
			
			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#include "UnityCG.cginc"
			#define ASE_NEEDS_FRAG_COLOR


			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				float4 ase_tangent : TANGENT;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord1 : TEXCOORD1;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
			};
			
			uniform fixed4 _Color;
			uniform float _EnableExternalAlpha;
			uniform sampler2D _MainTex;
			uniform sampler2D _AlphaTex;
			uniform float _Emissive;
			uniform sampler2D _Texture_Reflect;
			uniform sampler2D _Texture_Normal;
			uniform float4 _Texture_Normal_ST;
			uniform float _Reflection;
			uniform sampler2D _Texture;
			uniform float4 _Texture_ST;
			uniform float _Softness;

			
			v2f vert( appdata_t IN  )
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
				float3 ase_worldTangent = UnityObjectToWorldDir(IN.ase_tangent);
				OUT.ase_texcoord1.xyz = ase_worldTangent;
				float3 ase_worldNormal = UnityObjectToWorldNormal(IN.ase_normal);
				OUT.ase_texcoord2.xyz = ase_worldNormal;
				float ase_vertexTangentSign = IN.ase_tangent.w * ( unity_WorldTransformParams.w >= 0.0 ? 1.0 : -1.0 );
				float3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * ase_vertexTangentSign;
				OUT.ase_texcoord3.xyz = ase_worldBitangent;
				float3 ase_worldPos = mul(unity_ObjectToWorld, float4( (IN.vertex).xyz, 1 )).xyz;
				OUT.ase_texcoord4.xyz = ase_worldPos;
				
				OUT.ase_texcoord5 = IN.ase_texcoord1;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				OUT.ase_texcoord1.w = 0;
				OUT.ase_texcoord2.w = 0;
				OUT.ase_texcoord3.w = 0;
				OUT.ase_texcoord4.w = 0;
				
				IN.vertex.xyz +=  float3(0,0,0) ; 
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			fixed4 SampleSpriteTexture (float2 uv)
			{
				fixed4 color = tex2D (_MainTex, uv);

#if ETC1_EXTERNAL_ALPHA
				// get the color from an external texture (usecase: Alpha support for ETC1 on android)
				fixed4 alpha = tex2D (_AlphaTex, uv);
				color.a = lerp (color.a, alpha.r, _EnableExternalAlpha);
#endif //ETC1_EXTERNAL_ALPHA

				return color;
			}
			
			fixed4 frag(v2f IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				float2 uv_Texture_Normal = IN.texcoord.xy * _Texture_Normal_ST.xy + _Texture_Normal_ST.zw;
				float3 ase_worldTangent = IN.ase_texcoord1.xyz;
				float3 ase_worldNormal = IN.ase_texcoord2.xyz;
				float3 ase_worldBitangent = IN.ase_texcoord3.xyz;
				float3 tanToWorld0 = float3( ase_worldTangent.x, ase_worldBitangent.x, ase_worldNormal.x );
				float3 tanToWorld1 = float3( ase_worldTangent.y, ase_worldBitangent.y, ase_worldNormal.y );
				float3 tanToWorld2 = float3( ase_worldTangent.z, ase_worldBitangent.z, ase_worldNormal.z );
				float3 ase_worldPos = IN.ase_texcoord4.xyz;
				float3 ase_worldViewDir = UnityWorldSpaceViewDir(ase_worldPos);
				ase_worldViewDir = normalize(ase_worldViewDir);
				float3 worldRefl147 = reflect( -ase_worldViewDir, float3( dot( tanToWorld0, tex2D( _Texture_Normal, uv_Texture_Normal ).rgb ), dot( tanToWorld1, tex2D( _Texture_Normal, uv_Texture_Normal ).rgb ), dot( tanToWorld2, tex2D( _Texture_Normal, uv_Texture_Normal ).rgb ) ) );
				float2 uv_Texture = IN.texcoord.xy * _Texture_ST.xy + _Texture_ST.zw;
				float4 tex2DNode130 = tex2D( _Texture, uv_Texture );
				float4 texCoord154 = IN.ase_texcoord5;
				texCoord154.xy = IN.ase_texcoord5.xy * float2( 1,1 ) + float2( 0,0 );
				float4 appendResult29 = (float4(( ( saturate( IN.color ) * _Emissive ) + ( tex2D( _Texture_Reflect, worldRefl147.xy ) * _Reflection ) ).rgb , saturate( ( ( tex2DNode130.r - texCoord154.x ) / _Softness ) )));
				
				fixed4 c = appendResult29;
				c.rgb *= c.a;
				return c;
			}
		ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	Fallback Off
}
/*ASEBEGIN
Version=19302
Node;AmplifyShaderEditor.TexturePropertyNode;129;795.3386,554.8111;Inherit;True;Property;_Texture_Normal;Texture_Normal;2;0;Create;True;0;0;0;False;0;False;5d63ac5e2b3ccf945acf6765ef28536b;f99a508ed45a4b84793c6920052488f9;True;bump;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TexturePropertyNode;61;512.2886,114.237;Inherit;True;Property;_Texture;Texture;0;0;Create;True;0;0;0;False;0;False;87a49223327c8a74490bc017995eadc4;f99a508ed45a4b84793c6920052488f9;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SamplerNode;128;1079.987,518.9133;Inherit;True;Property;_T_Noise05_n;T_Noise05_n;2;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;130;798.5415,-55.6812;Inherit;True;Property;_T_Noise05_n1;T_Noise05_n;2;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;138;1083.65,-367.9581;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldReflectionVector;147;1500.501,527.4169;Inherit;True;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TexturePropertyNode;151;1498.539,322.868;Inherit;True;Property;_Texture_Reflect;Texture_Reflect;1;0;Create;True;0;0;0;False;0;False;c703cd705b8b9194f8814dc992b62d4a;f99a508ed45a4b84793c6920052488f9;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TextureCoordinatesNode;154;847.9374,262.1689;Inherit;False;1;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;150;1810.87,363.5758;Inherit;True;Property;_T_Noise05_n3;T_Noise05_n;3;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;141;1684.212,13.13998;Inherit;False;Property;_Emissive;Emissive;4;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;153;2150.44,514.9861;Inherit;False;Property;_Reflection;Reflection;5;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;144;1727.014,-152.2552;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;133;1147.464,43.0327;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;136;1148.264,223.8327;Inherit;False;Property;_Softness;Softness;3;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;140;1988.653,-109.9996;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;152;2130.82,240.3406;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;135;1332.264,103.0327;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;143;2015.151,31.69467;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;137;1489.779,156.4265;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;142;1547.946,-167.9051;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;29;2321.572,78.03764;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;166;2475.385,67.50301;Float;False;True;-1;2;ASEMaterialInspector;0;10;VFX_Liquid_Shader_01;0f8ba0101102bb14ebf021ddadce9b49;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;2;False;True;3;1;False;;10;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;True;5;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;CanUseSpriteAtlas=True;False;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;0;;0;0;Standard;0;0;1;True;False;;False;0
WireConnection;128;0;129;0
WireConnection;130;0;61;0
WireConnection;147;0;128;0
WireConnection;150;0;151;0
WireConnection;150;1;147;0
WireConnection;144;0;138;0
WireConnection;133;0;130;1
WireConnection;133;1;154;1
WireConnection;140;0;144;0
WireConnection;140;1;141;0
WireConnection;152;0;150;0
WireConnection;152;1;153;0
WireConnection;135;0;133;0
WireConnection;135;1;136;0
WireConnection;143;0;140;0
WireConnection;143;1;152;0
WireConnection;137;0;135;0
WireConnection;142;0;138;0
WireConnection;142;1;130;0
WireConnection;29;0;143;0
WireConnection;29;3;137;0
WireConnection;166;0;29;0
ASEEND*/
//CHKSM=16EE71EB0F0B5710A390A13FE09651B069BBB1B8