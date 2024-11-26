using System.Numerics;

namespace Thingus;

public class ButtonControl : TextureControl
{
    public Action Pressed;
    public Action Released;

    public override void Start()
    {
        base.Start();

        Drawer drawer = new Drawer(name: "ButtonBackground", drawOrder: DrawOrder - 1, drawMode: DrawMode, action: d =>
        {
            d.DrawSprite(
                texture: Library.Textures["Pixel"],
                scale: Bounds,
                origin: new Vector2(0),
                color: PaletteBasic.Black,
                position: d.GlobalPosition
            );
        });
        drawer.SubViewport = SubViewport;

        AddChild(drawer);
    }

    public override void Update()
    {
        base.Update();

        if (Hovered)
        {
            if (Input.LeftMouseButtonIsPressed)
            {
                Held = true;
                if (Pressed != null) Pressed();
            }

        }
        if (Held)
        {
            if (Input.LeftMouseButtonIsReleased)
            {
                Held = false;
                if (Released != null) Released();
            }
        }
    }

    public override void Draw()
    {
        DrawSprite(
            texture: Library.Textures["Pixel"],
            scale: Bounds - new Vector2(2),
            origin: new Vector2(0),
            color: Held || Hovered ? PaletteBasic.Green : PaletteBasic.Gray,
            position: GlobalPosition + new Vector2(1)
        );

        base.Draw();
    }
}