////////////////////////////////////////////////////////////////////////////////////////////////////
// NoesisGUI - http://www.noesisengine.com
// Copyright (c) 2013 Noesis Technologies S.L. All Rights Reserved.
////////////////////////////////////////////////////////////////////////////////////////////////////


// This is a minimal integration example. For simplification purposes only basic input events are
// handled, no resource providers are used and the shutdown procedure is omitted. A more complete
// multiplatform integration sample with step by step comments can be found at 'Samples/Integration'


#ifdef _MSC_VER
#define UNUSED_ARGS(...) (void)(true ? (void)0 : ((void)(__VA_ARGS__)))
#else
#define UNUSED_ARGS(...)
#endif


#include <NsApp/ThemeProviders.h>
#include <NsRender/GLFactory.h>
#include <NsGui/FontProperties.h>
#include <NsGui/IntegrationAPI.h>
#include <NsGui/IRenderer.h>
#include <NsGui/IView.h>
#include <NsGui/Grid.h>

#include "ManagedRenderDevice.h"

static Noesis::IView* _view;

#define DLL_FUNC __declspec(dllexport)
extern "C"
{

///////////////////////////////////////////////////////////////////////////////////////////////////
DLL_FUNC void NoesisInit(Noesis::RenderDevice* renderDevice, const char* xamlString)
{
    Noesis::SetLogHandler([](const char*, uint32_t, uint32_t level, const char*, const char* msg)
    {
        // [TRACE] [DEBUG] [INFO] [WARNING] [ERROR]
        const char* prefixes[] = { "T", "D", "I", "W", "E" };
        printf("[NOESIS/%s] %s\n", prefixes[level], msg);
    });

    // Noesis initialization. This must be the first step before using any NoesisGUI functionality
    Noesis::GUI::Init(NS_LICENSE_NAME, NS_LICENSE_KEY);

    // Setup theme
    NoesisApp::SetThemeProviders();
    Noesis::GUI::LoadApplicationResources("Theme/NoesisTheme.DarkBlue.xaml");

    // For simplicity purposes we are not using resource providers in this sample. ParseXaml() is
    // enough if there is no extra XAML dependencies
    Noesis::Ptr<Noesis::Grid> xaml(Noesis::GUI::ParseXaml<Noesis::Grid>(xamlString));

    // View creation to render and interact with the user interface
    // We transfer the ownership to a global pointer instead of a Ptr<> because there is no way
    // in GLUT to do shutdown and we don't want the Ptr<> to be released at global time
    _view = Noesis::GUI::CreateView(xaml).GiveOwnership();
    _view->SetFlags(Noesis::RenderFlags_PPAA | Noesis::RenderFlags_LCD);

    // Renderer initialization with the render device
    _view->GetRenderer()->Init(renderDevice);
}

DLL_FUNC void UpdateView(float t)
{
    // Update view (layout, animations, ...)
    _view->Update(t);

    // Offscreen rendering phase populates textures needed by the on-screen rendering
    _view->GetRenderer()->UpdateRenderTree();
    _view->GetRenderer()->RenderOffscreen();
}

DLL_FUNC void RenderView()
{
    _view->GetRenderer()->Render();
}

DLL_FUNC void SetViewSize(int w, int h)
{
    _view->SetSize(w, h);
}

DLL_FUNC bool ViewMouseButtonDown(int x, int y, Noesis::MouseButton button)
{
    return _view->MouseButtonDown(x, y, button);
}

DLL_FUNC bool ViewMouseButtonUp(int x, int y, Noesis::MouseButton button)
{
    return _view->MouseButtonUp(x, y, button);
}

DLL_FUNC bool ViewMouseDoubleClick(int x, int y, Noesis::MouseButton button)
{
    return _view->MouseDoubleClick(x, y, button);
}

DLL_FUNC bool ViewMouseMove(int x, int y)
{
    return _view->MouseMove(x, y);
}

DLL_FUNC bool ViewMouseWheel(int x, int y, int wheelRotation)
{
    return _view->MouseWheel(x, y, wheelRotation);
}

DLL_FUNC bool ViewMouseHWheel(int x, int y, int wheelRotation)
{
    return _view->MouseHWheel(x, y, wheelRotation);
}

DLL_FUNC bool ViewScroll(float value)
{
    return _view->Scroll(value);
}

DLL_FUNC bool ViewScrollPos(int x, int y, float value)
{
    return _view->Scroll(x, y, value);
}

DLL_FUNC bool ViewHScroll(float value)
{
    return _view->HScroll(value);
}

DLL_FUNC bool ViewHScrollPos(int x, int y, float value)
{
    return _view->HScroll(x, y, value);
}

DLL_FUNC bool ViewTouchDown(int x, int y, uint64_t id)
{
    return _view->TouchDown(x, y, id);
}

DLL_FUNC bool ViewTouchMove(int x, int y, uint64_t id)
{
    return _view->TouchMove(x, y, id);
}

DLL_FUNC bool ViewTouchUp(int x, int y, uint64_t id)
{
    return _view->TouchUp(x, y, id);
}

DLL_FUNC bool ViewKeyDown(Noesis::Key key)
{
    return _view->KeyDown(key);
}

DLL_FUNC bool ViewKeyUp(Noesis::Key key)
{
    return _view->KeyUp(key);
}

DLL_FUNC bool ViewChar(uint32_t ch)
{
    return _view->Char(ch);
}


}