#version 450

layout(set = 1, binding = 0) uniform texture2DArray Tex;
layout(set = 1, binding = 1) uniform sampler Samp;

layout(location = 0) in vec3 fsin_TexCoord;
layout(location = 0) out vec4 fsout_Color;

void main()
{
    fsout_Color = texture(sampler2DArray(Tex, Samp), fsin_TexCoord);
	if(fsout_Color.a == 0){
		discard;
	}
}