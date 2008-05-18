float4x4 WorldViewProj:WORLDVIEWPROJ;
float4x4 World:WORLD;

float4 EmissiveColor;
float4 DiffuseColor;

float3 LightDirection;

texture EmisTexture;
texture DiffTexture;

sampler EmissiveSampler:TEXUNIT2=
sampler_state
{
	Texture=<EmisTexture>;
	MipFilter=LINEAR;
	MinFilter=LINEAR;
	MagFilter=LINEAR;
};

sampler DiffuseSampler:TEXUNIT0=
sampler_state
{
	Texture=<DiffTexture>;
	MipFilter=LINEAR;
	MinFilter=LINEAR;
	MagFilter=LINEAR;
};


struct a2v_nomap
{
	float4 Position:POSITION0;
	float3 Normal:NORMAL;
};
struct a2v_map
{
	float4 Position:POSITION0;
	float2 Coords:TEXCOORD0;
	float3 Normal:NORMAL;
};

struct v2p_nomap
{
	float4 Position:POSITION0;
	float3 Normal:TEXCOORD1;
};
struct v2p_map
{
	float4 Position:POSITION0;
	float2 Coords:TEXCOORD0;
	float3 Normal:TEXCOORD1;
};

void vs_map(in a2v_map IN,out v2p_map OUT)
{
	OUT.Position=mul(IN.Position,WorldViewProj);
	OUT.Coords=IN.Coords;	
	OUT.Normal=mul(IN.Normal,World);
}
void vs_nomap(in a2v_nomap IN,out v2p_nomap OUT)
{
	OUT.Position=mul(IN.Position,WorldViewProj);
	OUT.Normal=mul(IN.Normal,World);
}

float4 ps_map(in v2p_map IN):COLOR0
{
	return tex2D(EmissiveSampler,IN.Coords)+saturate(dot(IN.Normal,LightDirection))*tex2D(DiffuseSampler,IN.Coords);
}

float4 ps_nomap(in v2p_nomap IN):COLOR0
{
	return EmissiveColor+saturate(dot(IN.Normal,LightDirection))*DiffuseColor;
}


technique emissive_diffuse_map
{
	pass p0
	{
		vertexshader=compile vs_3_0 vs_map();
		pixelshader=compile ps_3_0 ps_map();
	}
}
technique emissive_diffuse_nomap
{
	pass p0
	{
		vertexshader=compile vs_3_0 vs_nomap();
		pixelshader=compile ps_3_0 ps_nomap();
	}
}