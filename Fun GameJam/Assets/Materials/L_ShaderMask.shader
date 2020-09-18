// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "L_ShaderMask"
{
	Properties
	{
		_Float4("Float 4", Float) = 0.5
		_Float0("Float 0", Float) = -0.1
		_boue("boue", 2D) = "white" {}
		_herbe("herbe", 2D) = "white" {}
		_sentier("sentier", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
			float3 worldPos;
		};

		uniform sampler2D _boue;
		uniform sampler2D _herbe;
		uniform sampler2D _sentier;
		uniform float4 SignalInfo;
		uniform float _Float4;
		uniform float _Float0;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 appendResult119 = (float2(15.0 , 15.0));
			float2 uv_TexCoord117 = i.uv_texcoord * appendResult119;
			float4 lerpResult116 = lerp( tex2D( _boue, uv_TexCoord117 ) , tex2D( _herbe, uv_TexCoord117 ) , i.vertexColor.r);
			float4 lerpResult5 = lerp( lerpResult116 , tex2D( _sentier, uv_TexCoord117 ) , i.vertexColor.b);
			float4 color33 = IsGammaSpace() ? float4(0.1397903,0.9901811,0,1) : float4(0.01734426,0.9778085,0,1);
			float2 appendResult100 = (float2(SignalInfo.x , SignalInfo.y));
			float3 ase_worldPos = i.worldPos;
			float2 appendResult102 = (float2(ase_worldPos.x , ase_worldPos.z));
			float temp_output_9_0 = distance( appendResult100 , appendResult102 );
			float temp_output_96_0 = ( frac( ( _Time.y / 1.0 ) ) * 4.0 );
			float temp_output_72_0 = ( _Float4 * ( temp_output_96_0 * 2.0 ) );
			float smoothstepResult31 = smoothstep( ( temp_output_72_0 + _Float0 ) , temp_output_72_0 , temp_output_9_0);
			float temp_output_74_0 = ( 1.0 - smoothstepResult31 );
			float ifLocalVar76 = 0;
			if( temp_output_9_0 >= temp_output_96_0 )
				ifLocalVar76 = temp_output_74_0;
			else
				ifLocalVar76 = smoothstepResult31;
			float temp_output_85_0 = ( 0.5 * ( SignalInfo.z * 2.0 ) );
			float smoothstepResult81 = smoothstep( ( temp_output_85_0 + -0.5 ) , temp_output_85_0 , temp_output_9_0);
			float ifLocalVar104 = 0;
			if( SignalInfo.w > 0.5 )
				ifLocalVar104 = ( ifLocalVar76 * ( 1.0 - smoothstepResult81 ) );
			float4 lerpResult32 = lerp( lerpResult5 , color33 , ifLocalVar104);
			o.Emission = lerpResult32.rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16600
718;778;1828;780;3085.075;722.2615;1;True;False
Node;AmplifyShaderEditor.SimpleTimeNode;97;-2913.42,2058.013;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;99;-2746.141,2058.508;Float;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;107;-2977.025,1098.363;Float;False;282;257;XY  Pos XZ. Z: distance. W:bool;1;106;;1,1,1,1;0;0
Node;AmplifyShaderEditor.FractNode;98;-2598.821,2051.738;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;106;-2927.025,1148.362;Float;False;Global;SignalInfo;SignalInfo;6;0;Create;True;0;0;False;0;0,0,2,1;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;96;-2482.208,2053.689;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;4;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;77;-2341.518,1918.11;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-2508.941,1742.618;Float;False;Property;_Float4;Float 4;2;0;Create;True;0;0;False;0;0.5;0.45;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RelayNode;108;-2132.526,2179.033;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;23;-2524.389,1409.712;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;91;-1844.033,2042.547;Float;False;Constant;_Float6;Float 6;5;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;72;-2200.461,1664.945;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;83;-1712.28,2180.978;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;100;-2312.32,1307.641;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;73;-2088.026,1801.546;Float;False;Property;_Float0;Float 0;3;0;Create;True;0;0;False;0;-0.1;-0.52;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;102;-2312.847,1394.364;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;89;-1504.733,2168.647;Float;False;Constant;_Float5;Float 5;5;0;Create;True;0;0;False;0;-0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;71;-1891.501,1696.215;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;9;-2156.642,1330.218;Float;True;2;0;FLOAT2;0,0;False;1;FLOAT2;0.48,0.43;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;120;-2618.075,-386.2615;Float;False;Constant;_Float1;Float 1;9;0;Create;True;0;0;False;0;15;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;85;-1624.376,2030.35;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;82;-1315.416,2061.62;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;119;-2337.075,-454.2615;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SmoothstepOpNode;31;-1753.789,1498.006;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;117;-2047.894,-520.9735;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;74;-1469.78,1514.004;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;81;-1177.704,1863.411;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;1;-1605.27,-72.84948;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;114;-1603.551,-531.9863;Float;True;Property;_herbe;herbe;7;0;Create;True;0;0;False;0;ce0dd564c3607124bbc14b74d7bfd9d0;ce0dd564c3607124bbc14b74d7bfd9d0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;113;-1588.369,-745.4424;Float;True;Property;_boue;boue;6;0;Create;True;0;0;False;0;647daad88862db441836e78d54940735;647daad88862db441836e78d54940735;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;88;-866.1242,1786.695;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;76;-1101.771,1409.463;Float;True;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;115;-1625.678,-320.6184;Float;True;Property;_sentier;sentier;8;0;Create;True;0;0;False;0;04b2a84d7334aa2418c44a4108fef882;04b2a84d7334aa2418c44a4108fef882;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;94;-432.8827,1226.221;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RelayNode;109;-586.1799,1165.719;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;116;-1028.328,-462.1503;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;105;-682.3745,985.584;Float;False;Constant;_Float2;Float 2;5;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;104;-165.3221,842.3209;Float;False;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;33;-679.9485,650.5966;Float;False;Constant;_Color2;Color 2;1;0;Create;True;0;0;False;0;0.1397903,0.9901811,0,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;5;-322.6746,-221.9414;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.DistanceOpNode;80;-1813.903,962.565;Float;True;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0.48,0.43,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;103;-729.2176,830.6348;Float;False;Property;_SignalBigDisconnectionBool;SignalBigDisconnectionBool;5;0;Create;True;0;0;False;0;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;32;164.4439,158.1116;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector4Node;79;-2388.482,918.2154;Float;False;Property;_Vector1;Vector 1;1;0;Create;True;0;0;False;0;0,0,0,0;20,20,20,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector4Node;6;-2518.162,1239.061;Float;False;Property;_SignalPos;SignalPos;0;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;95;-1900.726,2143.275;Float;False;Property;_MaxDist;MaxDist;4;0;Create;True;0;0;False;0;10;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;78;-2181.798,1052.916;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;378.0746,149.1474;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;L_ShaderMask;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Translucent;0.5;True;True;0;False;Opaque;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;99;0;97;0
WireConnection;98;0;99;0
WireConnection;96;0;98;0
WireConnection;77;0;96;0
WireConnection;108;0;106;3
WireConnection;72;0;29;0
WireConnection;72;1;77;0
WireConnection;83;0;108;0
WireConnection;100;0;106;1
WireConnection;100;1;106;2
WireConnection;102;0;23;1
WireConnection;102;1;23;3
WireConnection;71;0;72;0
WireConnection;71;1;73;0
WireConnection;9;0;100;0
WireConnection;9;1;102;0
WireConnection;85;0;91;0
WireConnection;85;1;83;0
WireConnection;82;0;85;0
WireConnection;82;1;89;0
WireConnection;119;0;120;0
WireConnection;119;1;120;0
WireConnection;31;0;9;0
WireConnection;31;1;71;0
WireConnection;31;2;72;0
WireConnection;117;0;119;0
WireConnection;74;0;31;0
WireConnection;81;0;9;0
WireConnection;81;1;82;0
WireConnection;81;2;85;0
WireConnection;114;1;117;0
WireConnection;113;1;117;0
WireConnection;88;0;81;0
WireConnection;76;0;9;0
WireConnection;76;1;96;0
WireConnection;76;2;74;0
WireConnection;76;3;74;0
WireConnection;76;4;31;0
WireConnection;115;1;117;0
WireConnection;94;0;76;0
WireConnection;94;1;88;0
WireConnection;109;0;106;4
WireConnection;116;0;113;0
WireConnection;116;1;114;0
WireConnection;116;2;1;1
WireConnection;104;0;109;0
WireConnection;104;1;105;0
WireConnection;104;2;94;0
WireConnection;5;0;116;0
WireConnection;5;1;115;0
WireConnection;5;2;1;3
WireConnection;80;0;79;0
WireConnection;80;1;78;0
WireConnection;32;0;5;0
WireConnection;32;1;33;0
WireConnection;32;2;104;0
WireConnection;0;2;32;0
ASEEND*/
//CHKSM=1E766841DE12B036DEB78C6CF9EBCDF603C7C37C