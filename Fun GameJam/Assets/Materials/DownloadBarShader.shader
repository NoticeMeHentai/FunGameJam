// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Hidden/Templates/Legacy/UIDefault"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
		_DownloadBarMask("DownloadBarMask", 2D) = "white" {}
		_DownloadRatio("_DownloadRatio", Range( 0 , 1)) = 0.511759
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
		
		Stencil
		{
			Ref [_Stencil]
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
			CompFront [_StencilComp]
			PassFront [_StencilOp]
			FailFront Keep
			ZFailFront Keep
			CompBack Always
			PassBack Keep
			FailBack Keep
			ZFailBack Keep
		}


		Cull Off
		Lighting Off
		ZWrite Off
		ZTest LEqual
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		
		Pass
		{
			Name "Default"
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

			#pragma multi_compile __ UNITY_UI_CLIP_RECT
			#pragma multi_compile __ UNITY_UI_ALPHACLIP
			
			
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				
			};
			
			uniform fixed4 _Color;
			uniform fixed4 _TextureSampleAdd;
			uniform float4 _ClipRect;
			uniform sampler2D _MainTex;
			uniform sampler2D _DownloadBarMask;
			uniform float4 _DownloadBarMask_ST;
			uniform float _DownloadRatio;
			
			v2f vert( appdata_t IN  )
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID( IN );
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
				OUT.worldPosition = IN.vertex;
				
				
				OUT.worldPosition.xyz +=  float3( 0, 0, 0 ) ;
				OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

				OUT.texcoord = IN.texcoord;
				
				OUT.color = IN.color * _Color;
				return OUT;
			}

			fixed4 frag(v2f IN  ) : SV_Target
			{
				float4 color4 = IsGammaSpace() ? float4(0,1,0.0516715,0) : float4(0,1,0.004087993,0);
				float2 uv_DownloadBarMask = IN.texcoord.xy * _DownloadBarMask_ST.xy + _DownloadBarMask_ST.zw;
				float4 tex2DNode1 = tex2D( _DownloadBarMask, uv_DownloadBarMask );
				int temp_output_7_0_g1 = 11;
				float4 break15 = ( color4 * step( saturate( ( floor( ( tex2DNode1.g * temp_output_7_0_g1 ) ) / ( temp_output_7_0_g1 - 1 ) ) ) , _DownloadRatio ) );
				float4 appendResult16 = (float4(break15.r , break15.g , break15.b , step( saturate( ( floor( ( tex2DNode1.g * temp_output_7_0_g1 ) ) / ( temp_output_7_0_g1 - 1 ) ) ) , _DownloadRatio )));
				
				half4 color = appendResult16;
				
				#ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif
				
				#ifdef UNITY_UI_ALPHACLIP
				clip (color.a - 0.001);
				#endif

				return color;
			}
		ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=16600
2005;102;1779;692;1973.264;575.1136;1.667982;True;False
Node;AmplifyShaderEditor.SamplerNode;1;-1460.411,-319.3877;Float;True;Property;_DownloadBarMask;DownloadBarMask;0;0;Create;True;0;0;False;0;ad2cd9af903331f4a9421a60ac971176;ad2cd9af903331f4a9421a60ac971176;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;21;-1137.328,-329.9384;Float;True;Quantify;-1;;1;ab9238847c91c8442abad67d1801577b;1,10,0;5;6;FLOAT2;0,0;False;11;FLOAT;0;False;12;FLOAT3;0,0,0;False;15;FLOAT4;0,0,0,0;False;7;INT;11;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2;-1388.954,-58.96988;Float;False;Property;_DownloadRatio;_DownloadRatio;1;0;Create;True;0;0;False;0;0.511759;0.511759;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;26;-828.0194,-290.2537;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;22;-643.333,-252.5518;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RelayNode;23;-266.3325,-60.22137;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;4;-332,-282.2;Float;False;Constant;_Color0;Color 0;2;0;Create;True;0;0;False;0;0,1,0.0516715,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-76.50049,-115.4315;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.BreakToComponentsNode;15;112.4995,-119.4315;Float;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;-872.0356,-40.89162;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;16;440.1832,-64.79108;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-622.7999,419.6999;Float;False;Constant;_Float1;Float 1;2;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;5;-337,22.5;Float;True;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMinOpNode;20;-766.2729,183.1978;Float;False;2;0;FLOAT;0;False;1;FLOAT;0.99;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;9;-871.7065,115.4523;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-600.3004,502.7685;Float;False;Constant;_Float2;Float 2;2;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCCompareEqual;19;-637.3812,134.8327;Float;False;4;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;1.1;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-1198.543,273.3362;Float;False;Constant;_Float3;Float 3;2;0;Create;True;0;0;False;0;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;14;674,-54;Float;False;True;2;Float;ASEMaterialInspector;0;4;Hidden/Templates/Legacy/UIDefault;5056123faa0c79b47ab6ad7e8bf059a4;True;Default;0;0;Default;2;True;2;5;False;-1;10;False;-1;0;1;False;-1;0;False;-1;False;False;True;2;False;-1;True;True;True;True;True;0;True;-9;True;True;0;True;-5;255;True;-8;255;True;-7;0;True;-4;0;True;-6;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;2;False;-1;True;0;False;-1;False;True;5;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;CanUseSpriteAtlas=True;False;0;False;False;False;False;False;False;False;False;False;False;True;2;0;;0;0;Standard;0;0;1;True;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;21;11;1;2
WireConnection;26;0;21;0
WireConnection;22;0;26;0
WireConnection;22;1;2;0
WireConnection;23;0;22;0
WireConnection;11;0;4;0
WireConnection;11;1;23;0
WireConnection;15;0;11;0
WireConnection;24;0;2;0
WireConnection;16;0;15;0
WireConnection;16;1;15;1
WireConnection;16;2;15;2
WireConnection;16;3;23;0
WireConnection;5;0;1;2
WireConnection;5;1;19;0
WireConnection;5;2;7;0
WireConnection;5;3;7;0
WireConnection;5;4;8;0
WireConnection;20;0;9;0
WireConnection;9;0;2;0
WireConnection;9;1;10;0
WireConnection;19;0;2;0
WireConnection;19;3;20;0
WireConnection;14;0;16;0
ASEEND*/
//CHKSM=8965AA4CD8FBD690E595B9C48E426A3E1BBC00B3