#include "ManagedRenderDevice.h"

#pragma warning(disable : 4100)

using namespace Noesis;

#define DLL_FUNC __declspec(dllexport)
extern "C"
{
	DLL_FUNC ManagedRenderDevice* CreateManagedRenderDevice(const Noesis::DeviceCaps deviceCaps, bool flippedTextures) { return new ManagedRenderDevice(deviceCaps, flippedTextures); }
	DLL_FUNC void SetManagedRenderDeviceCallbacks(
		ManagedRenderDevice* pDevice,
		DrawBatch drawBatchCallback,
		MapVertices mapVerticesCallback,
		UnmapVertices unmapVerticesCallback,
		MapIndices mapIndicesCallback,
		UnmapIndices unmapIndicesCallback,
		BeginRender beginRenderCallback,
		EndRender endRenderCallback,
		CreateTexture createTextureCallback,
		UpdateTexture updateTextureCallback,
		CreateRenderTarget createRenderTargetCallback,
		CloneRenderTarget cloneRenderTargetCallback,
		SetRenderTarget setRenderTargetCallback,
		BeginTile beginTileCallback,
		EndTile endTileCallback,
		ResolveRenderTarget resolveRenderTargetCallback)
	{
		pDevice->drawBatchCallback = drawBatchCallback;
		pDevice->mapVerticesCallback = mapVerticesCallback;
		pDevice->unmapVerticesCallback = unmapVerticesCallback;
		pDevice->mapIndicesCallback = mapIndicesCallback;
		pDevice->unmapIndicesCallback = unmapIndicesCallback;
		pDevice->beginRenderCallback = beginRenderCallback;
		pDevice->endRenderCallback = endRenderCallback;
		pDevice->createTextureCallback = createTextureCallback;
		pDevice->updateTextureCallback = updateTextureCallback;
		pDevice->createRenderTargetCallback = createRenderTargetCallback;
		pDevice->cloneRenderTargetCallback = cloneRenderTargetCallback;
		pDevice->setRenderTargetCallback = setRenderTargetCallback;
		pDevice->beginTileCallback = beginTileCallback;
		pDevice->endTileCallback = endTileCallback;
		pDevice->resolveRenderTargetCallback = resolveRenderTargetCallback;
	}
};
//----------------------------------------------------------------------------------------------------------------------------------------------------------------
//----------------------------------------------------------------------------------------------------------------------------------------------------------------
//----------------------------------------------------------------------------------------------------------------------------------------------------------------

Noesis::Ptr<Noesis::RenderTarget> ManagedRenderDevice::CreateRenderTarget(const char* label, uint32_t width, uint32_t height, uint32_t sampleCount)
{
	if (createRenderTargetCallback)
	{
		Noesis::Ptr<ManagedRenderTarget> surface = *new ManagedRenderTarget(width, height, flippedTextures);
		createRenderTargetCallback(surface, surface->texture, label,  width, height, sampleCount);
		return surface;
	}

	return 0;
}

Noesis::Ptr<Noesis::RenderTarget> ManagedRenderDevice::CloneRenderTarget(const char* label, Noesis::RenderTarget* surface)
{
	if (cloneRenderTargetCallback)
	{
		auto managedTexture = (ManagedTexture*)surface->GetTexture();
		Noesis::Ptr<ManagedRenderTarget> clonedSurface = *new ManagedRenderTarget(managedTexture->width, managedTexture->height, flippedTextures);
		cloneRenderTargetCallback(clonedSurface, clonedSurface->texture, label, surface);
		return clonedSurface;
	}

	return 0;
}

Noesis::Ptr<Noesis::Texture> ManagedRenderDevice::CreateTexture(const char* label, uint32_t width, uint32_t height, uint32_t numLevels, Noesis::TextureFormat::Enum format, const void** data)
{
	if (createTextureCallback)
	{
		Noesis::Ptr<ManagedTexture> texture = *new ManagedTexture(width, height, numLevels, flippedTextures);
		createTextureCallback(texture, label, width, height, numLevels, format);
		return texture;
	}

	return 0;
}

void ManagedRenderDevice::UpdateTexture(Noesis::Texture* texture, uint32_t level, uint32_t x, uint32_t y, uint32_t width, uint32_t height, const void* data)
{
	if (updateTextureCallback)
		updateTextureCallback((ManagedTexture*)texture, level, x, y, width, height, data);
}

void ManagedRenderDevice::BeginRender(bool offscreen)
{
	if (beginRenderCallback)
		beginRenderCallback(offscreen);
}

void ManagedRenderDevice::SetRenderTarget(Noesis::RenderTarget* surface)
{
	if (setRenderTargetCallback)
		setRenderTargetCallback((ManagedRenderTarget*)surface);
}

void ManagedRenderDevice::BeginTile(const Noesis::Tile& tile, uint32_t surfaceWidth, uint32_t surfaceHeight)
{
	if (beginTileCallback)
		beginTileCallback(tile, surfaceWidth, surfaceHeight);
}

void ManagedRenderDevice::EndTile()
{
	if (endTileCallback)
		endTileCallback();
}

void ManagedRenderDevice::ResolveRenderTarget(Noesis::RenderTarget* surface, const Noesis::Tile* tiles, uint32_t numTiles)
{
	if (resolveRenderTargetCallback)
		resolveRenderTargetCallback((ManagedRenderTarget*)surface, *tiles, numTiles);
}

void ManagedRenderDevice::EndRender()
{
	if (endRenderCallback)
		endRenderCallback();
}

void* ManagedRenderDevice::MapVertices(uint32_t bytes)
{
	if (mapVerticesCallback)
		return mapVerticesCallback(bytes);
	return 0;
}

void ManagedRenderDevice::UnmapVertices()
{
	if (unmapVerticesCallback)
		unmapVerticesCallback();
}

void* ManagedRenderDevice::MapIndices(uint32_t bytes)
{
	if (mapIndicesCallback)
		return mapIndicesCallback(bytes);
	return 0;
}

void ManagedRenderDevice::UnmapIndices()
{
	if (unmapIndicesCallback)
		unmapIndicesCallback();
}

void ManagedRenderDevice::DrawBatch(const Noesis::Batch& batch)
{
	if (drawBatchCallback)
		drawBatchCallback(batch);
}