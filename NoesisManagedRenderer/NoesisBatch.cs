using System;
using System.Runtime.InteropServices;

namespace NoesisManagedRenderer
{
    // Render batch information
    [StructLayout(LayoutKind.Sequential)]
    public struct NoesisBatch
    {
        // Render state
        public NoesisShader shader;
        public byte renderState;
        public byte stencilRef;

        // Draw parameters
        public uint vertexOffset;
        public uint numVertices;
        public uint startIndex;
        public uint numIndices;

        // Textures (Unused textures are set to null)
        public IntPtr pattern;
        public byte patternSampler;

        public IntPtr ramps;
        public byte rampsSampler;

        public IntPtr image;
        public byte imageSampler;

        public IntPtr glyphs;
        public byte glyphsSampler;

        public IntPtr shadow;
        public byte shadowSampler;

        // Effect parameters
        public IntPtr effectParams;
        public uint effectParamsSize;
        public uint effectParamsHash;

        // Shader constants (Unused constants are set to null)
        public IntPtr projMtx;
        public uint projMtxHash;

        public IntPtr opacity;
        public uint opacityHash;

        public IntPtr rgba;
        public uint rgbaHash;

        public IntPtr radialGrad;
        public uint radialGradHash;
    };
}
