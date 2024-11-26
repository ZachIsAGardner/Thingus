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

    public override void Init()
    {
        base.Init();
    }

    public override void Start()
    {
        base.Start();

        DrawMode = DrawMode.Absolute;
        Editor = AddChild(new Editor()) as Editor;
        Editor.SetActive(false);
        Editor.SetVisible(false);
        Cli = AddChild(new Cli()) as Cli;
        // Cli.SetActive(false);
        // Cli.SetVisible(false);
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
            Editor.SetActive(true);
            Editor.SetVisible(true);

            Viewport.RelativeLayer.ClearColor = PaletteBasic.VeryDarkGray;

            Game.Mode = GameMode.Edit;
            if (Viewport.Target != null) Editor.CameraTarget.Position = Viewport.Target.Position;
            playTarget = Viewport.Target;
            Viewport.Target = Editor.CameraTarget;
            Viewport.LightLayer.Visible = false;
            Viewport.SetScalePixels(true);

            Editor.Focus();
        }
        else
        {
            Play();
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
        if (playTarget != null) Viewport.Target = playTarget;
        Viewport.Zoom = 1;
        Viewport.RelativeLayer.Camera.Offset = Vector2.Zero;
        if (Editor.CameraTarget != null && Viewport.Target != null) Editor.CameraTarget.Position = Viewport.Target.Position;
        Viewport.LightLayer.Visible = true;
        Viewport.SetScalePixels(false);
    }

    void Shortcuts()
    {
        if (Input.IsPressed(KeyboardKey.Grave))
        {
            ToggleCli(!Cli.Active);
        }

        if (Cli.Active) return;

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
            Log.Clear();
            Library.Refresh();
            Editor?.RefreshStamps();
            Play();
            Game.Root.Load(Game.LastMap);
        }
    }

    void CrawlTree(Thing root, int depth, ref int height)
    {
        depth++;
        for (int i = 0; i < root.Children.Count; i++)
        {
            Thing c = root.Children[i];
            Shapes.DrawText(
                font: font,
                text: $"{c.Id}: {c.Name} ({c.TypeName})",
                position: new Vector2((CONSTANTS.VIRTUAL_WIDTH) - (interfaceWidth) + (depth * 6), 4 + (height * (font.BaseSize + fontOffset))) + Viewport.AdjustFromTopRight,
                color: depth % 2 == 0 ? PaletteBasic.White : PaletteAapSplendor128.NightlyAurora
            );
            height++;
            CrawlTree(c, depth, ref height);
        }
    }

    void DrawThingTree()
    {
        Shapes.DrawSprite(
            texture: Library.Textures["Pixel"],
            position: new Vector2((CONSTANTS.VIRTUAL_WIDTH) - (interfaceWidth / 2f) - 4, CONSTANTS.VIRTUAL_HEIGHT / 2f) + Viewport.AdjustFromTopRight,
            scale: new Vector2(interfaceWidth, interfaceHeight),
            color: new Color(0, 50, 0, 245)
        );

        Shapes.DrawText(
            font: font,
            text: "TREE",
            position: new Vector2((CONSTANTS.VIRTUAL_WIDTH) - (interfaceWidth), 4) + Viewport.AdjustFromTopRight,
            color: PaletteBasic.Green
        );

        int height = 1;
        Game.Things.Where(c => c.Parent == null).ToList().ForEach(c =>
        {
            Shapes.DrawText(
                font: font,
                text: $"{c.Id}: {c.Name}",
                position: new Vector2((CONSTANTS.VIRTUAL_WIDTH) - (interfaceWidth), 4 + (height * (font.BaseSize + fontOffset))) + Viewport.AdjustFromTopRight,
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

        if (display == DeveloperDisplay.Tree) DrawThingTree();
        else if (display == DeveloperDisplay.Log) Log.Draw();
    }
}
