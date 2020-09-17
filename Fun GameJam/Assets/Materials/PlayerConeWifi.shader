// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "GameJam/ConeWifi"
{
	Properties
	{
		_ConeMasks("ConeMasks", 2D) = "white" {}
		[HDR][MainColor]_Color("_Color", Color) = (1,0.3915094,0.3915094,0)
		_TilingSpeeds1("TilingSpeeds1", Vector) = (0.1,0.1,3,2)
		_TilingSpeeds2("TilingSpeeds2", Vector) = (0.25,0.25,1,2.5)
		_TilingSpeeds3("TilingSpeeds3", Vector) = (0.5,0.5,1,0.5)
		_GlobalTimeMultiplier("GlobalTimeMultiplier", Float) = 0.01
		_GlobalExponent("GlobalExponent", Float) = 1.19
		_GlobalMult("GlobalMult", Float) = 3
		_Slider("Slider", Range( 0 , 1)) = 1
		_CompleteWidth("CompleteWidth", Float) = 0.2
		_GradientWidth("GradientWidth", Float) = 0.1
		_MaxWifi("MaxWifi", Range( 0 , 1)) = 0
		_AdditionalWifiMask("AdditionalWifiMask", 2D) = "white" {}
		_MinTransparency("MinTransparency", Float) = 0
		_MaxTransparency("MaxTransparency", Float) = 0
		_MinClouds("MinClouds", Float) = 0
		_MaxClouds("MaxClouds", Float) = 0
		_LowMultiplier("LowMultiplier", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Custom"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull Back
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha , SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
		};

		uniform float4 _Color;
		uniform sampler2D _ConeMasks;
		uniform float4 _ConeMasks_ST;
		uniform float _MinClouds;
		uniform float _MaxClouds;
		uniform float _GlobalTimeMultiplier;
		uniform float4 _TilingSpeeds1;
		uniform float _GlobalExponent;
		uniform float4 _TilingSpeeds2;
		uniform float _GlobalMult;
		uniform float4 _TilingSpeeds3;
		uniform float _LowMultiplier;
		uniform float _Slider;
		uniform float _CompleteWidth;
		uniform float _GradientWidth;
		uniform float _MinTransparency;
		uniform float _MaxTransparency;
		uniform sampler2D _AdditionalWifiMask;
		uniform float4 _AdditionalWifiMask_ST;
		uniform float _MaxWifi;


		float3 mod3D289( float3 x ) { return x - floor( x / 289.0 ) * 289.0; }

		float4 mod3D289( float4 x ) { return x - floor( x / 289.0 ) * 289.0; }

		float4 permute( float4 x ) { return mod3D289( ( x * 34.0 + 1.0 ) * x ); }

		float4 taylorInvSqrt( float4 r ) { return 1.79284291400159 - r * 0.85373472095314; }

		float snoise( float3 v )
		{
			const float2 C = float2( 1.0 / 6.0, 1.0 / 3.0 );
			float3 i = floor( v + dot( v, C.yyy ) );
			float3 x0 = v - i + dot( i, C.xxx );
			float3 g = step( x0.yzx, x0.xyz );
			float3 l = 1.0 - g;
			float3 i1 = min( g.xyz, l.zxy );
			float3 i2 = max( g.xyz, l.zxy );
			float3 x1 = x0 - i1 + C.xxx;
			float3 x2 = x0 - i2 + C.yyy;
			float3 x3 = x0 - 0.5;
			i = mod3D289( i);
			float4 p = permute( permute( permute( i.z + float4( 0.0, i1.z, i2.z, 1.0 ) ) + i.y + float4( 0.0, i1.y, i2.y, 1.0 ) ) + i.x + float4( 0.0, i1.x, i2.x, 1.0 ) );
			float4 j = p - 49.0 * floor( p / 49.0 );  // mod(p,7*7)
			float4 x_ = floor( j / 7.0 );
			float4 y_ = floor( j - 7.0 * x_ );  // mod(j,N)
			float4 x = ( x_ * 2.0 + 0.5 ) / 7.0 - 1.0;
			float4 y = ( y_ * 2.0 + 0.5 ) / 7.0 - 1.0;
			float4 h = 1.0 - abs( x ) - abs( y );
			float4 b0 = float4( x.xy, y.xy );
			float4 b1 = float4( x.zw, y.zw );
			float4 s0 = floor( b0 ) * 2.0 + 1.0;
			float4 s1 = floor( b1 ) * 2.0 + 1.0;
			float4 sh = -step( h, 0.0 );
			float4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;
			float4 a1 = b1.xzyw + s1.xzyw * sh.zzww;
			float3 g0 = float3( a0.xy, h.x );
			float3 g1 = float3( a0.zw, h.y );
			float3 g2 = float3( a1.xy, h.z );
			float3 g3 = float3( a1.zw, h.w );
			float4 norm = taylorInvSqrt( float4( dot( g0, g0 ), dot( g1, g1 ), dot( g2, g2 ), dot( g3, g3 ) ) );
			g0 *= norm.x;
			g1 *= norm.y;
			g2 *= norm.z;
			g3 *= norm.w;
			float4 m = max( 0.6 - float4( dot( x0, x0 ), dot( x1, x1 ), dot( x2, x2 ), dot( x3, x3 ) ), 0.0 );
			m = m* m;
			m = m* m;
			float4 px = float4( dot( x0, g0 ), dot( x1, g1 ), dot( x2, g2 ), dot( x3, g3 ) );
			return 42.0 * dot( m, px);
		}


		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			o.Emission = _Color.rgb;
			float2 uv_ConeMasks = i.uv_texcoord * _ConeMasks_ST.xy + _ConeMasks_ST.zw;
			float4 tex2DNode2 = tex2Dlod( _ConeMasks, float4( uv_ConeMasks, 0, (float)5) );
			float3 temp_output_12_0_g3 = float3( 1,0.5,1 );
			float3 ase_worldPos = i.worldPos;
			float3 temp_output_2_0_g3 = ( temp_output_12_0_g3 * ase_worldPos );
			float simplePerlin3D5_g3 = snoise( ( temp_output_2_0_g3 + ( ( temp_output_12_0_g3 * float3( 1,0.25,-0.8 ) ) * _Time.y ) ) );
			float lerpResult14 = lerp( ( tex2DNode2.g - saturate( (0.0 + (simplePerlin3D5_g3 - -1.0) * (1.0 - 0.0) / (1.0 - -1.0)) ) ) , tex2DNode2.g , tex2DNode2.g);
			float Time45_g4 = ( _Time.y * _GlobalTimeMultiplier );
			float4 temp_output_32_0_g4 = _TilingSpeeds1;
			float2 UV50_g4 = i.uv_texcoord;
			float2 panner18_g4 = ( Time45_g4 * (temp_output_32_0_g4).zw + ( (temp_output_32_0_g4).xy * UV50_g4 ));
			float4 tex2DNode30_g4 = tex2D( _ConeMasks, panner18_g4 );
			float temp_output_51_0_g4 = _GlobalExponent;
			float4 temp_output_33_0_g4 = _TilingSpeeds2;
			float2 panner17_g4 = ( Time45_g4 * (temp_output_33_0_g4).zw + ( (temp_output_33_0_g4).xy * UV50_g4 ));
			float4 tex2DNode29_g4 = tex2D( _ConeMasks, panner17_g4 );
			float temp_output_22_0_g4 = pow( tex2DNode29_g4.b , temp_output_51_0_g4 );
			float temp_output_52_0_g4 = _GlobalMult;
			float4 temp_output_38_0_g4 = _TilingSpeeds3;
			float2 panner19_g4 = ( Time45_g4 * (temp_output_38_0_g4).zw + ( UV50_g4 * (temp_output_38_0_g4).xy ));
			float4 tex2DNode20_g4 = tex2D( _ConeMasks, panner19_g4 );
			float lerpResult69 = lerp( _MinClouds , _MaxClouds , ( ( pow( tex2DNode30_g4.b , temp_output_51_0_g4 ) * temp_output_22_0_g4 * temp_output_52_0_g4 ) + ( temp_output_22_0_g4 * pow( tex2DNode20_g4.b , temp_output_51_0_g4 ) * temp_output_52_0_g4 ) ));
			float WaveSlider79 = _Slider;
			float LowMask75 = saturate( ( lerpResult14 * saturate( lerpResult69 ) * _LowMultiplier * ( 1.0 - abs( ( ( WaveSlider79 * 2.0 ) - 1.0 ) ) ) ) );
			float4 tex2DNode45 = tex2Dlod( _ConeMasks, float4( uv_ConeMasks, 0, (float)0) );
			float temp_output_34_0 = ( _CompleteWidth + _GradientWidth );
			float lerpResult35 = lerp( -temp_output_34_0 , ( 1.0 + temp_output_34_0 ) , _Slider);
			float temp_output_6_0_g13 = lerpResult35;
			float temp_output_3_0_g13 = temp_output_34_0;
			float temp_output_12_0_g13 = _CompleteWidth;
			float temp_output_2_0_g13 = tex2DNode45.r;
			float smoothstepResult9_g13 = smoothstep( ( temp_output_6_0_g13 + temp_output_3_0_g13 ) , ( temp_output_6_0_g13 + temp_output_12_0_g13 ) , temp_output_2_0_g13);
			float smoothstepResult8_g13 = smoothstep( ( temp_output_6_0_g13 - temp_output_12_0_g13 ) , ( temp_output_6_0_g13 - temp_output_3_0_g13 ) , temp_output_2_0_g13);
			float WaveMask44 = ( saturate( ( step( 0.01 , tex2DNode45.r ) - tex2DNode45.a ) ) * saturate( ( smoothstepResult9_g13 - smoothstepResult8_g13 ) ) );
			float2 uv_AdditionalWifiMask = i.uv_texcoord * _AdditionalWifiMask_ST.xy + _AdditionalWifiMask_ST.zw;
			float4 tex2DNode61 = tex2Dlod( _AdditionalWifiMask, float4( uv_AdditionalWifiMask, 0, (float)0) );
			float temp_output_58_0 = step( tex2DNode61.r , _MaxWifi );
			float lerpResult106 = lerp( ( ( 1.0 - temp_output_58_0 ) * tex2DNode61.a ) , temp_output_58_0 , temp_output_58_0);
			float lerpResult66 = lerp( _MinTransparency , _MaxTransparency , lerpResult106);
			float DistanceWifiMask68 = lerpResult66;
			float OutputMask96 = max( LowMask75 , ( WaveMask44 * DistanceWifiMask68 ) );
			o.Alpha = OutputMask96;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16600
2289;13;1538;1044;94.82446;193.0753;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;57;-2489.733,808.6426;Float;False;1938.065;897.7031;Comment;18;44;35;37;54;46;36;38;34;49;45;52;33;53;55;32;56;79;31;Wave mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;67;-2484.195,2343.881;Float;False;2913.924;973.218;Comment;16;66;107;68;106;99;101;103;104;63;65;98;58;59;61;62;60;DistanceWifiMask;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;31;-2445.52,1191.875;Float;False;Property;_Slider;Slider;9;0;Create;True;0;0;False;0;1;0.407;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;60;-2434.195,2495.259;Float;True;Property;_AdditionalWifiMask;AdditionalWifiMask;13;0;Create;True;0;0;False;0;None;908faea692e2bf94c841ffcf0eac2d6d;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.IntNode;62;-2181.111,2434.79;Float;False;Constant;_Int1;Int 1;14;0;Create;True;0;0;False;0;0;0;0;1;INT;0
Node;AmplifyShaderEditor.CommentaryNode;90;-3042.021,-403.4065;Float;False;3305.081;1170.505;Comment;29;80;7;10;8;6;9;11;73;82;83;71;70;4;2;69;74;84;85;14;76;72;12;78;75;86;87;88;89;16;Clouds UnderMask;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;33;-2439.733,1546.362;Float;False;Property;_GradientWidth;GradientWidth;11;0;Create;True;0;0;False;0;0.1;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;32;-2424.462,1444.5;Float;False;Property;_CompleteWidth;CompleteWidth;10;0;Create;True;0;0;False;0;0.2;0.3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;1;-3216.825,915.8566;Float;True;Property;_ConeMasks;ConeMasks;1;0;Create;True;0;0;False;0;44c3e5b469879d9428a9bf0a2c476626;44c3e5b469879d9428a9bf0a2c476626;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;79;-2141.869,1153.007;Float;False;WaveSlider;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;88;-2855.144,395.6971;Float;False;1;0;SAMPLER2D;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SamplerNode;61;-1988.111,2507.79;Float;True;Property;_TextureSample6;Texture Sample 6;14;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;MipLevel;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;59;-1777.782,2402.881;Float;False;Property;_MaxWifi;MaxWifi;12;0;Create;True;0;0;False;0;0;0.252;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;34;-2146.969,1562.778;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;89;-2992.021,116.6777;Float;False;1;0;SAMPLER2D;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.IntNode;46;-2397.414,939.6511;Float;False;Constant;_Int0;Int 0;12;0;Create;True;0;0;False;0;0;0;0;1;INT;0
Node;AmplifyShaderEditor.GetLocalVarNode;80;-1322.703,134.3105;Float;False;79;WaveSlider;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-2631.708,307.8784;Float;False;Property;_GlobalTimeMultiplier;GlobalTimeMultiplier;6;0;Create;True;0;0;False;0;0.01;0.01;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;87;-2823.557,369.3746;Float;False;1;0;SAMPLER2D;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.IntNode;73;-2142.018,-24.23094;Float;False;Constant;_Int2;Int 2;18;0;Create;True;0;0;False;0;5;0;0;1;INT;0
Node;AmplifyShaderEditor.Vector4Node;7;-2386.425,313.744;Float;False;Property;_TilingSpeeds2;TilingSpeeds2;4;0;Create;True;0;0;False;0;0.25,0.25,1,2.5;0.25,0.25,1,-2;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;11;-2596.552,551.4047;Float;False;Property;_GlobalMult;GlobalMult;8;0;Create;True;0;0;False;0;3;1.25;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;86;-2976.228,90.35513;Float;False;1;0;SAMPLER2D;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.Vector4Node;8;-2422.532,485.9321;Float;False;Property;_TilingSpeeds3;TilingSpeeds3;5;0;Create;True;0;0;False;0;0.5,0.5,1,0.5;0.5,0.5,5,2.5;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;58;-1458.782,2393.881;Float;True;2;0;FLOAT;0.01;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;82;-1093.703,128.3105;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;45;-2166.202,858.6426;Float;True;Property;_TextureSample5;Texture Sample 5;2;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;MipLevel;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NegateNode;36;-2008.373,1395.14;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;37;-2130.659,1323.052;Float;False;2;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;6;-2419.087,137.8936;Float;False;Property;_TilingSpeeds1;TilingSpeeds1;3;0;Create;True;0;0;False;0;0.1,0.1,3,2;0.1,0.1,2,2;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;10;-2624.12,477.3172;Float;False;Property;_GlobalExponent;GlobalExponent;7;0;Create;True;0;0;False;0;1.19;4.11;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;16;-2282.657,-353.4065;Float;True;WorldNoise;-1;;3;cc605bab2570fec48832d24d0ef3c541;2,16,0,22,0;3;11;FLOAT3;0,0,0;False;12;FLOAT3;1,0.5,1;False;13;FLOAT3;1,0.25,-0.8;False;4;FLOAT;0;FLOAT;20;FLOAT;14;FLOAT3;17
Node;AmplifyShaderEditor.OneMinusNode;98;-1200.847,2389.825;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;71;-1823.163,652.0981;Float;False;Property;_MaxClouds;MaxClouds;17;0;Create;True;0;0;False;0;0;32.28;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;4;-2072.161,303.3471;Float;True;MasksMultiplier;-1;;4;5466eb984d608044eab85ec5695a3a71;7,66,2,67,2,68,2,69,2,70,2,71,2,55,0;10;56;FLOAT;2;False;31;SAMPLER2D;;False;32;FLOAT4;0.4,0.05,0,0;False;33;FLOAT4;1.3,1.3,0.7,0.1;False;38;FLOAT4;1.5,1.5,0.9,0.15;False;41;FLOAT;1;False;53;FLOAT;0;False;48;FLOAT2;0,0;False;51;FLOAT;2;False;52;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;70;-1829.163,547.0981;Float;False;Property;_MinClouds;MinClouds;16;0;Create;True;0;0;False;0;0;0.32;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;35;-1912.283,1268.455;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-1967.054,-92.49175;Float;True;Property;_TextureSample0;Texture Sample 0;2;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;MipLevel;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;53;-1710.668,865.4146;Float;True;2;0;FLOAT;0.01;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;83;-918.7032,160.3105;Float;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;38;-1746.408,1426.697;Float;True;Isolate Smooth;-1;;13;c841d60e2a8864df4894dbd824731159;0;4;2;FLOAT;0.5;False;6;FLOAT;0.5;False;12;FLOAT;0.2;False;3;FLOAT;0.8;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;49;-1374.667,1029;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;69;-1582.163,580.0981;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;74;-1576.948,-54.73219;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;84;-772.7032,159.3105;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;99;-900.7637,2415.447;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;55;-1106.777,1208.844;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;65;-532.5229,2938.505;Float;False;Property;_MaxTransparency;MaxTransparency;15;0;Create;True;0;0;False;0;0;5.34;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;63;-530.5229,2860.505;Float;False;Property;_MinTransparency;MinTransparency;14;0;Create;True;0;0;False;0;0;0.29;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;72;-1440.163,539.0981;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;14;-1525.462,235.6018;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;106;-608.8865,2492.18;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;76;-1332.555,646.2491;Float;False;Property;_LowMultiplier;LowMultiplier;18;0;Create;True;0;0;False;0;0;0.15;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;85;-655.7032,193.3105;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;52;-1465.202,1506.533;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;66;-284.0928,2886.968;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;-430.6421,395.3679;Float;True;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;56;-1132.777,1400.844;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;44;-901.2709,1389.534;Float;False;WaveMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;78;-170.2408,432.7238;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;68;171.2502,2889.949;Float;False;DistanceWifiMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;93;338.1569,1504.508;Float;True;44;WaveMask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;75;-12.94026,415.4318;Float;False;LowMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;95;343.1569,1727.508;Float;False;68;DistanceWifiMask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;91;526.0055,1418.627;Float;False;75;LowMask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;94;604.1569,1575.508;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;92;742.1569,1451.508;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;96;913.1569,1541.508;Float;False;OutputMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;107;-33.18728,2835.452;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;104;-1166.372,2719.834;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;103;-1424.372,2674.834;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RelayNode;101;-1610.372,2652.834;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;3;960.5152,-3.232688;Float;False;Property;_Color;_Color;2;1;[HDR];Create;True;0;0;False;1;MainColor;1,0.3915094,0.3915094,0;0.02441454,1,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;77;962.0328,194.5362;Float;False;96;OutputMask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RelayNode;54;-1763.816,1139.512;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;97;963.8448,-287.6681;Float;False;Property;_ColorLowBar;ColorLowBar;19;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;40;1315.179,457.7474;Float;True;39;Debug;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;39;310.8872,966.1876;Float;False;Debug;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1276.783,24.04492;Float;False;True;2;Float;ASEMaterialInspector;0;0;Unlit;GameJam/ConeWifi;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;2;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Custom;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;2;5;False;-1;10;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;79;0;31;0
WireConnection;88;0;1;0
WireConnection;61;0;60;0
WireConnection;61;2;62;0
WireConnection;34;0;32;0
WireConnection;34;1;33;0
WireConnection;89;0;1;0
WireConnection;87;0;88;0
WireConnection;86;0;89;0
WireConnection;58;0;61;1
WireConnection;58;1;59;0
WireConnection;82;0;80;0
WireConnection;45;0;1;0
WireConnection;45;2;46;0
WireConnection;36;0;34;0
WireConnection;37;1;34;0
WireConnection;98;0;58;0
WireConnection;4;31;87;0
WireConnection;4;32;6;0
WireConnection;4;33;7;0
WireConnection;4;38;8;0
WireConnection;4;41;9;0
WireConnection;4;51;10;0
WireConnection;4;52;11;0
WireConnection;35;0;36;0
WireConnection;35;1;37;0
WireConnection;35;2;31;0
WireConnection;2;0;86;0
WireConnection;2;2;73;0
WireConnection;53;1;45;1
WireConnection;83;0;82;0
WireConnection;38;2;45;1
WireConnection;38;6;35;0
WireConnection;38;12;32;0
WireConnection;38;3;34;0
WireConnection;49;0;53;0
WireConnection;49;1;45;4
WireConnection;69;0;70;0
WireConnection;69;1;71;0
WireConnection;69;2;4;0
WireConnection;74;0;2;2
WireConnection;74;1;16;20
WireConnection;84;0;83;0
WireConnection;99;0;98;0
WireConnection;99;1;61;4
WireConnection;55;0;49;0
WireConnection;72;0;69;0
WireConnection;14;0;74;0
WireConnection;14;1;2;2
WireConnection;14;2;2;2
WireConnection;106;0;99;0
WireConnection;106;1;58;0
WireConnection;106;2;58;0
WireConnection;85;0;84;0
WireConnection;52;0;38;0
WireConnection;66;0;63;0
WireConnection;66;1;65;0
WireConnection;66;2;106;0
WireConnection;12;0;14;0
WireConnection;12;1;72;0
WireConnection;12;2;76;0
WireConnection;12;3;85;0
WireConnection;56;0;55;0
WireConnection;56;1;52;0
WireConnection;44;0;56;0
WireConnection;78;0;12;0
WireConnection;68;0;66;0
WireConnection;75;0;78;0
WireConnection;94;0;93;0
WireConnection;94;1;95;0
WireConnection;92;0;91;0
WireConnection;92;1;94;0
WireConnection;96;0;92;0
WireConnection;107;0;66;0
WireConnection;104;0;58;0
WireConnection;104;1;103;0
WireConnection;103;0;61;4
WireConnection;101;0;61;4
WireConnection;54;0;45;4
WireConnection;0;2;3;0
WireConnection;0;9;77;0
ASEEND*/
//CHKSM=0E36D50DCD164C9EFAB59DC4E7DCA7C8B513C1CC