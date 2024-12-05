using System.Numerics;
using Raylib_cs;

namespace Thingus;

public class Control : Thing
{
    public static Control FocusedControl = null;
    public static Control HighlightedControl = null;
    public static Control TextInputControl = null;

    public Vector2 Bounds;
    public Color Color = PaletteBasic.White;
    public Color HighlightColor = Theme.Primary;
    public Color TextColor = PaletteBasic.White;
    public Color TextHighlightColor = Theme.Primary;
    public int TileNumber = 0;
    public int TileSize = 0;

    public bool CheckMouse = true;

    public bool IsHovered;
    public bool IsFocused => FocusedControl == this || (Parent as Control)?.IsFocused == true;
    public bool IsHighlighted => HighlightedControl == this || (Parent as Control)?.IsHighlighted == true;
    public bool IsHeld;
    public Action Hovered;
    public Action Pressed;
    public Action Highlighting;
    public Action Released;
    public Action LostFocus;
    public Action LostHighlight;

    public Control Up;
    public Control Down;
    public Control Left;
    public Control Right;

    public Vector2 Padding;
    public Vector2 TextPadding;
    public Texture2D? Texture;
    public int Spacing;

    public virtual void Focus()
    {
        if (FocusedControl != null) FocusedControl.LoseFocus();
        FocusedControl = this;
    }

    public virtual void LoseFocus()
    {
        if (FocusedControl == this)
        {
            if (LostFocus != null) LostFocus();
            FocusedControl = null;
        }
    }

    public void Highlight()
    {
        if (HighlightedControl != null) HighlightedControl.LoseHighlight();
        HighlightedControl = this;
    }

    public void LoseHighlight()
    {
        if (HighlightedControl == this)
        {
            if (LostHighlight != null) LostHighlight();
            HighlightedControl = null;
        }
    }

    public override void Update()
    {
        base.Update();

        if (IsHighlighted) Highlighting();
        if (CheckMouse) Mouse();
    }

    void Mouse()
    {
        if (IsHeld)
        {
            if (!Input.LeftMouseButtonIsHeld)
            {
                IsHeld = false;
                if (Released != null) Released();
            }
        }

        if ((FocusedControl != null && !IsFocused) || (Pressed != null && Hovered != null && Released != null))
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
                if (Pressed != null) Pressed();
            }
        }
    }
}