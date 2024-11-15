using System.Numerics;
using Raylib_cs;

namespace Thingus;

public class BakedTile
{
    public Texture2D Texture;
    public Vector2 Position;
    public int TileNumber;

    public BakedTile() { }
    public BakedTile(Texture2D texture, Vector2 position, int tileNumber = 0)
    {
        Texture = texture;
        Position = position;
        TileNumber = tileNumber;
    }
}

public class BakedTilemap : Thing
{
    public List<BakedTile> Tiles = new List<BakedTile>() { };
    public int[][] CollisionGrid = new int[32][];
    public static readonly int COLLISION_GRID_SIZE = 32;
    public int Layer;
    public BlendMode? BlendMode = null;

    public static BakedTilemap Create(Thing root, ThingModel model)
    {
        BakedTilemap bakedTilemap = root.GetThings<BakedTilemap>().Find(b => b.Layer == model.Layer && b.BlendMode == model.BlendMode);
        if (bakedTilemap == null)
        {
            bakedTilemap = new BakedTilemap();
            bakedTilemap.Layer = model.Layer;
            bakedTilemap.DrawOrder = (model.Layer * 10) - 20;
            bakedTilemap.BlendMode = model.BlendMode;

            for (int r = 0; r < bakedTilemap.CollisionGrid.Count(); r++)
            {
                bakedTilemap.CollisionGrid[r] = new int[32];
            }
        }

        bakedTilemap.AddTile(model);

        bakedTilemap.Name = $"BakedTilemap_{bakedTilemap.Layer}";
        return bakedTilemap;
    }

    public void RemoveTile(Vector2 position)
    {
        Tiles = Tiles.Where(t => t.Position != position).ToList();

        int r = (int)(position.Y / CONSTANTS.TILE_SIZE);
        int c = (int)(position.X / CONSTANTS.TILE_SIZE);

        if (r < 0 || c < 0 || r >= BakedTilemap.COLLISION_GRID_SIZE || c >= BakedTilemap.COLLISION_GRID_SIZE) return;

        CollisionGrid[r][c] = 0;
    }

    public void AddTile(ThingModel model)
    {
        Tiles.Add(new BakedTile(Library.Textures[model.Name], model.Position, model.TileNumber ?? 0));
        Name = "BakedTilemap";

        int r = (int)(model.Position.Y / CONSTANTS.TILE_SIZE);
        int c = (int)(model.Position.X / CONSTANTS.TILE_SIZE);

        if (r < 0 || c < 0 || r >= COLLISION_GRID_SIZE || c >= COLLISION_GRID_SIZE) return;

        CollisionGrid[r][c] = model.Tags.Contains("Collision") ? 1 : 0;
    }

    public override void AfterCreatedFromMap()
    {
        base.AfterCreatedFromMap();
        Cell = null;
    }

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Draw()
    {
        base.Draw();

        if (BlendMode != null) Raylib.BeginBlendMode(BlendMode.Value);
        Tiles.ForEach(t =>
        {
            Shapes.DrawSprite(
                texture: t.Texture,
                tileNumber: t.TileNumber,
                tileSize: CONSTANTS.TILE_SIZE,
                position: GlobalPosition + t.Position
            );
        });
        if (BlendMode != null) Raylib.EndBlendMode();

        // for (int r = 0; r < CollisionGrid.Count(); r++)
        // {
        //     int[] row = CollisionGrid[r];
        //     for (int c = 0; c < row.Count(); c++)
        //     {
        //         int column = CollisionGrid[r][c];
        //         Shapes.DrawText(column.ToString(), new Vector2(c * 16, r * 16) - new Vector2(0, 8), outlineColor: Colors.Black, color: Colors.White);
        //     }
        // }
    }
}
