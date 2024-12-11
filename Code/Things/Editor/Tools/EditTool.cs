using System.ComponentModel.Design;
using System.Numerics;
using Thingus;

public class EditTool : Tool
{
    public override string Name => "Edit";
    public override KeyboardKey Shortcut => KeyboardKey.V;
    public override bool ShowPreview => false;
    public override int TileNumber => 0;

    Action state;
    RectangleSelection selection;
    List<(Thing Thing, Vector2 StartPosition)> moveThings = new List<(Thing Thing, Vector2 StartPosition)>() { };
    List<(BakedTile BakedTile, Vector2 StartPosition, BakedTilemap BakedTilemap, MapCell MapCell)> moveBakedTiles = new List<(BakedTile BakedTile, Vector2 StartPosition, BakedTilemap BakedTilemap, MapCell MapCell)>() { };

    Vector2? actionMousePositionStart = null;

    Thing lastThing;
    Thing window;

    public EditTool(Editor editor) : base(editor)
    {
        state = Normal;
    }

    public override void Exit()
    {
        CloseWindow();
    }

    public override void Update()
    {
        if (Input.AltIsHeld)
        {
            Picker();
            return;
        }

        if (Input.MiddleMouseButtonIsHeld)
        {
            Move();
            return;
        }

        editor.Mouse.TileNumber = 0;

        dragMousePosition = null;
        dragCameraPosition = null;

        if (editor.Room == null) return;
        if (editor.InteractDisabled) return;

        if (state != null) state();

        base.Update();
    }

    void Normal()
    {
        Thing thing = editor.Room.Children.Find(c => c.GlobalPosition == editor.GridPosition);
        if (thing != lastThing && lastThing != null)
        {
            Sprite sprite = lastThing.GetThing<Sprite>();
            if (sprite != null) sprite.Color = PaletteBasic.White;
        }

        // Hovering
        if (thing != null)
        {
            editor.Mouse.TileNumber = 1;
            Sprite sprite = thing.GetThing<Sprite>();
            if (sprite != null) sprite.Color = Theme.Primary;
            if (Input.LeftMouseButtonIsPressed)
            {
                CloseWindow();
                window = Window(thing);
            }
        }
        // Empty
        else
        {
            if (Input.LeftMouseButtonIsPressed)
            {
                if (window == null)
                {
                    state = Drag;
                }
                else if (Control.FocusedControl == null)
                {
                    CloseWindow();
                }
            }
        }
        lastThing = thing;
    }

    void Drag()
    {
        editor.Mouse.TileNumber = 1;

        if (Input.LeftMouseButtonIsHeld && selection == null)
        {
            selection = new RectangleSelection(editor.GridPosition, () => editor.GridPosition, new Color(0, 255, 0, 50));
            editor.AddChild(selection);
            Input.Holdup = true;
        }

        if (Input.LeftMouseButtonIsReleased && selection != null)
        {
            List<Thing> things = editor.Room.Children.Where(t => selection.Positions.Any(p => t.GlobalPosition == p && t.Layer == LayerControl.CurrentLayer)).ToList();
            List<(BakedTile BakedTile, BakedTilemap BakedTilemap, MapCell MapCell)> bakedTiles = new List<(BakedTile BakedTile, BakedTilemap BakedTilemap, MapCell MapCell)>() { };
            foreach (var bakedTilemap in editor.Room.GetThings<BakedTilemap>())
            {
                bakedTiles.AddRange(
                    bakedTilemap.Tiles.Where(t => selection.Positions.Any(p => bakedTilemap.GlobalPosition + t.Position == p && bakedTilemap.Layer == LayerControl.CurrentLayer))
                        .Select(t => (t, bakedTilemap, editor.Room.Map.GetCell(t.Position)))
                        .ToList()
                );
            }
        
            actionMousePositionStart = editor.GridPosition;
            if (things.Count <= 0 && bakedTiles.Count <= 0)
            {
                Reset();
            } 
            else
            {
                selection.ChangeState(selection.Move);
                moveThings = things.Select(r => (r, r.Position)).ToList();
                moveBakedTiles = bakedTiles.Select(t => (t.BakedTile, t.BakedTile.Position, t.BakedTilemap, t.MapCell)).ToList();
                // things.ForEach(r => r.GroupMove = true);
                state = MoveThings;
            }
        }
    }

