using System.Numerics;
using Raylib_cs;

namespace Thingus;

public class Control : Thing
{
    public Vector2 Bounds;
    public Color Color = PaletteBasic.White;
    public Color HighlightColor = PaletteBasic.Green;
    public int TileNumber = 0;
    public int TileSize = 0;

    public bool IsHovered;
    public bool IsHeld;
    public bool IsFocused;
    public Action Hovered;
    public Action Pressed;
    public Action Released;

    public int Padding = 0;
    public Texture2D? Texture;

    public override void Update()
    {
        base.Update();

        if (Pressed != null && Hovered != null && Released != null)
        {
            IsHovered = false;
            return;
        }

        Vector2 mouse = Vector2.Zero;
        if (DrawMode == DrawMode.Relative) mouse = Input.MousePositionRelative();
        if (DrawMode == DrawMode.Absolute) mouse = Input.MousePositionAbsolute();
        if (DrawMode == DrawMode.Texture && SubViewport != null)
        {
            if (!SubViewport.IsHovered)
            {
                IsHovered = false;
                return;
            }
            if (SubViewport.DrawMode == DrawMode.Relative) mouse = Input.MousePositionRelative() + SubViewport.Scroll - SubViewport.GlobalPosition;
            if (SubViewport.DrawMode == DrawMode.Absolute) mouse = Input.MousePositionAbsolute() + SubViewport.Scroll - SubViewport.GlobalPosition;
        }

            
        IsHovered = Utility.CheckRectangleOverlap(
            mouse,
            mouse,
            GlobalPosition,
            GlobalPosition + Bounds
        );

        if (IsHovered)
        {
            if (Hovered != null) Hovered();

            if (Input.LeftMouseButtonIsPressed)
            {
                IsHeld = true;
                Game.GetThings<Control>().ForEach(c => c.IsFocused = false);
                IsFocused = true;
                if (Pressed != null) Pressed();
            }

        }
        if (IsHeld)
        {
            if (Input.LeftMouseButtonIsReleased)
            {
                IsHeld = false;
                if (Released != null) Released();
            }
        }
    }
}