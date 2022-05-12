#version 450


layout(set = 0, binding = 0) uniform ProjView
{
    mat4 WorldViewProjection;
    mat4 Proj;
};

layout(set = 0, binding = 1) uniform ImageData
{
   vec2 ImageSizeArray[1000];
};

layout(location = 0) in vec4 Position;
layout(location = 1) in vec4 TexCoord;


layout(location = 2) in vec2 InstanceTransform;
layout(location = 3) in vec4 AtlasCoord;


layout(location = 0) out vec3 fsin_TexCoord;

void main()
{

	vec2 tileSizeHalf = vec2(16.0f,8.0f);
	
	//actual Image Index
	int index = int(AtlasCoord.z + AtlasCoord.w * 256);
	
	vec2 imageSize =  ImageSizeArray[index];
	
	//how many Images are possible inside of the big texture
	vec2 numberOfTextures = vec2(2048.0f,2048.0f) / vec2(imageSize.x,imageSize.y); // all Images are 2048 x 2048 
	
	//Calculate ImageSizeToDraw
	//- float2(imageSize.x/2,imageSize.y - tileSizeHalf.y) because images have different starting points 
	vec2 position = Position.xy * imageSize - vec2(imageSize.x/2,imageSize.y - tileSizeHalf.y);
	
	
	//calculate position with camera
	vec4 pos = vec4(position.xy + InstanceTransform.xy,1,1);


	gl_Position = pos * WorldViewProjection * Proj;
	fsin_TexCoord = vec3((TexCoord.x / numberOfTextures.x) + (1.0f / numberOfTextures.x * AtlasCoord.x),
							 (TexCoord.y / numberOfTextures.y) + (1.0f / numberOfTextures.y * AtlasCoord.y),index);
}