    void MoveThings()
    {
        editor.Mouse.TileNumber = 1;

        Vector2 difference = editor.GridPosition - actionMousePositionStart.Value;

        moveThings.ForEach(m =>
        {
            m.Thing.Position = m.Thing.Cell.Position = m.StartPosition + difference;
        });

        moveBakedTiles.ForEach(m =>
        {
            m.BakedTile.Position = m.MapCell.Position = m.StartPosition + difference;
        });

        if (Input.LeftMouseButtonIsPressed)
        {
            List<(Thing Thing, Vector2 StartPosition, Vector2 Destination)> storedMoveThings = new List<(Thing Thing, Vector2 StartPosition, Vector2 Destination)>() { };
            moveThings.ForEach(m => storedMoveThings.Add((m.Thing, m.StartPosition, m.Thing.Position)));

            List<(BakedTile BakedTile, Vector2 StartPosition, BakedTilemap BakedTilemap, MapCell MapCell, Vector2 Destination)> storedMoveBakedTiles = new List<(BakedTile BakedTile, Vector2 StartPosition, BakedTilemap BakedTilemap, MapCell MapCell, Vector2 Destination)>() { };
            moveBakedTiles.ForEach(m => storedMoveBakedTiles.Add((m.BakedTile, m.StartPosition, m.BakedTilemap, m.MapCell, m.BakedTile.Position)));

            editor.AddHistoryAction(new HistoryAction(
                "MoveThings",
                () =>
                {
                    storedMoveThings.ForEach(s =>
                    {
                        if (s.Thing.GlobalRemoved) s.Thing = Game.GetThings<Thing>().Find(t => t.Name == s.Thing.Name);
                        s.Thing.Position = s.Thing.Cell.Position = s.StartPosition;
                    });

                    storedMoveBakedTiles.ForEach(s =>
                    {
                        if (s.BakedTilemap.GlobalRemoved) s.BakedTilemap = Game.GetThings<BakedTilemap>().Find(t => t.Name == s.BakedTilemap.Name);
                        s.BakedTile.Position = s.MapCell.Position = s.StartPosition;
                    });
                },
                () =>
                {
                    storedMoveThings.ForEach(s =>
                    {
                        if (s.Thing.GlobalRemoved) s.Thing = Game.GetThings<Thing>().Find(t => t.Name == s.Thing.Name);
                        s.Thing.Position = s.Thing.Cell.Position = s.Destination;
                    });

                    storedMoveBakedTiles.ForEach(s =>
                    {
                        if (s.BakedTilemap.GlobalRemoved) s.BakedTilemap = Game.GetThings<BakedTilemap>().Find(t => t.Name == s.BakedTilemap.Name);
                        s.BakedTile.Position = s.MapCell.Position = s.Destination;
                    });
                }
            ));

            Reset();
        }
    }

    void Reset()
    {
        actionMousePositionStart = null;
        state = Normal;
        moveThings.Clear();
        moveBakedTiles.Clear();
        selection?.Destroy();
        selection = null;
    }

    void CloseWindow()
    {
        if (window != null)
        {
            window.Destroy();
            window = null;
        }
    }

