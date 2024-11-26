using System.Numerics;

namespace Thingus;

public class ScrollableControl : SubViewport
{
    ButtonControl scrollHead;
    bool scrolling = false;
    Vector2 scrollHeadStart;
    Vector2 mouseStart;

    int width = 6;

    public ScrollableControl() { }
    public ScrollableControl(Vector2 bounds, Vector2 area)
        : base(bounds, area)
    {

    }

    public override void Start()
    {
        base.Start();

        scrollHead = new ButtonControl();
        scrollHead.Pressed = () => Log.Write("Click");
        scrollHead.Texture = Library.Textures["Pixel"];
        scrollHead.Bounds = new Vector2(width, 12);
        scrollHead.DrawOrder = 10;
        scrollHead.Color = new Color(0, 0, 0, 1);
        scrollHead.DrawMode = DrawMode;
        scrollHead.Position.X = Bounds.X - scrollHead.Bounds.X;
        scrollHead.Pressed = () =>
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
        else if (Hovered)
        {
            scrollHead.Position.Y -= Input.MouseWheel * 5;
        }

        if (scrollHead.Position.Y < 0) scrollHead.Position.Y = 0;
        if (scrollHead.Position.Y > Bounds.Y - scrollHead.Bounds.Y) scrollHead.Position.Y = Bounds.Y - scrollHead.Bounds.Y;

        Scroll.Y = Max.Y * (scrollHead.Position.Y) / (Bounds.Y - scrollHead.Bounds.Y);
    }

    public override void Draw()
    {
        DrawSprite(
            texture: Library.Textures["Pixel"],
            position: GlobalPosition - new Vector2(1),
            scale: Bounds + new Vector2(2),
            origin: new Vector2(0),
            color: PaletteBasic.Black
        );

        base.Draw();

        DrawSprite(
            texture: Library.Textures["Pixel"],
            position: Position + new Vector2(Bounds.X - width - 1, -1),
            scale: new Vector2(width + 2, Bounds.Y + 2),
            color: PaletteBasic.Black,
            origin: new Vector2(0)
        );
    }
}