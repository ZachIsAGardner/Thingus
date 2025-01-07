using System.Numerics;
using Raylib_cs;

namespace Thingus;

public static class Viewport
{
    public static List<ViewportLayer> Layers = new List<ViewportLayer>() { };
    public static RelativeViewportLayer RelativeLayer = null;
    public static LightViewportLayer LightLayer = null;
    public static AbsoluteViewportLayer AbsoluteLayer = null;

    public static Vector2 Position;
    public static Vector2 Scale;
    public static float Rotation;
    public static Vector2 Offset;

    public static ViewportMode Mode = ViewportMode.FitToScreen;
    public static Color ClearColor = PaletteBasic.Black;
    public static bool ScalePixels = false;
    public static Thing Target = null;
    public static Vector2 CameraPosition { get; private set; }

    // public static Rectangle VisibleArea;
    public static Vector2 Margin;
    public static float AdjustFromLeft => Margin.X < 0 
        ? -Margin.X
        : 0;
    public static float AdjustFromRight => Margin.X < 0 
        ? Margin.X
        : 0;
    public static float AdjustFromTop => Margin.Y < 0 
        ? -Margin.Y
        : 0;
    public static float AdjustFromBottom => Margin.Y < 0 
        ? Margin.Y
        : 0;
    public static Vector2 AdjustFromBottomLeft => new Vector2(
        Margin.X == 0 ? 0 : AdjustFromLeft,
        Margin.Y == 0 ? 0 : AdjustFromBottom
    );
    public static Vector2 AdjustFromBottomRight => new Vector2(
        Margin.X == 0 ? 0 : AdjustFromRight,
        Margin.Y == 0 ? 0 : AdjustFromBottom
    );
    public static Vector2 AdjustFromTopLeft => new Vector2(
        Margin.X == 0 ? 0 : AdjustFromLeft,
        Margin.Y == 0 ? 0 : AdjustFromTop
    );
    public static Vector2 AdjustFromTopRight => new Vector2(
        Margin.X == 0 ? 0 : AdjustFromRight,
        Margin.Y == 0 ? 0 : AdjustFromTop
    );

    public static Vector2 Adjust(Vector2 position, AdjustFrom adjustFrom = AdjustFrom.TopLeft)
    { 
        if (adjustFrom != AdjustFrom.None)
        {
            // Left
            if ((adjustFrom == AdjustFrom.Auto && position.X < CONSTANTS.VIRTUAL_WIDTH / 2f)
                || adjustFrom == AdjustFrom.TopLeft
                || adjustFrom == AdjustFrom.Left
                || adjustFrom == AdjustFrom.BottomLeft)
            {
                position.X += AdjustFromLeft;
            }
            // Right
            else if ((adjustFrom == AdjustFrom.Auto && position.X >= CONSTANTS.VIRTUAL_WIDTH / 2f) 
                || adjustFrom == AdjustFrom.TopRight 
                || adjustFrom == AdjustFrom.Right 
                || adjustFrom == AdjustFrom.BottomRight)
            {
                position.X += AdjustFromRight;
            } 

            // Top
            if ((adjustFrom == AdjustFrom.Auto && position.Y < CONSTANTS.VIRTUAL_HEIGHT / 2f)
                || adjustFrom == AdjustFrom.TopLeft
                || adjustFrom == AdjustFrom.Top
                || adjustFrom == AdjustFrom.TopRight)
            {
                position.Y += AdjustFromTop;
            }
            // Bottom
            else if ((adjustFrom == AdjustFrom.Auto && position.Y >= CONSTANTS.VIRTUAL_HEIGHT / 2f) 
                || adjustFrom == AdjustFrom.BottomLeft 
                || adjustFrom == AdjustFrom.Bottom 
                || adjustFrom == AdjustFrom.BottomRight)
            {
                position.Y += AdjustFromBottom;
            } 
        }

        return position;
    }

    public static float Allowance = 0.9f;

    public static int? VirtualRatio = 1;
    public static int? LastVirtualRatio = 1;
    public static float Zoom = 1;
    public static Rectangle Rectangle = new Rectangle();

