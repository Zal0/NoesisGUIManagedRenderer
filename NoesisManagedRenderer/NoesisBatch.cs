using System;
using System.Runtime.InteropServices;

namespace NoesisManagedRenderer
{
    // Render batch information
    [StructLayout(LayoutKind.Explicit, Size = 184)]
    public struct NoesisBatch
    {
        // Render state
        [FieldOffset(0)] public NoesisShader shader;
        [FieldOffset(1)] public byte renderState;
        [FieldOffset(2)] public byte stencilRef;

        // Draw parameters
        [FieldOffset(4)] public UInt32 vertexOffset;
        [FieldOffset(8)] public UInt32 numVertices;
        [FieldOffset(12)] public UInt32 startIndex;
        [FieldOffset(16)] public UInt32 numIndices;

        // Textures (Unused textures are set to null)
        [FieldOffset(20)] public IntPtr pattern;
        [FieldOffset(24)] public byte patternSampler;

        [FieldOffset(28)] public IntPtr ramps;
        [FieldOffset(32)] public byte rampsSampler;

        [FieldOffset(36)] public IntPtr image;
        [FieldOffset(40)] public byte imageSampler;

        [FieldOffset(44)] public IntPtr glyphs;
        [FieldOffset(48)] public byte glyphsSampler;

        [FieldOffset(52)] public IntPtr shadow;
        [FieldOffset(56)] public byte shadowSampler;

        // Effect parameters
        [FieldOffset(60)] public IntPtr effectParams;
        [FieldOffset(64)] public UInt32 effectParamsSize;
        [FieldOffset(68)] public UInt32 effectParamsHash;

        // Shader constants (Unused constants are set to null)
        [FieldOffset(72)] public IntPtr projMtx;
        [FieldOffset(76)] public UInt32 projMtxHash;

        [FieldOffset(80)] public IntPtr opacity;
        [FieldOffset(84)] public UInt32 opacityHash;

        [FieldOffset(88)] public IntPtr rgba;
        [FieldOffset(92)] public UInt32 rgbaHash;

        [FieldOffset(96)] public IntPtr radialGrad;
        [FieldOffset(100)] public UInt32 radialGradHash;
    };
}
