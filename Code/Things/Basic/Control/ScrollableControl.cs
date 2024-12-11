using System.Numerics;
using Raylib_cs;

namespace Thingus;

public class ScrollableControl : SubViewport
{    
    ButtonControl scrollHead;
    bool scrolling = false;
    Vector2 scrollHeadStart;
    Vector2 mouseStart;

    public static readonly int ScrollBarWidth = 6;

    public ScrollableControl() { }
    public ScrollableControl(Vector2 bounds, Vector2 area)
        : base(bounds, area)
    {

    }

    public override void Start()
    {
        base.Start();

        scrollHead = new ButtonControl();
        scrollHead.Texture = Library.Textures["ScrollHead"];
        scrollHead.Bounds = new Vector2(ScrollBarWidth, 12);
        scrollHead.DrawOrder = 10;
        scrollHead.DrawMode = DrawMode;
        scrollHead.Position.X = Bounds.X - scrollHead.Bounds.X;
        scrollHead.OnPressed += () =>
        {
            scrolling = true;
            scrollHeadStart = scrollHead.Position;
            mouseStart = Input.MousePosition(DrawMode);
        };
        scrollHead.Released = () =>
        {
            scrolling = false;
        };
        AddChild(scrollHead);
    }

    public override void Update()
    {
        base.Update();

        if (scrolling)
        {
            Vector2 difference = Input.MousePosition(DrawMode) - mouseStart;
            scrollHead.Position.Y = scrollHeadStart.Y + difference.Y;
        }
        else if (IsHovered)
        {
            scrollHead.Position.Y -= Input.MouseWheel * 5;
        }

        if (scrollHead.Position.Y < 0) scrollHead.Position.Y = 0;
        if (scrollHead.Position.Y > Bounds.Y - scrollHead.Bounds.Y) scrollHead.Position.Y = Bounds.Y - scrollHead.Bounds.Y;

        Scroll.Y = Max.Y * (scrollHead.Position.Y) / (Bounds.Y - scrollHead.Bounds.Y);
    }

    public override void Draw()
    {
        // Outline
        // DrawSprite(
        //     texture: Library.Textures["Pixel"],
        //     position: GlobalPosition - new Vector2(1),
        //     scale: Bounds + new Vector2(2),
        //     origin: new Vector2(0),
        //     color: PaletteBasic.Black
        // );

        DrawNineSlice(
            texture: Texture,
            tileSize: 5,
            position: GlobalPosition - Padding,
            width: (int)(Bounds.X + (Padding.X * 2)),
            height: (int)(Bounds.Y + (Padding.Y * 2)),
            color: Color
        );

        base.Draw();

        // Scroll Background
        DrawSprite(
            texture: Library.Textures["Pixel"],
            position: GlobalPosition + new Vector2(Bounds.X - ScrollBarWidth, 0),
            scale: new Vector2(ScrollBarWidth, Bounds.Y),
            color: PaletteBasic.Black,
            origin: new Vector2(0)
        );
    }
}