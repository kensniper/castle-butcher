float4x4 WorldViewProjection:WORLDVIEWPROJ;
float4x4 World:WORLD;
float4x4 WorldI:WORLD;

float3 LightDirection;
float3 CameraPosition;
float3 SpotPosition;
float3 SpotDirection;

int SpecularPower;

texture DiffuseTexture;
texture NormalTexture;
texture EmissiveTexture;
texture SpecularTexture;

sampler DiffuseSampler:TEXUNIT0=
sampler_state
{
	Texture=<DiffuseTexture>;
	MipFilter=LINEAR;
	MinFilter=LINEAR;
	MagFilter=LINEAR;
};
sampler NormalSampler:TEXUNIT1=
sampler_state
{
	Texture=<NormalTexture>;
	MipFilter=LINEAR;
	MinFilter=LINEAR;
	MagFilter=LINEAR;
};

sampler EmissiveSampler:TEXUNIT2=
sampler_state
{
	Texture=<EmissiveTexture>;
	MipFilter=LINEAR;
	MinFilter=LINEAR;
	MagFilter=LINEAR;
};

sampler SpecularSampler:TEXUNIT3=
sampler_state
{
	Texture=<SpecularTexture>;
	MipFilter=LINEAR;
	MinFilter=LINEAR;
	MagFilter=LINEAR;
};

struct a2v
{
	float4 Position:POSITION0;
	float3 Normal:NORMAL;
	float2 Coords:TEXCOORD0;
	float3 Tangent:TANGENT;
	float3 Binormal:BINORMAL;	
};

struct v2p
{
	float4 Position:POSITION0;
	float2 Coords:TEXCOORD0;
	float3 LightVec:TEXCOORD1;
	float3 halfVec:TEXCOORD2;
	float3 PositionObj:TEXCOORD3;
};

struct p2s
{
	float4 color:COLOR0;
};

void vs_diff(in a2v IN,out v2p OUT)
{
	OUT.Position=mul(IN.Position,WorldViewProjection);
	OUT.Coords=IN.Coords;
	float3 posWorld=mul(IN.Position,World);
	float3x3 TBN=float3x3(IN.Tangent,IN.Binormal,IN.Normal);
	
	float3 objLightDir=mul(-LightDirection,WorldI);
	OUT.LightVec=mul(TBN,objLightDir);	
	OUT.halfVec=float3(0,0,0);
}

void vs_diffspec(in a2v IN,out v2p OUT)
{
	OUT.Position=mul(IN.Position,WorldViewProjection);
	OUT.Coords=IN.Coords;
	float3 posWorld=mul(IN.Position,World);
	float3x3 TBN=float3x3(IN.Tangent,IN.Binormal,IN.Normal);
	
	float3 objViewVec=mul(CameraPosition-posWorld,WorldI);
	float3 viewVec=normalize(mul(TBN,objViewVec));
	
	float3 objLightDir=mul(-LightDirection,WorldI);
	OUT.LightVec=normalize(mul(TBN,objLightDir));	
	OUT.halfVec=normalize((OUT.LightVec+viewVec)/2);
	
	OUT.PositionObj=IN.Position;
}

void ps_diff(in v2p IN,out p2s OUT)
{
	float4 color=tex2D(DiffuseSampler,IN.Coords);
	
	float3 normal=2*tex2D(NormalSampler,IN.Coords)-1;
	OUT.color=color;
}
void ps_diff_bump(in v2p IN,out p2s OUT)
{
	float4 color=tex2D(DiffuseSampler,IN.Coords);
	float3 normal=2*tex2D(NormalSampler,IN.Coords).rgb-1;	
	float diff=clamp(dot(normalize(IN.LightVec),normal),0,1);
	OUT.color=color*diff;
}

void ps_diffspec_bump(in v2p IN,out p2s OUT)
{
	float4 color=tex2D(DiffuseSampler,IN.Coords);
	float3 normal=2*tex2D(NormalSampler,IN.Coords).rgb-1;
	float spec=pow(clamp(dot(normalize(IN.halfVec),normal),0,1),SpecularPower);
	float diff=clamp(dot(IN.LightVec,normal),0,1);
	OUT.color.rgb=color.rgb*diff+tex2D(SpecularSampler,IN.Coords).rgb*spec;
	OUT.color.a=color.a;
}

void ps_emisdiffspec_bump(in v2p IN,out p2s OUT)
{
	float4 color=tex2D(DiffuseSampler,IN.Coords);
	float3 normal=2*tex2D(NormalSampler,IN.Coords).rgb-1;
	float spec=pow(clamp(dot(normalize(IN.halfVec),normal),0,1),SpecularPower);
	float diff=clamp(dot(IN.LightVec,normal),0,1);
	OUT.color.rgb=tex2D(EmissiveSampler,IN.Coords).rgb+color.rgb*diff+tex2D(SpecularSampler,IN.Coords).rgb*spec;
	OUT.color.a=color.a;
}

technique Diffuse
{
	pass p0
	{
		vertexshader=compile vs_2_0 vs_diff();
		pixelshader=compile ps_2_0 ps_diff();
	}
}
technique DiffuseNormal
{
	pass p0
	{
		vertexshader=compile vs_2_0 vs_diff();
		pixelshader=compile ps_2_0 ps_diff_bump();
	}
}
technique DiffuseNormalSpecular
{
	pass p0
	{
		vertexshader=compile vs_2_0 vs_diffspec();
		pixelshader=compile ps_2_0 ps_diffspec_bump();
	}
}

technique EmissiveDiffuseNormalSpecular
{
	pass p0
	{
		vertexshader=compile vs_2_0 vs_diffspec();
		pixelshader=compile ps_2_0 ps_emisdiffspec_bump();
	}
}
