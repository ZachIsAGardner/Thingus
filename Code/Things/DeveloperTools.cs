using System.Numerics;
using Raylib_cs;

namespace Thingus;

public enum DeveloperDisplay
{
    None,
    Tree,
    Log
}

public class DeveloperTools : Thing
{
    Font font = Library.FontSmall;
    int fontOffset = 1;
 
    public Editor Editor;
    public Cli Cli;
    Thing playTarget;

    DeveloperDisplay display = DeveloperDisplay.None;

    int interfaceWidth = 180;
    int interfaceHeight = CONSTANTS.VIRTUAL_HEIGHT - 4;

    public DeveloperTools()
    {
        UpdateInEditMode = true;
    }

    public override void Start()
    {
        base.Start();

        DrawMode = DrawMode.Absolute;
        Editor = AddChild(new Editor()) as Editor;
        Editor.SetActive(false);
        Editor.SetVisible(false);
        Cli = AddChild(new Cli()) as Cli;
        Cli.SetActive(false);
        Cli.SetVisible(false);
    }

    public override void Update()
    {
        base.Update();

        Shortcuts();
    }

    void ToggleEditor(bool show)
    {
        if (show)
        {
            Game.Root.Load(Library.Maps[Game.EditingMap]);
            Game.Mode = GameMode.Edit;
            
            Editor.SetActive(true);
            Editor.SetVisible(true);

            Viewport.RelativeLayer.ClearColor = PaletteBasic.VeryDarkGray;

            // if (Viewport.Target != null) Editor.CameraTarget.Position = Viewport.Target.Position;
            // playTarget = Viewport.Target;
            // Viewport.Target = Editor.CameraTarget;
            Viewport.LightLayer.Visible = false;
            Viewport.SetScalePixels(true);

            Editor.Focus();

        }
        else
        {
            LoadLastMap();
        }
    }

    void ToggleCli(bool show)
    {
        if (show)
        {
            Cli.SetActive(true);
            Cli.SetVisible(true);

            // Game.Mode = GameMode.Edit;
        }
        else
        {
            Cli.SetActive(false);
            Cli.SetVisible(false);
        }
    }

    void Play()
    {
        Viewport.RelativeLayer.ClearColor = PaletteBasic.Black;
        Editor.SetActive(false);
        Editor.SetVisible(false);
        Cli.SetActive(false);
        Cli.SetVisible(false);
        Game.Mode = GameMode.Play;

        // if (playTarget != null) Viewport.Target = playTarget;
        // if (Editor.CameraTarget != null && Viewport.Target != null) Editor.CameraTarget.Position = Viewport.Target.Position;
        Viewport.Zoom = 1;
        Viewport.RelativeLayer.Camera.Offset = Vector2.Zero;

        Viewport.LightLayer.Visible = true;
        Viewport.SetScalePixels(false);
    }

    void Shortcuts()
    {
        if (Input.IsPressed(KeyboardKey.Grave))
        {
            ToggleCli(!Cli.Active);
        }

        if (Cli.Active || Control.FocusedControl != null) return;

        if (Input.IsPressed(KeyboardKey.One))
        {
            ToggleEditor(!Editor.Active);
        }

        if (Input.IsPressed(KeyboardKey.Two))
        {
            if (display == DeveloperDisplay.None) display = DeveloperDisplay.Tree;
            else if (display == DeveloperDisplay.Tree) display = DeveloperDisplay.Log;
            else if (display == DeveloperDisplay.Log) display = DeveloperDisplay.None;
        }

        if (Input.IsPressed(KeyboardKey.M))
        {
            Game.Mute = !Game.Mute;
        }

        if (Input.IsPressed(KeyboardKey.Eight))
        {
            Raylib.SetWindowSize(
                (int)((CONSTANTS.VIRTUAL_WIDTH * Viewport.VirtualRatio.Value) * Viewport.Allowance), 
                (int)((CONSTANTS.VIRTUAL_HEIGHT * Viewport.VirtualRatio.Value) * Viewport.Allowance)
            );
        }

        if (Input.IsPressed(KeyboardKey.Nine))
        {
            int ratio = Viewport.VirtualRatio.Value - 1;
            if (ratio < 1) ratio = 1;
            Raylib.SetWindowSize(CONSTANTS.VIRTUAL_WIDTH * ratio, CONSTANTS.VIRTUAL_HEIGHT * ratio);
        }

        if (Input.IsPressed(KeyboardKey.Zero))
        {
            int ratio = Viewport.VirtualRatio.Value + 1;
            if (ratio > 7) ratio = 7;
            Raylib.SetWindowSize(CONSTANTS.VIRTUAL_WIDTH * ratio, CONSTANTS.VIRTUAL_HEIGHT * ratio);
        }

        // if (Input.CtrlIsHeld && Input.IsPressed(KeyboardKey.Three))
        // {
        //     Viewport.SetScalePixels(!Viewport.ScalePixels);
        // }

        // if (Input.CtrlIsHeld && Input.IsPressed(KeyboardKey.Four))
        // {
        //     Viewport.LightLayer.Visible = !Viewport.LightLayer.Visible;
        //     Log.Write("Lights: " + Viewport.LightLayer.Visible);
        // }

        if (Input.IsPressed(KeyboardKey.L))
        {
            LoadLastMap();
        }
    }

