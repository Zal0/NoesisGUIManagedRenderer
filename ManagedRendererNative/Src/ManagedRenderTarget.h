#ifndef MANAGED_RENDER_TARGET_H
#define MANAGED_RENDER_TARGET_H

class ManagedRenderTarget final : public  Noesis::RenderTarget
{
public:
	ManagedRenderTarget(uint32_t width_, uint32_t height_, bool flippedTextures_)
	{
		texture = *new ManagedTexture(width_, height_, 1, flippedTextures_);
	}

	~ManagedRenderTarget()
	{
		delete texture;

		// TODO: destroyRenderTargetCallback
	}

	Noesis::Ptr<ManagedTexture> texture;

	Noesis::Texture* GetTexture() override { return texture; }
};
#endif