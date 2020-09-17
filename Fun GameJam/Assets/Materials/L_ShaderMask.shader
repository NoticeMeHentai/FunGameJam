// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "L_ShaderMask"
{
	Properties
	{
		_Vector0("Vector 0", Vector) = (0,0,0,0)
		_Float4("Float 4", Float) = 0.5
		_Float0("Float 0", Float) = -0.1
		_MaxDist("MaxDist", Float) = 10
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
			float4 vertexColor : COLOR;
			float3 worldPos;
		};

		uniform float4 _Vector0;
		uniform float _Float4;
		uniform float _Float0;
		uniform float _MaxDist;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 color3 = IsGammaSpace() ? float4(0.4811321,0.2214253,0,0) : float4(0.196991,0.0401773,0,0);
			float4 color4 = IsGammaSpace() ? float4(0,0.3867925,0.1279711,0) : float4(0,0.1237993,0.01492492,0);
			float4 lerpResult5 = lerp( color3 , color4 , i.vertexColor.r);
			float4 color33 = IsGammaSpace() ? float4(0.1397903,0.9901811,0,1) : float4(0.01734425,0.9778085,0,1);
			float2 appendResult100 = (float2(_Vector0.x , _Vector0.z));
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
			float temp_output_85_0 = ( 0.5 * ( _MaxDist * 2.0 ) );
			float smoothstepResult81 = smoothstep( ( temp_output_85_0 + -0.5 ) , temp_output_85_0 , temp_output_9_0);
			float4 lerpResult32 = lerp( lerpResult5 , color33 , ( ifLocalVar76 * ( 1.0 - smoothstepResult81 ) ));
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
-172;1104;1828;862;3291.74;-611.0624;1;True;False
Node;AmplifyShaderEditor.SimpleTimeNode;97;-3195.259,1337.795;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;99;-3052.692,1400.069;Float;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;98;-2831.236,1399.477;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;96;-2520.017,1477.108;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;4;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;6;-2690.002,647.8898;Float;False;Property;_Vector0;Vector 0;0;0;Create;True;0;0;False;0;0,0,0,0;20,20,20,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;77;-2379.327,1341.529;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;23;-2680.318,881.5905;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;95;-2133.215,1614.622;Float;False;Property;_MaxDist;MaxDist;4;0;Create;True;0;0;False;0;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-2546.75,1166.037;Float;False;Property;_Float4;Float 4;2;0;Create;True;0;0;False;0;0.5;0.71;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;102;-2302.628,932.386;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;83;-1750.089,1604.397;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;100;-2351.628,706.386;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;72;-2238.27,1088.364;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;91;-1881.842,1465.966;Float;False;Constant;_Float6;Float 6;5;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;73;-2125.835,1224.965;Float;False;Property;_Float0;Float 0;3;0;Create;True;0;0;False;0;-0.1;-0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;89;-1542.542,1592.066;Float;False;Constant;_Float5;Float 5;5;0;Create;True;0;0;False;0;-0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;85;-1662.185,1453.769;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;9;-2103.423,768.2395;Float;True;2;0;FLOAT2;0,0;False;1;FLOAT2;0.48,0.43;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;71;-1929.31,1119.634;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;31;-1791.598,921.425;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;82;-1353.225,1485.039;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;81;-1215.513,1286.83;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;74;-1507.589,937.4225;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;3;-805,-403.5;Float;False;Constant;_Color0;Color 0;0;0;Create;True;0;0;False;0;0.4811321,0.2214253,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;88;-903.9328,1210.114;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;1;-1200,-199.5;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ConditionalIfNode;76;-1139.58,832.8815;Float;True;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;4;-915,-237.5;Float;False;Constant;_Color1;Color 1;0;0;Create;True;0;0;False;0;0,0.3867925,0.1279711,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;94;-470.6913,649.6401;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;5;-582,-90.5;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;33;-717.7571,74.01534;Float;False;Constant;_Color2;Color 2;1;0;Create;True;0;0;False;0;0.1397903,0.9901811,0,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;32;15.71886,109.3397;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector4Node;79;-2426.291,341.634;Float;False;Property;_Vector1;Vector 1;1;0;Create;True;0;0;False;0;0,0,0,0;20,20,20,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldPosInputsNode;78;-2219.607,476.3347;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DistanceOpNode;80;-1851.712,385.9837;Float;True;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0.48,0.43,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;353.758,45.80194;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;L_ShaderMask;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Translucent;0.5;True;True;0;False;Opaque;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;99;0;97;0
WireConnection;98;0;99;0
WireConnection;96;0;98;0
WireConnection;77;0;96;0
WireConnection;102;0;23;1
WireConnection;102;1;23;3
WireConnection;83;0;95;0
WireConnection;100;0;6;1
WireConnection;100;1;6;3
WireConnection;72;0;29;0
WireConnection;72;1;77;0
WireConnection;85;0;91;0
WireConnection;85;1;83;0
WireConnection;9;0;100;0
WireConnection;9;1;102;0
WireConnection;71;0;72;0
WireConnection;71;1;73;0
WireConnection;31;0;9;0
WireConnection;31;1;71;0
WireConnection;31;2;72;0
WireConnection;82;0;85;0
WireConnection;82;1;89;0
WireConnection;81;0;9;0
WireConnection;81;1;82;0
WireConnection;81;2;85;0
WireConnection;74;0;31;0
WireConnection;88;0;81;0
WireConnection;76;0;9;0
WireConnection;76;1;96;0
WireConnection;76;2;74;0
WireConnection;76;3;74;0
WireConnection;76;4;31;0
WireConnection;94;0;76;0
WireConnection;94;1;88;0
WireConnection;5;0;3;0
WireConnection;5;1;4;0
WireConnection;5;2;1;1
WireConnection;32;0;5;0
WireConnection;32;1;33;0
WireConnection;32;2;94;0
WireConnection;80;0;79;0
WireConnection;80;1;78;0
WireConnection;0;2;32;0
ASEEND*/
//CHKSM=4F81F2A87C31FCB21B8E536008276CF4E723B73B