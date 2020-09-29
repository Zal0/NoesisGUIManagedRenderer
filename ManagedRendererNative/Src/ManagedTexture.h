#ifndef MANAGED_TEXTURE_H
#define MANAGED_TEXTURE_H

#include <NsRender/Texture.h>
#include <NsRender/RenderDevice.h>

class ManagedTexture : public Noesis::Texture
{
private:
	uint32_t mWidth;
	uint32_t mHeight;
	uint32_t mNumLevels;
	bool mIsInverted;

public:

	~ManagedTexture()
	{
	}

	/// Returns the width of the texture
	virtual uint32_t GetWidth() const { return mWidth; }

	/// Returns the height of the texture
	virtual uint32_t GetHeight() const { return mHeight; }

	/// True if the texture has mipmaps
	virtual bool HasMipMaps() const { return mNumLevels > 1; }

	/// True is the texture must be vertically inverted when mapped. This is true for render targets
	/// on platforms (OpenGL) where texture V coordinate is zero at the "bottom of the texture"
	virtual bool IsInverted() const { return mIsInverted; }

	void SetIsInverted(bool value) { mIsInverted = value; }

	void Update(uint32_t width, uint32_t height, uint32_t numLevels)
	{
		mWidth = width;
		mHeight = height;
		mNumLevels = numLevels;
	}
};
#endif