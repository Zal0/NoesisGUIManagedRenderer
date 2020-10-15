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

#define DLL_FUNC __declspec(dllexport)
extern "C"
{

///////////////////////////////////////////////////////////////////////////////////////////////////
DLL_FUNC void NoesisInit(const char* licenseName, const char* licenseKey)
{
    Noesis::SetLogHandler([](const char*, uint32_t, uint32_t level, const char*, const char* msg)
    {
        // [TRACE] [DEBUG] [INFO] [WARNING] [ERROR]
        const char* prefixes[] = { "T", "D", "I", "W", "E" };
        printf("[NOESIS/%s] %s\n", prefixes[level], msg);
    });

    // Noesis initialization. This must be the first step before using any NoesisGUI functionality
    Noesis::GUI::Init(licenseName, licenseKey);

    // Setup theme
    NoesisApp::SetThemeProviders();
    Noesis::GUI::LoadApplicationResources("Theme/NoesisTheme.DarkBlue.xaml");
}

DLL_FUNC Noesis::IView* CreateView(Noesis::RenderDevice* renderDevice, const char* xamlString)
{
    // For simplicity purposes we are not using resource providers in this sample. ParseXaml() is
    // enough if there is no extra XAML dependencies
    Noesis::Ptr<Noesis::Grid> xaml(Noesis::GUI::ParseXaml<Noesis::Grid>(xamlString));

    // View creation to render and interact with the user interface
    // We transfer the ownership to a global pointer instead of a Ptr<> because there is no way
    // in GLUT to do shutdown and we don't want the Ptr<> to be released at global time
    auto view = Noesis::GUI::CreateView(xaml).GiveOwnership();
    view->SetFlags(Noesis::RenderFlags_PPAA | Noesis::RenderFlags_LCD);

    // Renderer initialization with the render device
    view->GetRenderer()->Init(renderDevice);
    return view;
}

DLL_FUNC void DeleteView(Noesis::IView* view)
{
    delete view;
}

DLL_FUNC void UpdateView(Noesis::IView* view, float t)
{
    // Update view (layout, animations, ...)
    view->Update(t);
}

DLL_FUNC void RenderView(Noesis::IView* view)
{
    // Offscreen rendering phase populates textures needed by the on-screen rendering
    view->GetRenderer()->UpdateRenderTree();
    view->GetRenderer()->RenderOffscreen();

    view->GetRenderer()->Render();
}

DLL_FUNC void SetViewSize(Noesis::IView* view, int w, int h)
{
    view->SetSize(w, h);
}

DLL_FUNC bool ViewMouseButtonDown(Noesis::IView* view, int x, int y, Noesis::MouseButton button)
{
    return view->MouseButtonDown(x, y, button);
}

DLL_FUNC bool ViewMouseButtonUp(Noesis::IView* view, int x, int y, Noesis::MouseButton button)
{
    return view->MouseButtonUp(x, y, button);
}

DLL_FUNC bool ViewMouseDoubleClick(Noesis::IView* view, int x, int y, Noesis::MouseButton button)
{
    return view->MouseDoubleClick(x, y, button);
}

DLL_FUNC bool ViewMouseMove(Noesis::IView* view, int x, int y)
{
    return view->MouseMove(x, y);
}

DLL_FUNC bool ViewMouseWheel(Noesis::IView* view, int x, int y, int wheelRotation)
{
    return view->MouseWheel(x, y, wheelRotation);
}

DLL_FUNC bool ViewMouseHWheel(Noesis::IView* view, int x, int y, int wheelRotation)
{
    return view->MouseHWheel(x, y, wheelRotation);
}

DLL_FUNC bool ViewScroll(Noesis::IView* view, float value)
{
    return view->Scroll(value);
}

DLL_FUNC bool ViewScrollPos(Noesis::IView* view, int x, int y, float value)
{
    return view->Scroll(x, y, value);
}

DLL_FUNC bool ViewHScroll(Noesis::IView* view, float value)
{
    return view->HScroll(value);
}

DLL_FUNC bool ViewHScrollPos(Noesis::IView* view, int x, int y, float value)
{
    return view->HScroll(x, y, value);
}

DLL_FUNC bool ViewTouchDown(Noesis::IView* view, int x, int y, uint64_t id)
{
    return view->TouchDown(x, y, id);
}

DLL_FUNC bool ViewTouchMove(Noesis::IView* view, int x, int y, uint64_t id)
{
    return view->TouchMove(x, y, id);
}

DLL_FUNC bool ViewTouchUp(Noesis::IView* view, int x, int y, uint64_t id)
{
    return view->TouchUp(x, y, id);
}

DLL_FUNC bool ViewKeyDown(Noesis::IView* view, Noesis::Key key)
{
    return view->KeyDown(key);
}

DLL_FUNC bool ViewKeyUp(Noesis::IView* view, Noesis::Key key)
{
    return view->KeyUp(key);
}

DLL_FUNC bool ViewChar(Noesis::IView* view, uint32_t ch)
{
    return view->Char(ch);
}


}