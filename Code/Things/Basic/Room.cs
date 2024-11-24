using System.Numerics;
using Raylib_cs;

namespace Thingus;

public class Room : Thing
{
    public Vector2 Bounds;
    public Vector2 TopLeft => Position;
    public Vector2 BottomRight => Position + Bounds;
    public Vector2 Center => Position + (Bounds / 2);
    public Vector2 Left => new Vector2(TopLeft.X, Center.Y);
    public Vector2 Right => new Vector2(BottomRight.X, Center.Y);
    public Vector2 Top => new Vector2(Center.X, TopLeft.Y);
    public Vector2 Bottom => new Vector2(Center.X, BottomRight.Y);

    public bool Hovered = false;

    public static Room Create(Thing root, ThingModel model) => new Room(model.Name, model.Position, model.Bounds);
    public Room() { }
    public Room(string name, Vector2 position, Vector2 bounds) : base(name, position)
    {
        Bounds = bounds;
    }

    public override void Init()
    {
        base.Init();
        DrawOrder = 100;
    }

    public override void Start()
    {
        base.Start();

        AddChild(new Drawer("RoomBackground", drawOrder: -100, action: () =>
        {
            if (Game.Mode == GameMode.Play) return;
            Vector2 bounds = Bounds;
            Shapes.DrawSprite(
                texture: Library.Textures["Pixel"],
                position: TopLeft,
                origin: new Vector2(0),
                scale: bounds 
                    + (Game.ProjectionType == ProjectionType.Grid
                        ? new Vector2(CONSTANTS.TILE_SIZE)
                        : Game.ProjectionType == ProjectionType.Oblique
                            ? new Vector2(0)
                            : new Vector2(CONSTANTS.TILE_SIZE_HALF, CONSTANTS.TILE_SIZE_QUARTER)
                    ),
                color: Hovered && Game.Root.DeveloperTools.Editor?.Room != this ? new Color(255, 255, 255, 255) : new Color(255, 255, 255, 150)
            );
        }));
    }

    public override void Update()
    {
        base.Update();

        Hovered = Utility.CheckRectangleOverlap(
            Input.MousePositionRelative() - new Vector2(8, 8),
            Input.MousePositionRelative() + new Vector2(8, 8),
            TopLeft,
            BottomRight
        );

        if (Game.Root.DeveloperTools.Editor?.Room != this)
        {
            if (Hovered)
            {
                if (Input.LeftMouseButtonIsPressed)
                {
                    Click();
                }
            }
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

        if (Game.Root.DeveloperTools.Editor != null) Game.Root.DeveloperTools.Editor.Room = this;
    }

    void DrawGrid()
    {
        if (Game.ProjectionType == ProjectionType.Isometric)
        {
            for (int c = 0; c <= (Bounds.Y / CONSTANTS.TILE_SIZE_QUARTER); c++)
            {
                for (int r = 0; r <= (Bounds.X / CONSTANTS.TILE_SIZE_HALF); r++)
                {
                    Shapes.DrawSprite(
                        texture: Library.Textures[$"{CONSTANTS.TILE_SIZE_HALF}x{CONSTANTS.TILE_SIZE_QUARTER}Grid"],
                        origin: Vector2.Zero,
                        color: new Color(255, 255, 255, 150),
                        position: TopLeft + new Vector2(r * CONSTANTS.TILE_SIZE_HALF, c * CONSTANTS.TILE_SIZE_QUARTER)
                    );
                }
            }
        }
        else if (Game.ProjectionType == ProjectionType.Oblique)
        {
            for (int c = 0; c < (Bounds.Y / (CONSTANTS.TILE_SIZE_OBLIQUE)); c++)
            {
                for (int r = 0; r < (Bounds.X / (CONSTANTS.TILE_SIZE_OBLIQUE)); r++)
                {
                    Shapes.DrawSprite(
                        texture: Library.Textures[$"{(CONSTANTS.TILE_SIZE)}ObliqueGrid"],
                        origin: Vector2.Zero,
                        color: new Color(255, 255, 255, 150),
                        position: TopLeft + new Vector2(
                            r * (CONSTANTS.TILE_SIZE_OBLIQUE), 
                            c * (CONSTANTS.TILE_SIZE_OBLIQUE)
                        )
                    );
                }
            }
        }
        else
        {
            for (int c = 0; c <= (Bounds.Y / CONSTANTS.TILE_SIZE); c++)
            {
                for (int r = 0; r <= (Bounds.X / CONSTANTS.TILE_SIZE); r++)
                {
                    Shapes.DrawSprite(
                        texture: Library.Textures[$"{CONSTANTS.TILE_SIZE}Grid"],
                        origin: Vector2.Zero,
                        color: new Color(255, 255, 255, 150),
                        position: TopLeft + new Vector2(r * CONSTANTS.TILE_SIZE, c * CONSTANTS.TILE_SIZE)
                    );
                }
            }
        }
    }

    public override void Draw()
    {
        base.Draw();

        if (Game.Mode == GameMode.Play) return;

        DrawGrid();

        Shapes.DrawText(
            text: Name,
            position: new Vector2(Center.X, TopLeft.Y) - new Vector2(Raylib.MeasureText(Name, Library.Font.BaseSize) / 2f, Library.Font.BaseSize * 3),
            color: Colors.White,
            outlineColor: Colors.Black
        );

        Shapes.DrawText(
            text: $"{Position.X}x,{Position.Y}y ({Bounds.X}w,{Bounds.Y}h)",
            position: new Vector2(Center.X, TopLeft.Y) - new Vector2(Raylib.MeasureText(Name, Library.Font.BaseSize) / 2f, Library.Font.BaseSize * 2),
            color: Colors.White,
            outlineColor: Colors.Black
        );
    }
}
