//!#version 450 // Comments with //! are for tricking the Visual Studio GLSL plugin into doing the right thing

// Resource Sets
layout(binding = 2) uniform _Uniform {
	uint uFlags;
	uint _u_padding_1;
	uint _u_padding_2;
	uint _u_padding_3;
};

// Shared set
#include "CommonResources.glsl"

// Vertex Data
layout(location = 0) in vec2 vPosition;
layout(location = 1) in vec2 vTexCoords;

// Instance Data
layout(location = 2) in vec3 iT1;
layout(location = 3) in vec3 iT2;
layout(location = 4) in vec3 iT3;
layout(location = 5) in vec3 iT4;
layout(location = 6) in vec2 iTexOffset;
layout(location = 7) in vec2 iTexSize;
layout(location = 8) in int  iTexLayer;
layout(location = 9) in uint iFlags;

// Outputs to fragment shader
layout(location = 0) out vec2 oTexPosition;   // Texture Coordinates
layout(location = 1) out flat float oLayer;   // Texture Layer
layout(location = 2) out flat uint oFlags;    // Flags
layout(location = 3) out vec2 oNormCoords;    // Normalised sprite coordinates
layout(location = 4) out vec3 oWorldPosition; // World position

void main()
{
	mat4 transform = mat4(vec4(iT1, 0), vec4(iT2, 0), vec4(iT3, 0), vec4(iT4, 1));

	vec4 worldSpace = transform * vec4(vPosition, 0, 1);

	vec4 normPosition;

	if ((uFlags & SKF_NO_TRANSFORM) == 0)
	{
		mat4 worldTransform = uView * transform;

		if ((iFlags & SF_BILLBOARD) != 0)
		{
			worldTransform[0] = transform[0];
			worldTransform[1] = transform[1];
		}

		normPosition = uProjection * worldTransform * vec4(vPosition, 0, 1);
    
		if ((iFlags & SF_BILLBOARD) != 0)
		{
			float cx = cos(-u_camera_look_direction.x + 1.5708);
			float sx = sin(-u_camera_look_direction.x + 1.5708);

			vec4 rotPosition = uProjection * uView * transform * mat4(
				cx, 0, sx, 0,
				0, 1, 0, 0,
			   -sx, 0, cx, 0,
				0, 0, 0, 1) * vec4(0.354, 0, 0, 1);

			float depth = rotPosition[2] / rotPosition[3];

			depth = depth <= 0.0 ? min(0, normPosition[2] / normPosition[3]) : depth; // switch to original depth if before the near plane - not working

			normPosition[2] = depth * normPosition[3];
		}
	}
	else
	{
		normPosition = worldSpace;
	}

	gl_Position = normPosition;

	oTexPosition = vTexCoords * iTexSize + iTexOffset;
	oLayer = float(iTexLayer);
	oFlags = iFlags;
	oNormCoords = vTexCoords;
	oWorldPosition = worldSpace.xyz;
}

