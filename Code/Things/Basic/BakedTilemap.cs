using System.Net.WebSockets;
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
    public BlendMode? BlendMode = null;
    public Color Color = PaletteBasic.White;

    bool queueReorder = false;

    public static BakedTilemap Create(ThingModel model)
    { 
        BakedTilemap bakedTilemap = model.Root.GetThings<BakedTilemap>().Find(b => b.Layer == model.Layer && b.BlendMode == model.BlendMode);
        if (bakedTilemap == null)
        {
            bakedTilemap = new BakedTilemap(model);
            model.Root.AddChild(bakedTilemap);
        }

        bakedTilemap.AddTile(model);

        bakedTilemap.Name = $"BakedTilemap_{bakedTilemap.Layer}";
        return bakedTilemap;
    }

    public BakedTilemap(ThingModel model)
    {
        UpdateInEditMode = true;
        Layer = model.Layer;
        DrawOrder = Game.Layers[Layer];
        BlendMode = model.BlendMode;
    }

    public void RemoveTile(Vector2 position)
    {
        Tiles = Tiles.Where(t => t.Position != position).ToList();

        // Update TokenGrid
        Vector2 p = Utility.WorldToGridPosition(position + GlobalPosition);
        int r = (int)p.Y;
        int c = (int)p.X;
        // if (r < 0 || c < 0) return;
        // if (r < 0 || c < 0 || r >= TokenGrid.Bounds.Y || c >= TokenGrid.Bounds.X) return;
        TokenGrid.Set(c, r, 0);

        queueReorder = true;
    }

    public void AddTile(ThingModel model)
    {
        Tiles.Add(new BakedTile(Library.Textures[model.Name], model.Position, model.TileNumber ?? 0));
        Name = "BakedTilemap";

        // Update TokenGrid
        Vector2 position = Utility.WorldToGridPosition(model.Position + GlobalPosition);
        int r = (int)position.Y;
        int c = (int)position.X;
        // if (r < 0 || c < 0 || r >= TokenGrid.Bounds.Y || c >=  TokenGrid.Bounds.X) return;
        TokenGrid.Set(c, r, model.Token);

        queueReorder = true;
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

        if (queueReorder)
        {
            queueReorder = false;
            Tiles = Tiles.ToList().OrderBy(t => t.Position.Y).ThenByDescending(t => t.Position.X).ToList();
        }
    }

    public override void Draw()
    {
        base.Draw();

        Color color = Color;
        if (GlobalAlphaOverride != null) color = color.WithAlpha(GlobalAlphaOverride.Value);

        if (BlendMode != null) Raylib.BeginBlendMode(BlendMode.Value);
        Tiles.ForEach(t =>
        {
            DrawSprite(
                texture: t.Texture,
                tileNumber: t.TileNumber,
                tileSize: CONSTANTS.TILE_SIZE,
                position: GlobalPosition + t.Position,
                color: color
            );
        });
        if (BlendMode != null) Raylib.EndBlendMode();

        // for (int r = 0; r < CollisionGrid.Count(); r++)
        // {
        //     int[] row = CollisionGrid[r];
        //     for (int c = 0; c < row.Count(); c++)
        //     {
        //         int column = CollisionGrid[r][c];
        //         Shapes.DrawText(column.ToString(), new Vector2(c * 16, r * 16) - new Vector2(0, 8), outlineColor: PaletteBasic.Black, color: PaletteBasic.White);
        //     }
        // }
    }
}
