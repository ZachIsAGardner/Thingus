using System.Numerics;
using Raylib_cs;

namespace Thingus;

public class Tool
{
    public virtual string Name => "Tool";
    public virtual KeyboardKey Shortcut => KeyboardKey.Null;
    public virtual bool ShowPreview => true;
    public virtual int TileNumber => 1;
    
    protected Editor editor;
    protected Vector2? dragCameraPosition = null;
    protected Vector2? dragMousePosition = null;

    public Tool(Editor editor)
    {
        this.editor = editor;
    }

    public virtual void Update()
    {
        if (Input.LeftMouseButtonIsHeld && editor.LastGridInteractPosition != editor.GridPosition)
        {
            editor.Mouse.ShakeY();
            editor.LastGridInteractPosition = editor.GridPosition;
        }

        if (Input.RightMouseButtonIsHeld && editor.LastGridInteractPosition != editor.GridPosition)
        {
            editor.Mouse.ShakeX();
            editor.LastGridInteractPosition = editor.GridPosition;
        }

        if (Input.LeftMouseButtonIsReleased || Input.RightMouseButtonIsReleased)
        {
            editor.LastGridInteractPosition = null;
            editor.RefreshAutoTilemaps();
            editor.Save();
        }
    }

    public void Picker()
    {
        editor.Mouse.TileNumber = 2;

        if (Input.LeftMouseButtonIsHeld && editor.LastGridInteractPosition != editor.GridPosition)
        {
            editor.PickCell(editor.GridPosition);
        }
    }

    public void Move()
    {
        editor.Mouse.TileNumber = 5;

        if (dragMousePosition == null)
        {
            dragMousePosition = (Raylib.GetMousePosition() / Viewport.RelativeLayer.Camera.Zoom);
            dragCameraPosition = editor.CameraTarget.Position;
        }
        else
        {
            Vector2 difference = dragMousePosition.Value - (Raylib.GetMousePosition() / Viewport.RelativeLayer.Camera.Zoom);
            editor.CameraTarget.Position = dragCameraPosition.Value + difference;
        }
    }

    public virtual void Draw()
    {

    }
}
