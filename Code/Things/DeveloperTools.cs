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
    Thing playTarget;

    DeveloperDisplay display = DeveloperDisplay.None;

    int interfaceWidth = 180;
    int interfaceHeight = CONSTANTS.VIRTUAL_HEIGHT - 4;

    public override void Init()
    {
        base.Init();

        DrawMode = DrawMode.Absolute;
    }

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();

        Shortcuts();
    }

    void ToggleEditor(bool show)
    {
        if (Editor == null) Editor = AddChild(new Editor()) as Editor;
        if (show) Edit();
        else Play();
    }

    void Edit()
    {
        if (Editor == null) Editor = AddChild(new Editor()) as Editor;
        Editor.SetActive(true);
        Editor.SetVisible(true);

        Game.Mode = GameMode.Edit;
        if (Viewport.Target != null) Editor.CameraTarget.Position = Viewport.Target.Position;
        playTarget = Viewport.Target;
        Viewport.Target = Editor.CameraTarget;
        Viewport.LightLayer.Visible = false;
        Viewport.SetScalePixels(true);
    }

    void Play()
    {
        if (Editor == null) Editor = AddChild(new Editor()) as Editor;
        Editor.SetActive(false);
        Editor.SetVisible(false);

        Game.Mode = GameMode.Play;
        if (playTarget != null) Viewport.Target = playTarget;
        Viewport.RelativeLayer.Camera.Zoom = Viewport.VirtualRatio.Value;
        Viewport.RelativeLayer.Camera.Offset = Vector2.Zero;
        Editor.CameraTarget.Position = Viewport.Target.Position;
        Viewport.LightLayer.Visible = true;
        Viewport.SetScalePixels(false);
    }

    void Shortcuts()
    {
        if (Input.IsPressed(KeyboardKey.Grave))
        {
            ToggleEditor(Editor?.Active != true);
        }

        if (Input.IsPressed(KeyboardKey.M))
        {
            Game.Mute = !Game.Mute;
        }

        if (Input.IsPressed(KeyboardKey.One))
        {
            int ratio = Viewport.VirtualRatio.Value - 1;
            if (ratio < 1) ratio = 1;
            Raylib.SetWindowSize(CONSTANTS.VIRTUAL_WIDTH * ratio, CONSTANTS.VIRTUAL_HEIGHT * ratio);
        }

        if (Input.IsPressed(KeyboardKey.Two))
        {
            int ratio = Viewport.VirtualRatio.Value + 1;
            if (ratio > 7) ratio = 7;
            Raylib.SetWindowSize(CONSTANTS.VIRTUAL_WIDTH * ratio, CONSTANTS.VIRTUAL_HEIGHT * ratio);
        }

        if (Input.IsPressed(KeyboardKey.Three))
        {
            Viewport.SetScalePixels(!Viewport.ScalePixels);
        }

        if (Input.IsPressed(KeyboardKey.Four))
        {
            Viewport.LightLayer.Visible = !Viewport.LightLayer.Visible;
            Log.Write("Lights: " + Viewport.LightLayer.Visible);
        }

        if (Input.IsPressed(KeyboardKey.Five))
        {
            if (display == DeveloperDisplay.None) display = DeveloperDisplay.Tree;
            else if (display == DeveloperDisplay.Tree) display = DeveloperDisplay.Log;
            else if (display == DeveloperDisplay.Log) display = DeveloperDisplay.None;
        }

        if (Input.IsPressed(KeyboardKey.L))
        {
            Log.Clear();
            Library.Refresh();
            Play();
            Game.Root.Load(CONSTANTS.START_ROOM);
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
                position: new Vector2(4 + (depth * 6), 4 + (height * (font.BaseSize + fontOffset))) + Viewport.AdjustFromTopLeft,
                color: depth % 2 == 0 ? Colors.White : Colors.Gray2
            );
            height++;
            CrawlTree(c, depth, ref height);
        }
    }

    void DrawThingTree()
    {
        Shapes.DrawSprite(
            texture: Library.Textures["Pixel"],
            position: new Vector2((interfaceWidth / 2f) + 2, CONSTANTS.VIRTUAL_HEIGHT / 2f) + Viewport.AdjustFromTopLeft,
            scale: new Vector2(interfaceWidth, interfaceHeight),
            color: new Color(0, 0, 0, 200)
        );

        Shapes.DrawText(
            font: font,
            text: "TREE",
            position: new Vector2(4, 4) + Viewport.AdjustFromTopLeft,
            color: CONSTANTS.PRIMARY_COLOR
        );

        int height = 1;
        Game.Things.Where(c => c.Parent == null).ToList().ForEach(c =>
        {
            Shapes.DrawText(
                font: font,
                text: $"{c.Id}: {c.Name}",
                position: new Vector2(4, 4 + (height * (font.BaseSize + fontOffset))) + Viewport.AdjustFromTopLeft,
                color: Colors.White
            );
            height++;
        });
        CrawlTree(Game.Root, 0, ref height);
    }

    public override void Draw()
    {
        base.Draw();

        Shapes.DrawText($"{Game.Root?.Zone?.Name}, {Game.Root?.Room?.Name}", new Vector2(4, 2), drawMode: DrawMode.Absolute, color: Colors.White, outlineColor: Colors.Black);

        if (display == DeveloperDisplay.Tree) DrawThingTree();
        else if (display == DeveloperDisplay.Log) Log.Draw();
    }
}
