float4x4 WorldViewProjection:WORLDVIEWPROJ;

float4 ParticleColor;
float Alpha;

texture ParticleTexture;
sampler ParticleSampler:TEXUNIT2=
sampler_state
{
	Texture=<ParticleTexture>;
	MipFilter=LINEAR;
	MinFilter=LINEAR;
	MagFilter=LINEAR;
};


struct a2v_nomap
{
	float4 Position:POSITION0;
};
struct a2v_map
{
	float4 Position:POSITION0;
	float2 Coords:TEXCOORD0;
};

struct v2p_nomap
{
	float4 Position:POSITION0;
};
struct v2p_map
{
	float4 Position:POSITION0;
	float2 Coords:TEXCOORD0;
};

void vs_map(in a2v_map IN,out v2p_map OUT)
{
	OUT.Position=mul(IN.Position,WorldViewProjection);
	OUT.Coords=IN.Coords;	
}
void vs_nomap(in a2v_nomap IN,out v2p_nomap OUT)
{
	OUT.Position=mul(IN.Position,WorldViewProjection);
}

float4 ps_map(in v2p_map IN):COLOR0
{
	float4 color=tex2D(ParticleSampler,IN.Coords);
	color*=Alpha;
	return color;
}

float4 ps_nomap(in v2p_nomap IN):COLOR0
{
	float4 color= ParticleColor;
	color*=Alpha;
	return color;
}


technique particle_map
{
	pass p0
	{
		vertexshader=compile vs_3_0 vs_map();
		pixelshader=compile ps_3_0 ps_map();
	}
}
technique particle_nomap
{
	pass p0
	{
		vertexshader=compile vs_3_0 vs_nomap();
		pixelshader=compile ps_3_0 ps_nomap();
	}
}