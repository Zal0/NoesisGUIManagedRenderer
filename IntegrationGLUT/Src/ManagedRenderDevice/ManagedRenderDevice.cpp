#include "ManagedRenderDevice.h"

#include <NsRender/Texture.h>

using namespace Noesis;

typedef void(*DrawBatch)(const Noesis::Batch& batch);

DrawBatch drawBatchCallback = 0;

#define DLL_FUNC __declspec(dllexport)
extern "C"
{
	DLL_FUNC void SetDrawBatchCallback(DrawBatch func) { drawBatchCallback = func;}
}

//----------------------------------------------------------------------------------------------------------------------------------------------------------------
//----------------------------------------------------------------------------------------------------------------------------------------------------------------
//----------------------------------------------------------------------------------------------------------------------------------------------------------------

class ManagedTexture : public Texture
{
private:
	int width;
	int height;
	int numLevels;
	bool isInverted;

public:
	ManagedTexture(int width, int height, int numLevels, bool isInverted) :
		width(width), height(height), numLevels(numLevels), isInverted(isInverted)
	{
	}

	/// Returns the width of the texture
	virtual uint32_t GetWidth() const
	{
		return width;
	}

	/// Returns the height of the texture
	virtual uint32_t GetHeight() const
	{
		return height;
	}

	/// True if the texture has mipmaps
	virtual bool HasMipMaps() const
	{
		return numLevels > 1;
	}

	/// True is the texture must be vertically inverted when mapped. This is true for render targets
	/// on platforms (OpenGL) where texture V coordinate is zero at the "bottom of the texture"
	virtual bool IsInverted() const
	{
		return isInverted;
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
	Ptr<ManagedTexture> texture = *new ManagedTexture(width, height, numLevels, false);
	return texture;
}

void ManagedRenderDevice::UpdateTexture(Noesis::Texture* texture, uint32_t level, uint32_t x, uint32_t y, uint32_t width, uint32_t height, const void* data)
{

}

void ManagedRenderDevice::BeginRender(bool offscreen)
{

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

}

uint32_t* vertices = 0;
void* ManagedRenderDevice::MapVertices(uint32_t bytes)
{
	if (vertices)
	{
		delete[] vertices;
	}

	vertices = new uint32_t[bytes];
	return vertices;
}

void ManagedRenderDevice::UnmapVertices()
{
	delete[] vertices;
	vertices = 0;
}

uint32_t* indices = 0;
void* ManagedRenderDevice::MapIndices(uint32_t bytes)
{
	if (indices)
	{
		delete[] indices;
	}

	indices = new uint32_t[bytes];
	return indices;
}

void ManagedRenderDevice::UnmapIndices()
{
	delete[] indices;
	indices = 0;
}

void ManagedRenderDevice::DrawBatch(const Noesis::Batch& batch)
{
	if (drawBatchCallback)
	{
		drawBatchCallback(batch);
	}
}