    Thing Window(Thing thing)
    {
        int width = 128;
        int height = 16 + thing.Cell.Import.Options.Count * 16;

        Control root = new Control();
        root.Position = thing.GlobalPosition + new Vector2(CONSTANTS.TILE_SIZE + CONSTANTS.TILE_SIZE_QUARTER, -height);

        VerticalFlexControl flex = new VerticalFlexControl();
        flex.Bounds = new Vector2(width, height);
        flex.Texture = Library.Textures["BoxThinSlice"];
        flex.TileSize = 5;
        flex.Padding = new Vector2(3);
        flex.DrawOrder = 100;
        root.AddChild(flex);

        TextureControl tailControl = new TextureControl();
        tailControl.Texture = Library.Textures["BoxTail"];
        tailControl.Position = flex.Position + new Vector2(-8, height - 5);
        tailControl.Bounds = new Vector2(10);
        tailControl.DrawOrder = 110;
        root.AddChild(tailControl);

        HorizontalFlexControl title = new HorizontalFlexControl();
        title.Texture = Library.Textures["BoxThinLightSlice"];
        title.Bounds = new Vector2(width, 8);
        title.Padding = new Vector2(3);
        title.Justify = FlexJustify.Between;
        title.Offset = new Vector2(2, -3);
        flex.AddChild(title);

        TextControl textControl = new TextControl();
        textControl.Text = thing.Name;
        textControl.Color = PaletteBasic.LightGray;
        textControl.Padding = new Vector2(0);
        title.AddChild(textControl);

        ButtonControl close = new ButtonControl();
        close.OnPressed += () => 
        {
            CloseWindow();
        };
        close.Texture = Library.Textures["CrossButton"];
        close.Bounds = new Vector2(16);
        close.DrawOrder = 101;
        close.TileSize = 16;
        title.AddChild(close);

        foreach (ThingOption option in thing.Cell.Import.Options)
        {
            if (option.Type == "Checkbox") Checkbox(thing, option, flex);
            if (option.Type == "TextInput") TextInput(thing, option, flex);
            if (option.Type == "Dropdown") Dropdown(thing, option, flex);
        }

        textControl.Refresh();
        title.Refresh();
        flex.Refresh();

        editor.AddChild(root);

        return root;
    }

    Control Checkbox(Thing thing, ThingOption option, Control flex)
    {
        ThingProperty thingOption = thing.Cell.Options.Find(o => o.Name == option.Name);
        if (thingOption == null)
        {
            thingOption = new ThingProperty() { Name = option.Name };
            thingOption.Value = option.Value;
            thing.Cell.Options.Add(thingOption);
        }

        CheckboxControl checkbox = new CheckboxControl(option.Name, option.Value?.ToLower() == "true", (int)flex.Bounds.X);
        checkbox.OnPressed += () =>
        {
            thingOption.Value = checkbox.Value.ToString().ToLower();
        };
        flex.AddChild(checkbox);

        return checkbox;
    }

    Control TextInput(Thing thing, ThingOption option, Control flex)
    {
        ThingProperty thingOption = thing.Cell.Options.Find(o => o.Name == option.Name);
        if (thingOption == null)
        {
            thingOption = new ThingProperty() { Name = option.Name };
            thingOption.Value = option.Value;
            thing.Cell.Options.Add(thingOption);
        }
        TextInputControl control = new TextInputControl(option.Name);
        control.Bounds.X = flex.Bounds.X;
        control.Bounds.Y = 16;
        control.Texture = Library.Textures["BoxInsideSlice"];
        control.DrawOrder = 10;
        control.HighlightColor = Theme.Dark;
        control.Color = PaletteBasic.White;
        control.TextPadding.X = 3;
        control.Padding = new Vector2(1, 0);
        control.TextHighlightColor = PaletteBasic.White;
        control.Text = thingOption.Value;
        control.LostFocus = () =>
        {
            thingOption.Value = control.Text;
        };
        flex.AddChild(control);

        control.Refresh();

        return control;
    }

    Control Dropdown(Thing thing, ThingOption option, Control flex)
    {
        ThingProperty thingOption = thing.Cell.Options.Find(o => o.Name == option.Name);
        if (thingOption == null)
        {
            thingOption = new ThingProperty() { Name = option.Name };
            thingOption.Value = option.Value;
            thing.Cell.Options.Add(thingOption);
        }
        DropdownControl control = new DropdownControl(option.Name, option.Options);
        control.Bounds.X = flex.Bounds.X;
        control.Bounds.Y = 16;
        control.Texture = Library.Textures["BoxInsideSlice"];
        control.DrawOrder = 10;
        control.HighlightColor = Theme.Dark;
        control.Color = PaletteBasic.White;
        control.TextPadding.X = 3;
        control.TextHighlightColor = PaletteBasic.White;
        control.Text = thingOption.Value;
        control.LostFocus = () =>
        {
            thingOption.Value = control.Text;
        };
        flex.AddChild(control);

        control.Refresh();

        return control;
    }
}