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

    public static ViewportMode Mode = ViewportMode.FitToScreen;
    public static Color ClearColor = Colors.White;
    public static bool ScalePixels = false;
    public static Thing Target = null;
    public static Vector2 CameraPosition { get; private set; }

    // public static Rectangle VisibleArea;
    public static Vector2 Margin;
    public static Vector2 Adjust => new Vector2(-Viewport.Margin.X * (1 + Viewport.VirtualRatio.Value), -Viewport.Margin.Y * (1 + Viewport.VirtualRatio.Value));
    public static Vector2 AdjustFromBottomLeft => new Vector2(
        Margin.X == 0 ? 0
            : Margin.X < 0 ? -Margin.X * (1 + VirtualRatio.Value) : -Margin.X * (VirtualRatio.Value),
        Margin.Y == 0 ? 0
            : Margin.Y < 0 ? -Margin.Y * (VirtualRatio.Value - 1) : -Margin.Y * (VirtualRatio.Value)
    );
    public static Vector2 AdjustFromBottomRight => new Vector2(
        Margin.X == 0 ? 0
            : Margin.X < 0 ? -Margin.X * (VirtualRatio.Value - 1) : -Margin.X * (VirtualRatio.Value),
        Margin.Y == 0 ? 0
            : Margin.Y < 0 ? -Margin.Y * (VirtualRatio.Value - 1) : -Margin.Y * (VirtualRatio.Value)
    );
    public static Vector2 AdjustFromTopLeft => new Vector2(
        Margin.X == 0 ? 0
            : Margin.X < 0 ? -Margin.X * (1 + VirtualRatio.Value) : -Margin.X * (VirtualRatio.Value),
        Margin.Y == 0 ? 0
            : Margin.Y < 0 ? -Margin.Y * (2 + VirtualRatio.Value - 1) : -Margin.Y * (1 + VirtualRatio.Value - 1)
    );
    public static Vector2 AdjustFromTopRight => new Vector2(
        Margin.X == 0 ? 0
            : Margin.X < 0 ? -Margin.X * (VirtualRatio.Value - 1) : -Margin.X * (VirtualRatio.Value),
        Margin.Y == 0 ? 0
            : Margin.Y < 0 ? -Margin.Y * (2 + VirtualRatio.Value - 1) : -Margin.Y * (1 + VirtualRatio.Value - 1)
    );

    public static float Allowance = 0.9f;

    public static int? VirtualRatio = 1;
    public static Rectangle Rectangle = new Rectangle();

    public static Shader Shader = null;

    public static void Start()
    {
        Scale = new Vector2(CONSTANTS.VIRTUAL_WIDTH, CONSTANTS.VIRTUAL_HEIGHT);
        // VisibleArea = new Rectangle(0, 0, CONSTANTS.VIRTUAL_HEIGHT, CONSTANTS.VIRTUAL_HEIGHT);
        RefreshProjection();

        RelativeLayer = new RelativeViewportLayer(0);
        Layers.Add(RelativeLayer);
        LightLayer = new LightViewportLayer(1);
        Layers.Add(LightLayer);
        AbsoluteLayer = new AbsoluteViewportLayer(2);
        Layers.Add(AbsoluteLayer);

        Layers.ForEach(l => l.RefreshProjection());
    }

    public static void Update()
    {
        if (Raylib.IsWindowResized()) RefreshProjection();

        if (Target != null)
        {
            CameraPosition = Target.Position
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
        Layers.Where(l => l.Visible).ToList().ForEach(l => l.DrawToWindow());
        if (Shader != null) Raylib.EndShaderMode();
    }

    public static void Draw()
    {
        Raylib.ClearBackground(ClearColor.ToRaylib());
        DrawToLayers();
        DrawToWindow();
    }

    // ---

    public static void RefreshProjection()
    {
        Scale.X = Raylib.GetScreenWidth();
        Scale.Y = Raylib.GetScreenHeight();

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

            // VisibleArea = new Rectangle(
            //     (-Margin.X / 2f) / VirtualRatio.Value,
            //     (-Margin.Y / 2f) / VirtualRatio.Value,
            //     (Rectangle.Width - (-Margin.X / 2f)) / VirtualRatio.Value,
            //     (Rectangle.Height - (-Margin.Y / 2f)) / VirtualRatio.Value
            // );

            // Margin /= VirtualRatio.Value;
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
        Log.Write("ScalePixels: " + ScalePixels);
        RefreshProjection();
    }
}
