#include "ManagedRenderDevice.h"

#pragma warning(disable : 4100)

using namespace Noesis;

#define DLL_FUNC __declspec(dllexport)
extern "C"
{
	DLL_FUNC ManagedRenderDevice* CreateManagedRenderDevice() { return new ManagedRenderDevice(); }
	DLL_FUNC void SetDrawBatchCallback    (ManagedRenderDevice* pDevice, DrawBatch func)     { pDevice->drawBatchCallback = func;}
	DLL_FUNC void SetMapVerticesCallback  (ManagedRenderDevice* pDevice, MapVertices func)   { pDevice->mapVerticesCallback = func; }
	DLL_FUNC void SetUnmapVerticesCallback(ManagedRenderDevice* pDevice, UnmapVertices func) { pDevice->unmapVerticesCallback = func; }
	DLL_FUNC void SetMapIndicesCallback   (ManagedRenderDevice* pDevice, MapVertices func)   { pDevice->mapIndicesCallback = func; }
	DLL_FUNC void SetUnmapIndicesCallback (ManagedRenderDevice* pDevice, UnmapVertices func) { pDevice->unmapIndicesCallback = func; }
	DLL_FUNC void SetBeginRenderCallback  (ManagedRenderDevice* pDevice, BeginRender func)   { pDevice->beginRenderCallback = func; }
	DLL_FUNC void SetEndRenderCallback    (ManagedRenderDevice* pDevice, EndRender func)     { pDevice->endRenderCallback = func; }
	DLL_FUNC void SetCreateTextureCallback(ManagedRenderDevice* pDevice, CreateTexture func) { pDevice->createTextureCallback = func; }
	DLL_FUNC void SetUpdateTextureCallback(ManagedRenderDevice* pDevice, UpdateTexture func) { pDevice->updateTextureCallback = func; }
}

//----------------------------------------------------------------------------------------------------------------------------------------------------------------
//----------------------------------------------------------------------------------------------------------------------------------------------------------------
//----------------------------------------------------------------------------------------------------------------------------------------------------------------


const Noesis::DeviceCaps& ManagedRenderDevice::GetCaps() const
{
	static Noesis::DeviceCaps caps;
	return caps;
}

Noesis::Ptr<Noesis::RenderTarget> ManagedRenderDevice::CreateRenderTarget(const char* label, uint32_t width, uint32_t height, uint32_t sampleCount)
{
	return Noesis::Ptr<Noesis::RenderTarget>();
}

Noesis::Ptr<Noesis::RenderTarget> ManagedRenderDevice::CloneRenderTarget(const char* label, Noesis::RenderTarget* surface)
{
	return Noesis::Ptr<Noesis::RenderTarget>();
}

Noesis::Ptr<Noesis::Texture> ManagedRenderDevice::CreateTexture(const char* label, uint32_t width, uint32_t height, uint32_t numLevels, Noesis::TextureFormat::Enum format, const void** data)
{
	if (createTextureCallback)
	{
		Noesis::Ptr<ManagedTexture> texture = *new ManagedTexture();
		texture->Update(width, height, numLevels);
		texture->SetIsInverted(false);
		createTextureCallback(texture, width, height, numLevels, format);
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
		beginRenderCallback();
}

void ManagedRenderDevice::SetRenderTarget(Noesis::RenderTarget* surface)
{

}

void ManagedRenderDevice::BeginTile(const Noesis::Tile& tile, uint32_t surfaceWidth, uint32_t surfaceHeight)
{

}

void ManagedRenderDevice::EndTile()
{

}

void ManagedRenderDevice::ResolveRenderTarget(Noesis::RenderTarget* surface, const Noesis::Tile* tiles, uint32_t numTiles)
{

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