    public static Shader Shader = null;

    public static void Start()
    {
        Scale = new Vector2(CONSTANTS.VIRTUAL_WIDTH, CONSTANTS.VIRTUAL_HEIGHT);
        // VisibleArea = new Rectangle(0, 0, CONSTANTS.VIRTUAL_HEIGHT, CONSTANTS.VIRTUAL_HEIGHT);
        RefreshProjection();

        RelativeLayer = new RelativeViewportLayer(0);
        Layers.Add(RelativeLayer);
        LightLayer = new LightViewportLayer(5);
        Layers.Add(LightLayer);
        AbsoluteLayer = new AbsoluteViewportLayer(10);
        Layers.Add(AbsoluteLayer);
    }

    public static void Update()
    {
        if (Raylib.IsWindowResized()) RefreshProjection();

        if (Target != null)
        {
            CameraPosition = Target.Position
                + Target.Offset
                + Position
                - new Vector2(
                    CONSTANTS.VIRTUAL_WIDTH / 2f,
                    CONSTANTS.VIRTUAL_HEIGHT / 2f
                );
        }
        else
        {
            CameraPosition = Position;
        }


        Layers.ForEach(l => l.Update());
    }

    static void DrawToLayers()
    {
        Layers.Where(l => l.Visible).ToList().ForEach(l => l.DrawToTexture());
    }

    static void DrawToWindow()
    {
        if (Shader != null)
        {
            Raylib.BeginShaderMode(Shader.ToRaylib());
            Shader.Update();
        }
        Layers.Where(l => l.Visible).OrderBy(l => l.Order).ToList().ForEach(l => l.DrawToWindow());

        if (Shader != null) Raylib.EndShaderMode();
    }

    public static void Draw()
    {
        Raylib.ClearBackground(ClearColor.ToRaylib());
        Game.DrawThings.ForEach(t => t.DrawToTexture());
        DrawToLayers();
        DrawToWindow();
    }

    // ---

    public static void RefreshProjection()
    {
        Scale.X = Raylib.GetScreenWidth();
        Scale.Y = Raylib.GetScreenHeight();

        LastVirtualRatio = VirtualRatio.Value;

        if (Mode == ViewportMode.FitToScreen)
        {
            Rectangle.Width = CONSTANTS.VIRTUAL_WIDTH;
            Rectangle.Height = CONSTANTS.VIRTUAL_HEIGHT;

            int virtualRatioX = (int)(Scale.X / (CONSTANTS.VIRTUAL_WIDTH * Allowance)).Floor();
            int virtualRatioY = (int)(Scale.Y / (CONSTANTS.VIRTUAL_HEIGHT * Allowance)).Floor();
            VirtualRatio = Math.Max(Math.Min(virtualRatioX, virtualRatioY), 1);

            Rectangle.Width = CONSTANTS.VIRTUAL_WIDTH * VirtualRatio.Value;
            Rectangle.Height = CONSTANTS.VIRTUAL_HEIGHT * VirtualRatio.Value;

            // Center rectangle in window
            Rectangle.X = Position.X = (int)((Scale.X - Rectangle.Width) / 2);
            Rectangle.Y = Position.Y = (int)((Scale.Y - Rectangle.Height) / 2);

            Margin = new Vector2(
                Position.X / VirtualRatio.Value,
                Position.Y / VirtualRatio.Value
            );
        }
        else if (Mode == ViewportMode.Free)
        {
            Rectangle.X = Position.X;
            Rectangle.Y = Position.Y;
            Rectangle.Width = Scale.X;
            Rectangle.Height = Scale.Y;
            VirtualRatio = null;
        }

        Layers.ForEach(l => l.RefreshProjection());
        Game.DrawThings.ForEach(t => t.RefreshProjection());
    }

    public static void SetScalePixels(bool value)
    {
        ScalePixels = value;
        // Log.Write("ScalePixels: " + ScalePixels);
        RefreshProjection();
    }
}
