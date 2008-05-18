float4x4 WorldViewProjection:WORLDVIEWPROJ;
float4x4 World:WORLD;

float3 LightDirection;
float3 CameraPosition;

int SpecularPower;

float4 DiffuseColor;
float3 NormalColor;
float3 EmissiveColor;
float3 SpecularColor;



struct a2v
{
	float4 Position:POSITION0;
	float3 Normal:NORMAL;
};

struct v2p
{
	float4 Position:POSITION0;
	float3 Normal:TEXCOORD0;
	float3 LightVec:TEXCOORD1;
	float3 HalfVec:TEXCOORD2;
};

struct p2s
{
	float4 color:COLOR0;
};


void vs(in a2v IN,out v2p OUT)
{
	OUT.Position=mul(IN.Position,WorldViewProjection);
	OUT.Normal=mul(IN.Normal,World);
	float3 posWorld=mul(IN.Position,World);
	float3 viewVec=normalize(CameraPosition-posWorld);
	OUT.LightVec=-LightDirection;	
	OUT.HalfVec=normalize((OUT.LightVec+viewVec)/2);
}



void ps(in v2p IN,out p2s OUT)
{
	
	float spec=pow(saturate(dot(normalize(IN.HalfVec),normalize(IN.Normal))),SpecularPower);
	float diff=saturate(dot(IN.LightVec,normalize(IN.Normal)));
	OUT.color.rgb=EmissiveColor+DiffuseColor.rgb*diff+SpecularColor*spec;
	OUT.color.a=DiffuseColor.a;
}


technique Simple
{
	pass p0
	{
		vertexshader=compile vs_2_0 vs();
		pixelshader=compile ps_2_0 ps();
	}
}