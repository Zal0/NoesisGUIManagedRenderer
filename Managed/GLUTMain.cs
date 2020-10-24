using NoesisManagedRenderer;
using System;
using System.Collections.Generic;


class GLUTMain
{
    private List<IntPtr> noesisViews = new List<IntPtr>();

    private const string xamlString = @"<Grid xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
            <Grid.Background>
                <LinearGradientBrush StartPoint=""0,0"" EndPoint=""0,1"">
                    <GradientStop Offset=""0"" Color=""#FF123F61""/>
                    <GradientStop Offset=""0.6"" Color=""#FF0E4B79""/>
                    <GradientStop Offset=""0.7"" Color=""#FF106097""/>
                </LinearGradientBrush>
            </Grid.Background>
            <Viewbox>
                <StackPanel Margin=""50"">
                    <Button Content=""Hello World!"" Margin=""0,30,0,0""/>
                    <Rectangle Height=""5"" Margin=""-10,20,-10,0"">
                        <Rectangle.Fill>
                            <RadialGradientBrush>
                                <GradientStop Offset=""0"" Color=""#40000000""/>
                                <GradientStop Offset=""1"" Color=""#00000000""/>
                            </RadialGradientBrush>
                        </Rectangle.Fill>
                    </Rectangle>
                </StackPanel>
            </Viewbox>
</Grid>
";

    public static void Main(string[] args)
    {
        GLUTMain instance = new GLUTMain();
        instance.Run(args);
    }

    public void Run(string[] _args)
    {
        GLUT.Init();
        GLUT.InitDisplayMode(GLUT.GLUT_RGB | GLUT.GLUT_DOUBLE | GLUT.GLUT_STENCIL);
        GLUT.InitWindowSize(1000, 600);
        GLUT.CreateWindow("NoesisGUI - Managed Renderer");

        var renderer = new GLUTRenderer();

        NoesisApp.NoesisInit(string.Empty, string.Empty);

        this.noesisViews.Add(NoesisApp.CreateView(renderer, xamlString));

        GLUT.DisplayFunc(DisplayFunc);
        GLUT.ReshapeFunc(ReshapeFunc);
        GLUT.PassiveMotionFunc(MouseMoveFunc);
        GLUT.MouseFunc(MouseFunc);

        GLUT.MainLoop();
    }

    private void DisplayFunc()
    {
        foreach (var view in this.noesisViews)
        {
            NoesisApp.UpdateView(view, GLUT.Get(GLUT.GLUT_ELAPSED_TIME) / 1000.0f);
        }

        //GL.BindFramebuffer(GL.GL_FRAMEBUFFER, 0);
        GL.Viewport(0, 0, GLUT.Get(GLUT.GLUT_WINDOW_WIDTH), GLUT.Get(GLUT.GLUT_WINDOW_HEIGHT));

        GL.ClearColor(0.0f, 0.0f, 0.25f, 0.0f);
        GL.ClearStencil(0);
        GL.Clear(GL.GL_COLOR_BUFFER_BIT | GL.GL_STENCIL_BUFFER_BIT);

        //GL.PolygonMode(GL.GL_FRONT_AND_BACK, GL.GL_LINE);

        foreach (var view in this.noesisViews)
        {
            NoesisApp.RenderView(view);
        }

        GLUT.SwapBuffers();
        GLUT.PostRedisplay();
    }

    private void ReshapeFunc(int w, int h)
    {
        foreach (var view in this.noesisViews)
        {
            NoesisApp.SetViewSize(view, w, h);
        }

        GL.MatrixMode(GL.GL_PROJECTION);
        GL.LoadIdentity();
        GL.Ortho(0.0f, w, 0.0f, h, 0.0f, 1.0f);
        GL.MatrixMode(GL.GL_MODELVIEW);
    }

    private void MouseFunc(int button, int state, int x, int y)
    {
        if (button == GLUT.GLUT_LEFT_BUTTON)
        {
            if (state == GLUT.GLUT_DOWN)
            {
                foreach (var view in this.noesisViews)
                {
                    NoesisApp.ViewMouseButtonDown(view, x, y, button);
                }
            }
            else
            {
                foreach (var view in this.noesisViews)
                {
                    NoesisApp.ViewMouseButtonUp(view, x, y, button);
                }
            }
        }
    }

    private void MouseMoveFunc(int x, int y)
    {
        foreach (var view in this.noesisViews)
        {
            NoesisApp.ViewMouseMove(view, x, y);
        }
    }
}

