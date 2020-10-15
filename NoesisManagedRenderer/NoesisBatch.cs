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
        public NoesisRenderState renderState;
        public byte stencilRef;

        // Draw parameters
        public uint vertexOffset;
        public uint numVertices;
        public uint startIndex;
        public uint numIndices;

        // Textures (Unused textures are set to null)
        private IntPtr pPattern;
        public NoesisSamplerState patternSampler;

        private IntPtr pRamps;
        public NoesisSamplerState rampsSampler;

        private IntPtr pImage;
        public NoesisSamplerState imageSampler;

        private IntPtr pGlyphs;
        public NoesisSamplerState glyphsSampler;

        private IntPtr pShadow;
        public NoesisSamplerState shadowSampler;

        public ManagedTexture Pattern => ManagedTexture.GetTexture(this.pPattern);

        public ManagedTexture Ramps => ManagedTexture.GetTexture(this.pRamps);

        public ManagedTexture Image => ManagedTexture.GetTexture(this.pImage);

        public ManagedTexture Glyphs => ManagedTexture.GetTexture(this.pGlyphs);

        public ManagedTexture Shadow => ManagedTexture.GetTexture(this.pShadow);

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
