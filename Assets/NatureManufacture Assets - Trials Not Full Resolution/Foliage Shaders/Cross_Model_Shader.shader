Shader "NatureManufacture Shaders/Trees/Cross Model Shader"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.65
		_ColorAdjustment("Color Adjustment", Vector) = (1,1,1,0)
		_MainTex("MainTex", 2D) = "white" {}
		_HealthyColor("Healthy Color", Color) = (1,0.9735294,0.9338235,1)
		_Smooothness("Smooothness", Float) = 0.3
		_AO("AO", Float) = 1
		[NoScaleOffset]_BumpMap("BumpMap", 2D) = "bump" {}
		_BumpScale("BumpScale", Range( 0 , 3)) = 1
		_InitialBend("Wind Initial Bend", Float) = 1
		_Stiffness("Wind Stiffness", Float) = 1
		_Drag("Wind Drag", Float) = 0.2
		_NewNormal("Vertex Normal Multiply", Vector) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "RenderPipeline" = "UniversalPipeline" }
		Pass 
		{

			 Name "ForwardLit"
			Tags{"LightMode" = "UniversalForward"}
		Cull Back
		HLSLPROGRAM
			//#include "UnityStandardUtils.cginc"
			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x
			#pragma target 2.0
			#pragma target 3.0
			#pragma multi_compile_instancing
			#include "NMWindNoShiver.cginc"
			#include "NM_indirect.cginc"
			
			#pragma instancing_options procedural:setup
			#pragma multi_compile GPU_FRUSTUM_ON __
			#pragma surface surf StandardSpecular keepalpha addshadow fullforwardshadows dithercrossfade 

			 #pragma vertex LitPassVertexSimple
			#pragma fragment LitPassFragmentSimple
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

        struct Attributes
        {
            float4 positionOS    : POSITION;
            float3 normalOS      : NORMAL;
            float4 tangentOS     : TANGENT;
            float2 texcoord      : TEXCOORD0;
            float2 lightmapUV    : TEXCOORD1;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        struct Varyings
        {
            float2 uv                       : TEXCOORD0;
            DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 1);

            float3 posWS                    : TEXCOORD2;    // xyz: posWS

        #ifdef _NORMALMAP
            float4 normal                   : TEXCOORD3;    // xyz: normal, w: viewDir.x
            float4 tangent                  : TEXCOORD4;    // xyz: tangent, w: viewDir.y
            float4 bitangent                : TEXCOORD5;    // xyz: bitangent, w: viewDir.z
        #else
            float3  normal                  : TEXCOORD3;
            float3 viewDir                  : TEXCOORD4;
        #endif

            half4 fogFactorAndVertexLight   : TEXCOORD6; // x: fogFactor, yzw: vertex light

        #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
            float4 shadowCoord              : TEXCOORD7;
        #endif

            float4 positionCS               : SV_POSITION;
            UNITY_VERTEX_INPUT_INSTANCE_ID
            UNITY_VERTEX_OUTPUT_STEREO
        };

        void InitializeInputData(Varyings input, half3 normalTS, out InputData inputData)
        {
            inputData.positionWS = input.posWS;

        #ifdef _NORMALMAP
            half3 viewDirWS = half3(input.normal.w, input.tangent.w, input.bitangent.w);
            inputData.normalWS = TransformTangentToWorld(normalTS,
                half3x3(input.tangent.xyz, input.bitangent.xyz, input.normal.xyz));
        #else
            half3 viewDirWS = input.viewDir;
            inputData.normalWS = input.normal;
        #endif

            inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
            viewDirWS = SafeNormalize(viewDirWS);

            inputData.viewDirectionWS = viewDirWS;

        #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
            inputData.shadowCoord = input.shadowCoord;
        #elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
            inputData.shadowCoord = TransformWorldToShadowCoord(inputData.positionWS);
        #else
            inputData.shadowCoord = float4(0, 0, 0, 0);
        #endif

            inputData.fogCoord = input.fogFactorAndVertexLight.x;
            inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;
            inputData.bakedGI = SAMPLE_GI(input.lightmapUV, input.vertexSH, inputData.normalWS);
        }

        ///////////////////////////////////////////////////////////////////////////////
        //                  Vertex and Fragment functions                            //
        ///////////////////////////////////////////////////////////////////////////////

        // Used in Standard (Simple Lighting) shader
        Varyings LitPassVertexSimple(Attributes input)
        {
            Varyings output = (Varyings)0;

            UNITY_SETUP_INSTANCE_ID(input);
            UNITY_TRANSFER_INSTANCE_ID(input, output);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

            VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
            VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
            half3 viewDirWS = GetCameraPositionWS() - vertexInput.positionWS;
            half3 vertexLight = VertexLighting(vertexInput.positionWS, normalInput.normalWS);
            half fogFactor = ComputeFogFactor(vertexInput.positionCS.z);

            output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);
            output.posWS.xyz = vertexInput.positionWS;
            output.positionCS = vertexInput.positionCS;

        #ifdef _NORMALMAP
            output.normal = half4(normalInput.normalWS, viewDirWS.x);
            output.tangent = half4(normalInput.tangentWS, viewDirWS.y);
            output.bitangent = half4(normalInput.bitangentWS, viewDirWS.z);
        #else
            output.normal = NormalizeNormalPerVertex(normalInput.normalWS);
            output.viewDir = viewDirWS;
        #endif

            OUTPUT_LIGHTMAP_UV(input.lightmapUV, unity_LightmapST, output.lightmapUV);
            OUTPUT_SH(output.normal.xyz, output.vertexSH);

            output.fogFactorAndVertexLight = half4(fogFactor, vertexLight);

        #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
            output.shadowCoord = GetShadowCoord(vertexInput);
        #endif

            return output;
        }

        // Used for StandardSimpleLighting shader
        half4 LitPassFragmentSimple(Varyings input) : SV_Target
        {
            UNITY_SETUP_INSTANCE_ID(input);
            UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

            float2 uv = input.uv;
            half4 diffuseAlpha = SampleAlbedoAlpha(uv, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap));
            half3 diffuse = diffuseAlpha.rgb * _BaseColor.rgb;

            half alpha = diffuseAlpha.a * _BaseColor.a;
            AlphaDiscard(alpha, _Cutoff);
        #ifdef _ALPHAPREMULTIPLY_ON
            diffuse *= alpha;
        #endif

            half3 normalTS = SampleNormal(uv, TEXTURE2D_ARGS(_BumpMap, sampler_BumpMap));
            half3 emission = SampleEmission(uv, _EmissionColor.rgb, TEXTURE2D_ARGS(_EmissionMap, sampler_EmissionMap));
            half4 specular = SampleSpecularSmoothness(uv, alpha, _SpecColor, TEXTURE2D_ARGS(_SpecGlossMap, sampler_SpecGlossMap));
            half smoothness = specular.a;

            InputData inputData;
            InitializeInputData(input, normalTS, inputData);

            half4 color = UniversalFragmentBlinnPhong(inputData, diffuse, specular, smoothness, emission, alpha);
            color.rgb = MixFog(color.rgb, inputData.fogCoord);
            return color;
        };

			ENDHLSL
		}
	}
	Fallback "Diffuse"
}