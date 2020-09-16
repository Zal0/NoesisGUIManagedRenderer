#include "ManagedRenderDevice.h"

#include <NsRender/Texture.h>
#include <NsRender/RenderTarget.h>

#pragma warning(disable : 4100)

using namespace Noesis;

//Render device callbacks
typedef void(*DrawBatch)(const Noesis::Batch& batch);
typedef void*(*MapVertices)(uint32_t bytes);
typedef void(*UnmapVertices)();
typedef void* (*MapIndices)(uint32_t bytes);
typedef void(*UnmapIndices)();
typedef void(*BeginRender)();
typedef void(*EndRender)();

DrawBatch drawBatchCallback = 0;
MapVertices mapVerticesCallback = 0;
UnmapVertices unmapVerticesCallback = 0;
MapIndices mapIndicesCallback = 0;
UnmapIndices unmapIndicesCallback = 0;
BeginRender beginRenderCallback = 0;
EndRender endRenderCallback = 0;

#define DLL_FUNC __declspec(dllexport)
extern "C"
{
	DLL_FUNC void SetDrawBatchCallback    (DrawBatch func)     { drawBatchCallback = func;}
	DLL_FUNC void SetMapVerticesCallback  (MapVertices func)   { mapVerticesCallback = func; }
	DLL_FUNC void SetUnmapVerticesCallback(UnmapVertices func) { unmapVerticesCallback = func; }
	DLL_FUNC void SetMapIndicesCallback   (MapVertices func)   { mapIndicesCallback = func; }
	DLL_FUNC void SetUnmapIndicesCallback (UnmapVertices func) { unmapIndicesCallback = func; }
	DLL_FUNC void SetBeginRenderCallback  (BeginRender func)   { beginRenderCallback = func; }
	DLL_FUNC void SetEndRenderCallback    (EndRender func)     { endRenderCallback = func; }
}

//Texture callbacks
class ManagedTexture;
typedef void(*CreateTexture)(const ManagedTexture* ptr, uint32_t width, uint32_t height, uint32_t numLevels, Noesis::TextureFormat::Enum format, const void** data);
typedef int(*GetWidth)(const ManagedTexture* ptr);
typedef int(*GetHeight)(const ManagedTexture* ptr);
typedef bool(*HasMipMaps)(const ManagedTexture* ptr);
typedef bool(*IsInverted)(const ManagedTexture* ptr);
typedef void(*UpdateTexture)(const ManagedTexture* ptr, uint32_t level, uint32_t x, uint32_t y, uint32_t width, uint32_t height, const void* data);

CreateTexture createTextureCallbak = 0;
GetWidth getWidth = 0;
GetHeight getHeight = 0;
HasMipMaps hasMipMaps = 0;
IsInverted isInverted = 0;
UpdateTexture updateTexture = 0;

extern "C"
{
	DLL_FUNC void SetCreateTextureCallback(CreateTexture func) { createTextureCallbak = func; }
	DLL_FUNC void SetGetWidthCallback     (GetWidth func)      { getWidth = func; }
	DLL_FUNC void SetGetHeightCallback    (GetHeight func)     { getHeight = func; }
	DLL_FUNC void SetHasMipMapsCallback   (HasMipMaps func)    { hasMipMaps = func; }
	DLL_FUNC void SetIsInvertedCallback   (IsInverted func)    { isInverted = func; }
	DLL_FUNC void SetUpdateTextureCallback(UpdateTexture func) { updateTexture = func; }
}

//----------------------------------------------------------------------------------------------------------------------------------------------------------------
//----------------------------------------------------------------------------------------------------------------------------------------------------------------
//----------------------------------------------------------------------------------------------------------------------------------------------------------------

class ManagedTexture : public Texture
{
public:
	ManagedTexture(const char* label, uint32_t width, uint32_t height, uint32_t numLevels, Noesis::TextureFormat::Enum format, const void** data)
	{
		createTextureCallbak(this, width, height, numLevels, format, data);
	}

	/// Returns the width of the texture
	virtual uint32_t GetWidth() const
	{
		return getWidth(this);
	}

	/// Returns the height of the texture
	virtual uint32_t GetHeight() const
	{
		return getHeight(this);
	}

	/// True if the texture has mipmaps
	virtual bool HasMipMaps() const
	{
		return hasMipMaps(this);
	}

	/// True is the texture must be vertically inverted when mapped. This is true for render targets
	/// on platforms (OpenGL) where texture V coordinate is zero at the "bottom of the texture"
	virtual bool IsInverted() const
	{
		return isInverted(this);
	}
};

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
	Ptr<ManagedTexture> texture = *new ManagedTexture(label, width, height, numLevels, format, data);
	return texture;
}

void ManagedRenderDevice::UpdateTexture(Noesis::Texture* texture, uint32_t level, uint32_t x, uint32_t y, uint32_t width, uint32_t height, const void* data)
{
	updateTexture((ManagedTexture*)texture, level, x, y, width, height, data);
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