    void LoadLastMap()
    {
        TokenGrid.Reset();
        Log.Clear();
        Library.Refresh();
        Game.Root.Load(Library.Maps[Game.LastFocusedMap]);
        
        if (Input.ShiftIsHeld)
        {
            if (Editor != null)
            {
                Editor.Destroy();
                Editor = AddChild(new Editor()) as Editor;
            }

            ToggleEditor(true);
        }
        else
        {
            Play();
        }
    }

    void CrawlTree(Thing root, int depth, ref int height)
    {
        depth++;
        for (int i = 0; i < root.Children.Count; i++)
        {
            Thing c = root.Children[i];
            DrawText(
                font: font,
                text: $"{c.Id}: {c.Name} ({c.TypeName})",
                position: Viewport.Adjust(new Vector2((CONSTANTS.VIRTUAL_WIDTH) - (interfaceWidth) + (depth * 6), 4 + (height * (font.BaseSize + fontOffset))), AdjustFrom.TopRight),
                color: depth % 2 == 0 ? PaletteBasic.White : Theme.Light
            );
            height++;
            CrawlTree(c, depth, ref height);
        }
    }

    void DrawThingTree()
    {
        DrawSprite(
            texture: Library.Textures["Pixel"],
            position: Viewport.Adjust(new Vector2((CONSTANTS.VIRTUAL_WIDTH) - (interfaceWidth / 2f) - 4, CONSTANTS.VIRTUAL_HEIGHT / 2f), AdjustFrom.TopRight),
            scale: new Vector2(interfaceWidth, interfaceHeight),
            color: Theme.Dark.WithAlpha(0.8f)
        );

        DrawText(
            font: font,
            text: "TREE",
            position: Viewport.Adjust(new Vector2((CONSTANTS.VIRTUAL_WIDTH) - (interfaceWidth), 4), AdjustFrom.TopRight),
            color: Theme.Primary
        );

        int height = 2;
        Game.Things.Where(c => c.Parent == null).ToList().ForEach(c =>
        {
            DrawText(
                font: font,
                text: $"{c.Id}: {c.Name}",
                position: Viewport.Adjust(new Vector2((CONSTANTS.VIRTUAL_WIDTH) - (interfaceWidth), 4 + (height * (font.BaseSize + fontOffset))), AdjustFrom.TopRight),
                color: PaletteBasic.White
            );
            height++;
        });
        CrawlTree(Game.Root, 0, ref height);
    }

    public override void Draw()
    {
        base.Draw();

        if (Game.FrameAdvance) DrawText(
            "FRAME ADVANCE", 
            position: new Vector2((CONSTANTS.VIRTUAL_WIDTH / 2f) - (Raylib.MeasureText("FRAME ADVANCE", Library.FontSmall.BaseSize) / 2f), 4), 
            color: PaletteBasic.Red, 
            outlineColor: PaletteBasic.Black,
            font: Library.FontSmall
        );

        // DrawText(
        //     Raylib.GetFPS(), 
        //     color: PaletteBasic.White, 
        //     outlineColor: PaletteBasic.Black,
        //     position: new Vector2((CONSTANTS.VIRTUAL_WIDTH / 2f), 4)
        // );

        if (display == DeveloperDisplay.Tree) DrawThingTree();
        else if (display == DeveloperDisplay.Log) Log.Draw();
    }
}
