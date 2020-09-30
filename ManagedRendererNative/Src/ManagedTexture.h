#ifndef MANAGED_TEXTURE_H
#define MANAGED_TEXTURE_H

#include <NsRender/Texture.h>

class ManagedTexture final : public  Noesis::Texture
{
public:
	ManagedTexture(uint32_t width_, uint32_t height_,
		uint32_t levels_, bool isInverted_) : width(width_), height(height_),
		levels(levels_), isInverted(isInverted_) {}

	~ManagedTexture()
	{
		// TODO: destroyTextureCallback
	}

	uint32_t GetWidth() const override { return width; }
	uint32_t GetHeight() const override { return height; }
	bool HasMipMaps() const override { return levels > 1; }
	bool IsInverted() const override { return isInverted; }

	const uint32_t width;
	const uint32_t height;
	const uint32_t levels;
	const bool isInverted;
};
#endif