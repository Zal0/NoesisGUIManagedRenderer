#ifndef MANAGED_RENDER_DEVICE_H
#define MANAGED_RENDER_DEVICE_H

#include <NsRender/RenderDevice.h>

class ManagedRenderDevice : public Noesis::RenderDevice
{
private:
    /// From RenderDevice
    //@{
    const Noesis::DeviceCaps& GetCaps() const override;
    Noesis::Ptr<Noesis::RenderTarget> CreateRenderTarget(const char* label, uint32_t width,
        uint32_t height, uint32_t sampleCount) override;
    Noesis::Ptr<Noesis::RenderTarget> CloneRenderTarget(const char* label,
        Noesis::RenderTarget* surface) override;
    Noesis::Ptr<Noesis::Texture> CreateTexture(const char* label, uint32_t width, uint32_t height,
        uint32_t numLevels, Noesis::TextureFormat::Enum format, const void** data) override;
    void UpdateTexture(Noesis::Texture* texture, uint32_t level, uint32_t x, uint32_t y,
        uint32_t width, uint32_t height, const void* data) override;
    void BeginRender(bool offscreen) override;
    void SetRenderTarget(Noesis::RenderTarget* surface) override;
    void BeginTile(const Noesis::Tile& tile, uint32_t surfaceWidth,
        uint32_t surfaceHeight) override;
    void EndTile() override;
    void ResolveRenderTarget(Noesis::RenderTarget* surface, const Noesis::Tile* tiles,
        uint32_t numTiles) override;
    void EndRender() override;
    void* MapVertices(uint32_t bytes) override;
    void UnmapVertices() override;
    void* MapIndices(uint32_t bytes) override;
    void UnmapIndices() override;
    void DrawBatch(const Noesis::Batch& batch) override;
    //@}
};

#endif