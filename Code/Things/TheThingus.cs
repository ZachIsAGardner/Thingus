using System.Numerics;

namespace Thingus;

public class TheThingus : Sprite
{
     // Create from Map
    public static TheThingus Create(Thing root, ThingModel model)
        => new TheThingus(
            name: model.Name,
            position: model.Position,
            drawMode: DrawMode.Relative,
            tileSize: CONSTANTS.TILE_SIZE,
            tileNumber: model.TileNumber,
            color: PaletteBasic.White
        );
    public TheThingus() { }
    // Create from code
    public TheThingus(
        string name, Vector2? position = null, DrawMode drawMode = DrawMode.Relative, float drawOrder = 0, float updateOrder = 0,
        int? tileSize = null, int? tileNumber = null, Color? color = null, Vector2? scale = null, float rotation = 0f
    ) : base(name, position, drawMode, drawOrder, updateOrder)
    {
        Texture = Library.Textures[name];
    }

    public override void Update()
    {
        base.Update();
    }
}