using System.Numerics;
using Raylib_cs;

namespace Thingus;

public class Room : Thing
{
    public string Type = "Room";

    public Vector2 TopLeft;
    public Vector2 BottomRight;
    public Vector2 Bounds;
    public Vector2 Center;

    public bool Hovered = false;

    public Room() { }
    public Room(string name, Vector2 position) : base(name, position)
    {

    }

    public override void Init()
    {
        base.Init();
        DrawOrder = -100;
    }

    public override void Update()
    {
        base.Update();

        TopLeft = Position;
        Bounds = new Vector2(
            Map.Cells.OrderByDescending(c => c.Position.X).FirstOrDefault()?.Position.X ?? 0,
            Map.Cells.OrderByDescending(c => c.Position.Y).FirstOrDefault()?.Position.Y ?? 0
        );
        BottomRight = Position + Bounds;
        Center = (TopLeft + BottomRight) / 2f;

        if (Type == "Zone") return;

        if (Game.Root.Room != this)
        {
            Hovered = Utility.CheckRectangleOverlap(
                Input.MousePositionRelative() - new Vector2(8, 8),
                Input.MousePositionRelative() + new Vector2(8, 8),
                TopLeft,
                BottomRight
            );

            if (Hovered)
            {
                if (Input.LeftMouseButtonIsPressed)
                {
                    Click();
                }
            }
        }
        else
        {
            Hovered = false;
        }
    }

    public bool CheckOverlap(Collider other)
    {
        return Utility.CheckRectangleOverlap(TopLeft, BottomRight, other.GlobalTopLeft, other.GlobalBottomRight);
    }

    public void Click()
    {
        Editor editor = Game.GetThing<Editor>();
        if (editor != null) editor.Holdup = true;

        Game.Root.Room = this;
    }

    public override void Draw()
    {
        base.Draw();

        if (Type == "Zone")
        {
            // Shapes.DrawSprite(
            //     texture: Library.Textures["Pixel"],
            //     position: Center, 
            //     scale: Bounds + new Vector2(16),
            //     color: new Color(100, 100, 100, 150)
            // );

            return;
        };

        Shapes.DrawSprite(
            texture: Library.Textures["Pixel"],
            position: Center,
            scale: Bounds + new Vector2(16),
            color: Hovered ? new Color(255, 255, 255, 255) : new Color(255, 255, 255, 150)
        );

        // Shapes.DrawSprite(
        //     texture: Library.Textures["Pixel"],
        //     position: topLeft, 
        //     scale: new Vector2(16)
        // );

        // Shapes.DrawSprite(
        //     texture: Library.Textures["Pixel"],
        //     position: bottomRight, 
        //     scale: new Vector2(16)
        // );

        Shapes.DrawText(
            text: Name,
            position: new Vector2(Center.X, TopLeft.Y) - new Vector2(Raylib.MeasureText(Name, Library.Font.BaseSize) / 2f, Library.Font.BaseSize * 3),
            color: Colors.White,
            outlineColor: Colors.Black
        );

        Shapes.DrawText(
            text: $"{Position.X}x,{Position.Y}y ({(BottomRight.X - TopLeft.X) / CONSTANTS.TILE_SIZE}w,{(BottomRight.Y - TopLeft.Y) / CONSTANTS.TILE_SIZE}h)",
            position: new Vector2(Center.X, TopLeft.Y) - new Vector2(Raylib.MeasureText(Name, Library.Font.BaseSize) / 2f, Library.Font.BaseSize * 2),
            color: Colors.White,
            outlineColor: Colors.Black
        );
    }
}
