#ifndef MANAGED_RENDER_DEVICE_H
#define MANAGED_RENDER_DEVICE_H

#include <NsRender/RenderDevice.h>
#include <NsRender/RenderTarget.h>
#include "ManagedTexture.h"

struct CreateTextureParams
{
	uint32_t width;
	uint32_t height;
	uint32_t numLevels;
	Noesis::TextureFormat::Enum format;
};

//Render device callbacks
typedef void(*DrawBatch)(const Noesis::Batch& batch);
typedef void* (*MapVertices)(uint32_t bytes);
typedef void(*UnmapVertices)();
typedef void* (*MapIndices)(uint32_t bytes);
typedef void(*UnmapIndices)();
typedef void(*BeginRender)();
typedef void(*EndRender)();
typedef bool (*CreateTexture)(const ManagedTexture* ptr, const CreateTextureParams& args);
typedef void(*UpdateTexture)(const ManagedTexture* ptr, uint32_t level, uint32_t x, uint32_t y, uint32_t width, uint32_t height, const void* data);

class ManagedRenderDevice : public Noesis::RenderDevice
{
public:
	::DrawBatch drawBatchCallback = 0;
	::MapVertices mapVerticesCallback = 0;
	::UnmapVertices unmapVerticesCallback = 0;
	::MapIndices mapIndicesCallback = 0;
	::UnmapIndices unmapIndicesCallback = 0;
	::BeginRender beginRenderCallback = 0;
	::EndRender endRenderCallback = 0;
	::CreateTexture createTextureCallback = 0;
	::UpdateTexture updateTextureCallback = 0;

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