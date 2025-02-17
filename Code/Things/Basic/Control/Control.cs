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
    public bool IgnoreParentHighlight = false;
    public int TileNumber = 0;
    public int TileSize = 0;

    public bool CheckMouse = true;

    public bool IsHovered;
    public bool IsFocused => FocusedControl == this || (Parent as Control)?.IsFocused == true;
    public bool IsHighlighted => HighlightedControl == this || (!IgnoreParentHighlight && (Parent as Control)?.IsHighlighted == true);
    public bool IsHeld;
    public Action Hovered;
    public event Action OnPressed;
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

        if (IsHighlighted && Highlighting != null) Highlighting();
        if (CheckMouse) Mouse();
    }

    public virtual void Refresh()
    {

    }

    public Control HighlightOverride = null;

    public virtual bool ShouldShowHighlight => (HighlightOverride != null && HighlightOverride.ShouldShowHighlight) || (OnPressed != null && ((IsHovered && !Input.IsGamepadFocused) || IsHeld || (IsHighlighted && Input.IsGamepadFocused)));
    
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

        if ((FocusedControl != null && !IsFocused) || (OnPressed != null && Hovered != null && Released != null))
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
            GlobalPosition - (Padding / 2),
            GlobalPosition + Bounds + (Padding / 2)
        );

        if (IsHovered)
        {
            if (Hovered != null) Hovered();

            if (Input.LeftMouseButtonIsPressed)
            {
                IsHeld = true;
                if (OnPressed != null) OnPressed.Invoke();
            }
        }
    }
}