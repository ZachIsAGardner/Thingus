using System.Numerics;
using Raylib_cs;

namespace Thingus;

public class AnimationEvent
{
    public List<int> Frames;
    public Action<Thing> Action;

    public AnimationEvent() { }
    public AnimationEvent(int frame, Action<Thing> action)
    {
        Frames = new List<int>() { frame };
        Action = action;
    }
    public AnimationEvent(List<int> frames, Action<Thing> action)
    {
        Frames = frames;
        Action = action;
    }
}

public class AnimationState
{
    public string Name;
    public int Start;
    public int End;
    public bool Loop = true;
    public int? LoopStart = null;
    public float Speed = 10;
    public List<AnimationEvent> Events = new List<AnimationEvent>() { };
    public bool Reverse;
    public int? AutoStep = null;

    public AnimationState() { }
    public AnimationState(int start, int end)
    {
        Name = "Idle";
        Start = start;
        End = end;
    }
    public AnimationState(string name = "Default", int start = 0, int? end = null, bool loop = true, float speed = 10, List<AnimationEvent> events = null, int? autoStep = null, int? loopStart = null)
    {
        Name = name;
        Start = start;
        End = end ?? start;
        Loop = loop;
        Speed = speed;
        if (events != null) Events = events;
        if (end < start)
        {
            Reverse = true;
        }
        AutoStep = autoStep;
        LoopStart = loopStart;
    }
}

public class Sprite : Thing
{
    // Relative Rotation
    public float Rotation = 0f;
    // public float GlobalRotation => (Parent?.GlobalRotation ?? 0f) + Rotation;
    // Relative Scale
    public Vector2 Scale = new Vector2(1, 1);
    // public Vector2 GlobalScale => ((Parent?.GlobalScale ?? new Vector2(1)) - new Vector2(1)) + Scale;
    public Color Color = PaletteBasic.White;
    public Texture2D Texture;
    public float TileNumber = 0;
    public float TileNumberOffset = 0;
    public int TileSize = 0;
    public bool FlipHorizontally = false;
    public bool FlipVertically = false;
    public AdjustFrom AdjustFrom = AdjustFrom.Auto;

    // Shake
    float shakeXOffset;
    float shakeXDuration = 0;
    float shakeXTime = 0;
    float shakeXMagnitude = 1;
    int shakeXDirection = 1;
    float shakeXStep = 0;

    float shakeYOffset;
    float shakeYDuration = 0;
    float shakeYTime = 0;
    float shakeYMagnitude = 1;
    int shakeYDirection = 1;
    float shakeYStep = 0;

    float shakeStepDuration = 0.01f;

    public AnimationState State;
    string lastState;
    int lastFlooredTileNumber;
    public int Offset = 0;

    // Create from Map
    public static Sprite Create(Thing root, ThingModel model)
        => new Sprite(
            name: model.Name,
            position: model.Position,
            drawMode: DrawMode.Relative,
            tileSize: CONSTANTS.TILE_SIZE,
            tileNumber: model.TileNumber,
            color: PaletteBasic.White
        );
    public Sprite() { }
    // Create from code
    public Sprite(
        string name, Vector2? position = null, DrawMode drawMode = DrawMode.Relative, float drawOrder = 0, float updateOrder = 0,
        int? tileSize = null, int? tileNumber = null, Color? color = null, Vector2? scale = null, float rotation = 0f, AdjustFrom adjustFrom = AdjustFrom.Auto
    ) : base(name, position, drawMode, drawOrder, updateOrder)
    {
        Texture = Library.Textures[name];

        if (tileSize != null) TileSize = tileSize.Value;
        if (tileNumber != null) TileNumber = tileNumber.Value;
        if (color != null) Color = color.Value;
        if (scale != null) Scale = scale.Value;
        Rotation = rotation;
        AdjustFrom = adjustFrom;
    }

    public void ShakeX(float magnitude = 2, float duration = 0.25f)
    {
        shakeXMagnitude = magnitude;
        shakeXTime = shakeXDuration = duration;
        shakeXStep = 0;
    }

    public void ShakeY(float magnitude = 2, float duration = 0.25f)
    {
        shakeYMagnitude = magnitude;
        shakeYTime = shakeYDuration = duration;
        shakeYStep = 0;
    }

    public void Shake(float magnitude = 2, float duration = 0.25f)
    {
        if (Chance.OneIn(2)) ShakeX(magnitude, duration);
        else ShakeY(magnitude, duration);
    }

    void CheckShake()
    {
        if (shakeXTime > 0)
        {
            shakeXTime -= Time.Delta;
            shakeXStep -= Time.Delta;
            if (shakeXStep <= 0)
            {
                shakeXOffset = shakeXMagnitude * shakeXDirection * (shakeXTime / shakeXDuration);
                shakeXDirection *= -1;
                shakeXStep = shakeStepDuration;
            }
        }
        else
        {
            shakeXOffset = 0;
        }

        if (shakeYTime > 0)
        {
            shakeYTime -= Time.Delta;
            shakeYStep -= Time.Delta;
            if (shakeYStep <= 0)
            {
                shakeYOffset = shakeYMagnitude * shakeYDirection * (shakeYTime / shakeYDuration);
                shakeYDirection *= -1;
                shakeYStep = shakeStepDuration;
            }
        }
        else
        {
            shakeYOffset = 0;
        }
    }

    public override void Update()
    {
        base.Update();

        CheckShake();

        if (State != null)
        {
            if (lastState != State.Name)
            {
                TileNumber = State.Start;
            }
            lastState = State.Name;

            TileNumber += State.Speed * Time.Delta;
            if (TileNumber >= State.End + 1)
            {
                if (State.Loop) TileNumber = State.Start;
                else TileNumber = State.End;
            }

            int flooredTileNumber = (int)Math.Floor(TileNumber);

            // Process events
            if ((flooredTileNumber != lastFlooredTileNumber || State.Name != lastState) && State.Events != null && State.Events.Count > 0)
            {
                AnimationEvent animationEvent = State.Events.Find(e => e.Frames.Contains(flooredTileNumber));
                if (animationEvent != null)
                {
                    animationEvent.Action(this);
                }
            }

            lastFlooredTileNumber = flooredTileNumber;
            lastState = State.Name;
        }
    }

    public override void Draw()
    {
        base.Draw();

        Shapes.DrawSprite(
            texture: Texture,
            position: GlobalPosition
                + new Vector2(shakeXOffset, shakeYOffset),
            tileSize: TileSize,
            tileNumber: (int)(TileNumber + TileNumberOffset).Floor(),
            rotation: Rotation,
            scale: Scale,
            color: Color,
            drawMode: DrawMode,
            flipHorizontally: FlipHorizontally,
            flipVertically: FlipVertically,
            adjustFrom: AdjustFrom
        );

        // DrawText(GlobalPosition, outlineColor: PaletteBasic.Black, color: PaletteBasic.White, position: GlobalPosition);
        // DrawText(Position, outlineColor: PaletteBasic.Black, color: PaletteBasic.White, position: GlobalPosition + new Vector2(0, 16));
    }
}
