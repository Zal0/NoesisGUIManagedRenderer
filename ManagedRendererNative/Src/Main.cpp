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

#include "ManagedRenderDevice/ManagedRenderDevice.h"

static Noesis::IView* _view;

#define DLL_FUNC __declspec(dllexport)
extern "C"
{

///////////////////////////////////////////////////////////////////////////////////////////////////
DLL_FUNC void NoesisInit()
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
    Noesis::Ptr<Noesis::Grid> xaml(Noesis::GUI::ParseXaml<Noesis::Grid>(R"(
        <Grid xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
            <Grid.Background>
                <LinearGradientBrush StartPoint="0,0" EndPoint="0,1">
                    <GradientStop Offset="0" Color="#FF123F61"/>
                    <GradientStop Offset="0.6" Color="#FF0E4B79"/>
                    <GradientStop Offset="0.7" Color="#FF106097"/>
                </LinearGradientBrush>
            </Grid.Background>
            <Viewbox>
                <StackPanel Margin="50">
                    <Button Content="Hello World!" Margin="0,30,0,0"/>
                    <Rectangle Height="5" Margin="-10,20,-10,0">
                        <Rectangle.Fill>
                            <RadialGradientBrush>
                                <GradientStop Offset="0" Color="#40000000"/>
                                <GradientStop Offset="1" Color="#00000000"/>
                            </RadialGradientBrush>
                        </Rectangle.Fill>
                    </Rectangle>
                </StackPanel>
            </Viewbox>
        </Grid>
    )"));

    // View creation to render and interact with the user interface
    // We transfer the ownership to a global pointer instead of a Ptr<> because there is no way
    // in GLUT to do shutdown and we don't want the Ptr<> to be released at global time
    _view = Noesis::GUI::CreateView(xaml).GiveOwnership();
    _view->SetFlags(Noesis::RenderFlags_PPAA | Noesis::RenderFlags_LCD);

    // Renderer initialization with an OpenGL device
    //_view->GetRenderer()->Init(NoesisApp::GLFactory::CreateDevice(false));
    _view->GetRenderer()->Init(new ManagedRenderDevice());
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

DLL_FUNC void ViewMouseMove(int x, int y)
{
    _view->MouseMove(x, y);
}

DLL_FUNC void ViewMouseButtonDown(int x, int y)
{
    _view->MouseButtonDown(x, y, Noesis::MouseButton_Left);
}

DLL_FUNC void ViewMouseButtonUp(int x, int y)
{
    _view->MouseButtonUp(x, y, Noesis::MouseButton_Left);
}

}