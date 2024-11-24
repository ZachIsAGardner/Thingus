using System.Numerics;
using Raylib_cs;

namespace Thingus;

public class ViewportLayer
{
    public bool Visible = true;
    public int Index = 0;
    public Rectangle Rectangle = new Rectangle();
    public Camera2D Camera = new Camera2D() { Zoom = 1f };
    public RenderTexture2D Texture;
    public bool FollowsTarget = false;
    public Color ClearColor = Colors.Black;

    public ViewportLayer() { }
    public ViewportLayer(int index)
    {
        Index = index;
    }

    public virtual void RefreshProjection()
    {
        if (Viewport.ScalePixels)
        {
            Texture = Raylib.LoadRenderTexture((int)Viewport.Rectangle.Width, (int)Viewport.Rectangle.Height);
            Rectangle = new Rectangle(0, 0, (float)Texture.Texture.Width, -(float)Texture.Texture.Height);

            Camera.Zoom = Viewport.VirtualRatio.Value;
            if (Camera.Offset != Vector2.Zero)
            {
                float change = (float)Viewport.VirtualRatio.Value / (float)Viewport.LastVirtualRatio.Value;
                Camera.Offset *= change;
            }

            Raylib.SetTextureFilter(Texture.Texture, TextureFilter.Point);
        }
        else
        {
            Texture = Raylib.LoadRenderTexture(CONSTANTS.VIRTUAL_WIDTH, CONSTANTS.VIRTUAL_HEIGHT);
            Rectangle = new Rectangle(0, 0, (float)Texture.Texture.Width, -(float)Texture.Texture.Height);

            Camera.Zoom = 1f;
            Camera.Offset = Vector2.Zero;
            Raylib.SetTextureFilter(Texture.Texture, TextureFilter.Point);
        }
    }

    public virtual void Update()
    {

    }

    public virtual void DrawToTexture()
    {

    }

    public virtual void DrawToWindow()
    {
        Raylib.DrawTexturePro(
            texture: Texture.Texture,
            source: Rectangle,
            dest: Viewport.Rectangle,
            origin: new Vector2(0f, 0f),
            rotation: Viewport.Rotation,
            tint: Colors.White.ToRaylib()
        );
    }
}