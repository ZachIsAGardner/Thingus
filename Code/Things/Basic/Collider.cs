using System.Numerics;
using Raylib_cs;

namespace Thingus;

public class CollisionInfo
{
    public bool Left;
    public bool Right;
    public bool Up;
    public bool Down;
    public bool Any => Left || Right || Down || Up;

    public void Reset()
    {
        Up = false;
        Down = false;
        Left = false;
        Right = false;
    }
}

public class GridCollision
{
    public Vector2 Position;
    public Vector2 GlobalTop => Position + new Vector2(0, -CONSTANTS.TILE_SIZE / 2);
    public Vector2 GlobalBottom => Position + new Vector2(0, CONSTANTS.TILE_SIZE / 2);
    public Vector2 GlobalLeft => Position + new Vector2(-CONSTANTS.TILE_SIZE / 2, 0);
    public Vector2 GlobalRight => Position + new Vector2(CONSTANTS.TILE_SIZE / 2, 0);

    public GridCollision() { }
    public GridCollision(int column, int row)
    {
        Position = new Vector2(column * CONSTANTS.TILE_SIZE, row * CONSTANTS.TILE_SIZE);
    }
    public GridCollision(Vector2 position)
    {
        Position = position;
    }
}

public enum CollisionMask
{
    NoCollision,
    Collision
}

public class Collider : Thing
{
    public Vector2 Bounds;

    public Vector2 GlobalTopLeft => GlobalPosition + new Vector2(-Bounds.X / 2f, -Bounds.Y / 2f);
    public Vector2 GlobalTop => GlobalPosition + new Vector2(0, -Bounds.Y / 2f);
    public Vector2 GlobalTopRight => GlobalPosition + new Vector2(Bounds.X / 2f, -Bounds.Y / 2f);
    public Vector2 GlobalBottom => GlobalPosition + new Vector2(0, Bounds.Y / 2f);
    public Vector2 GlobalBottomLeft => GlobalPosition + new Vector2(-Bounds.X / 2f, Bounds.Y / 2f);
    public Vector2 GlobalLeft => GlobalPosition + new Vector2(-Bounds.X / 2f, 0);
    public Vector2 GlobalBottomRight => GlobalPosition + new Vector2(Bounds.X / 2f, Bounds.Y / 2f);
    public Vector2 GlobalRight => GlobalPosition + new Vector2(Bounds.X / 2f, 0);

    public Vector2 Velocity;
    public int RaycastCount = 3;
    public int RaycastStep = 0;
    float skin = 0.1f;
    public bool CheckForCollisions = true;

    public int Power = 1;

    List<Collider> possibleCollisions = new List<Collider>() { };

    public CollisionInfo Info = new CollisionInfo() { };

    public Collider() { }
    public Collider(Vector2 position, Vector2 bounds): base(position: position)
    {
        Bounds = bounds;
        // Sprite sprite = AddChild(new Sprite("Pixel", color: new Color(0, 255, 0, 100), scale: Bounds, drawOrder: 100)) as Sprite;
        // sprite.VisibleOnlyInDebug = true;
    }

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();

        if (Velocity == Vector2.Zero) return;

        if (CheckForCollisions)
        {
            possibleCollisions = Game.GetThings<Collider>().Where(c => c.Tags.Contains("Collision") && c != this).ToList();
            GetRoomCollisions();
            GetBakedTilemapCollisions();
            GetColliderCollisions();
        }

        if (!Info.Left && !Info.Right) Position.X += Velocity.X * Delta();
        if (!Info.Up && !Info.Down) Position.Y += Velocity.Y * Delta();
    }

    public virtual void LandedHit(Collider other)
    {
        Destroy();
    }

    public override void LateUpdate()
    {
        base.LateUpdate();

        Info.Reset();
    }

    void GetRoomCollisions()
    {
        Room room = Game.Root.Room;
        if (room == null) return;
        Vector2? hit = null;

        // Vertical

        foreach (Vector2 raycast in BuildVerticalRaycasts())
        {
            if (hit != null) break;
            if (raycast.Y > room.Bottom.Y) hit = room.Bottom;
            if (raycast.Y < room.Top.Y) hit = room.Top;
        }

        if (hit != null)
        {
            if (Velocity.Y < 0)
            {
                Position.Y = hit.Value.Y + (Bounds.Y / 2f) + (Position.Y - GlobalPosition.Y);
                Info.Up = true;
            }
            else
            {
                Position.Y = hit.Value.Y - (Bounds.Y / 2f) + (Position.Y - GlobalPosition.Y);
                Info.Down = true;
            }
        }
        hit = null;

        // Horizontal

        if (Velocity.X != 0)
        {
            foreach (var raycast in BuildHorizontalRaycasts())
            {
                if (hit != null) break;
                if (raycast.X > room.Right.X) hit = room.Right;
                if (raycast.X < room.Left.X) hit = room.Left;
            }

            if (hit != null)
            {
                if (Velocity.X < 0)
                {
                    Position.X = hit.Value.X + (Bounds.X / 2f) + (Position.X - GlobalPosition.X);
                    Info.Left = true;
                }
                else
                {
                    Position.X = hit.Value.X - (Bounds.X / 2f) + (Position.X - GlobalPosition.X);
                    Info.Right = true;
                }
            }
        }
    }

    void GetBakedTilemapCollisions()
    {
        // Vertical

        List<GridCollision> verticalHits = new List<GridCollision>() { };

        foreach (Vector2 raycast in BuildVerticalRaycasts())
        {
            GridCollision hit = CheckGridPoint(raycast);
            if (hit != null)
            {
                verticalHits.Add(hit);
                break;
            }
        }

        GridCollision verticalHit = null;
        if (verticalHits.Count >= 1)
        {
            verticalHit = verticalHits.First();
        }

        if (verticalHit != null)
        {
            if (Velocity.Y < 0)
            {
                Position.Y = verticalHit.GlobalBottom.Y + (Bounds.Y / 2f) + (Position.Y - GlobalPosition.Y);
                Info.Up = true;
            }
            else
            {
                Position.Y = verticalHit.GlobalTop.Y - (Bounds.Y / 2f) + (Position.Y - GlobalPosition.Y);
                Info.Down = true;
            }
        }

        // Horizontal

        if (Velocity.X != 0)
        {
            List<GridCollision> horizontalHits = new List<GridCollision>() { };

            foreach (var raycast in BuildHorizontalRaycasts())
            {
                GridCollision hit = CheckGridPoint(raycast);
                if (hit != null)
                {
                    horizontalHits.Add(hit);
                    break;
                }
            }

            GridCollision horizontalHit = null;
            if (horizontalHits.Count >= 1)
            {
                horizontalHit = horizontalHits.First();
            }

            if (horizontalHit != null && horizontalHit != verticalHit)
            {
                if (Velocity.X < 0)
                {
                    Position.X = horizontalHit.GlobalRight.X + (Bounds.X / 2f) + (Position.X - GlobalPosition.X);
                    Info.Left = true;
                }
                else
                {
                    Position.X = horizontalHit.GlobalLeft.X - (Bounds.X / 2f) + (Position.X - GlobalPosition.X);
                    Info.Right = true;
                }
            }
        }
    }

    private void GetColliderCollisions()
    {
        // Vertical

        List<Collider> verticalHits = new List<Collider>() { };

        foreach (Collider possibleCollision in possibleCollisions)
        {
            foreach (Vector2 raycast in BuildVerticalRaycasts())
            {
                if (CheckColliderPoint(raycast, possibleCollision))
                {
                    verticalHits.Add(possibleCollision);
                    break;
                }
            }
        }

        Collider verticalHit = null;
        if (verticalHits.Count >= 1)
        {
            verticalHit = verticalHits.First();
        }

        if (verticalHit != null)
        {
            if (Velocity.Y < 0)
            {
                Position.Y = verticalHit.GlobalBottom.Y + (Bounds.Y / 2f) + (Position.Y - GlobalPosition.Y);
                Info.Up = true;
            }
            else
            {
                Position.Y = verticalHit.GlobalTop.Y - (Bounds.Y / 2f) + (Position.Y - GlobalPosition.Y);
                Info.Down = true;
            }
        }

        // Horizontal

        if (Velocity.X != 0)
        {
            List<Collider> horizontalHits = new List<Collider>() { };

            foreach (Collider possibleCollision in possibleCollisions)
            {
                foreach (var raycast in BuildHorizontalRaycasts())
                {
                    // Factory.Point(raycast, Color.Yellow);
                    if (CheckColliderPoint(raycast, possibleCollision))
                    {
                        horizontalHits.Add(possibleCollision);
                        break;
                    }
                }
            }

            Collider horizontalHit = null;
            if (horizontalHits.Count >= 1)
            {
                horizontalHit = horizontalHits.First();
            }

            if (horizontalHit != null && horizontalHit != verticalHit)
            {
                if (Velocity.X < 0)
                {
                    Position.X = horizontalHit.GlobalRight.X + (Bounds.X / 2f) + (Position.X - GlobalPosition.X);
                    Info.Left = true;
                }
                else
                {
                    Position.X = horizontalHit.GlobalLeft.X - (Bounds.X / 2f) + (Position.X - GlobalPosition.X);
                    Info.Right = true;
                }
            }
        }
    }

    List<Vector2> BuildVerticalRaycasts()
    {
        List<Vector2> raycasts = new List<Vector2>();
        float raySpacing = (Bounds.X - (skin * 2f)) / (RaycastCount - 1);

        // Up (-1), Down (1)
        int dir = (Velocity.Y < 0) ? -1 : 1;

        float yTarget = (Velocity.Y < 0 ? GlobalTop.Y : GlobalBottom.Y) + (Velocity.Y * Delta());

        List<float> yPositions = new List<float>() { yTarget };

        // Build additional Positions to check.
        if (RaycastStep > 0)
        {
            float position = GlobalPosition.Y + (RaycastStep * dir);
            if (dir == -1)
            {
                while (position > yTarget)
                {
                    yPositions.Add(position);
                    position -= RaycastStep;
                }
            }
            else
            {
                while (position < yTarget)
                {
                    yPositions.Add(position);
                    position += RaycastStep;
                }
            }
        }

        for (int i = 0; i < RaycastCount; i++)
        {
            foreach (float yPosition in yPositions)
            {
                raycasts.Add(new Vector2(
                    GlobalLeft.X + skin + (raySpacing * i),
                    yPosition
                ));
            }
        }

        return raycasts;
    }

    List<Vector2> BuildHorizontalRaycasts()
    {
        List<Vector2> raycasts = new List<Vector2>();
        float raySpacing = (Bounds.Y - (skin * 2f)) / (RaycastCount - 1);

        int dir = (Velocity.X < 0) ? -1 : 1;

        float xTarget = (Velocity.X < 0 ? GlobalLeft.X : GlobalRight.X) + (Velocity.X * Delta());

        List<float> xPositions = new List<float>() { xTarget };

        // Build additional Positions to check.
        if (RaycastStep > 0)
        {
            float position = GlobalPosition.X + (RaycastStep * dir);
            if (dir == -1)
            {
                while (position > xTarget)
                {
                    xPositions.Add(position);
                    position -= RaycastStep;
                }
            }
            else
            {
                while (position < xTarget)
                {
                    xPositions.Add(position);
                    position += RaycastStep;
                }
            }
        }

        for (int i = 0; i < RaycastCount; i++)
        {
            foreach (float xPosition in xPositions)
            {
                raycasts.Add(new Vector2(
                    xPosition,
                    GlobalTop.Y + skin + (raySpacing * i)
                ));
            }
        }

        return raycasts;
    }

    public Collider CheckTag(List<string> tags)
    {
        List<Collider> others = Game.GetThings<Collider>().Where(c => tags.Any(t => c.Tags.Contains(t)) && CheckCollider(c)).ToList();
        return others.FirstOrDefault();
    }
    public Collider CheckTag(string tag)
    {
        List<Collider> others = Game.GetThings<Collider>().Where(c => c.Tags.Contains(tag) && CheckCollider(c)).ToList();
        return others.FirstOrDefault();
    }

    public bool CheckCollider(Collider other)
    {
        if (other == null || this == other) return false;

        // If one Collider is on left side of other  
        if (GlobalLeft.X >= other.GlobalRight.X || other.GlobalLeft.X >= GlobalRight.X)
        {
            return false;
        }

        // If one Collider is above other  
        if (GlobalTop.Y >= other.GlobalBottom.Y || other.GlobalTop.Y >= GlobalBottom.Y)
        {
            return false;
        }

        return true;
    }

    public bool CheckColliderPoint(Vector2 point, Collider other)
    {
        if (other == null || this == other) return false;

        if (point.X > other.GlobalLeft.X
            && point.X < other.GlobalRight.X
            && point.Y < other.GlobalBottom.Y
            && point.Y > other.GlobalTop.Y)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public GridCollision CheckGridPoint(Vector2 point)
    {
        int r = (int)Math.Round((point.Y - CONSTANTS.TILE_SIZE_HALF) / CONSTANTS.TILE_SIZE);
        int c = (int)Math.Round((point.X - CONSTANTS.TILE_SIZE_HALF) / CONSTANTS.TILE_SIZE);

        // new DebugPoint(point);
        // Log.Clear();
        // Log.Write($"{c}, {r}");

        // if (r < 0 || c < 0 || r > TokenGrid.Bounds.Y - 1 || c > TokenGrid.Bounds.X - 1) return null;

        if (TokenGrid.Get(c, r) == 1)
        {
            return new GridCollision(new Vector2(
                ((c * CONSTANTS.TILE_SIZE) + CONSTANTS.TILE_SIZE_HALF),
                ((r * CONSTANTS.TILE_SIZE) + CONSTANTS.TILE_SIZE_HALF)
            ));
        }
        else
        {
            return null;
        }
    }

    public bool CheckPoint(Vector2 point)
    {
        bool hit = CheckGridPoint(point) != null;
        if (hit) return true;

        foreach (var other in possibleCollisions)
        {
            hit = CheckColliderPoint(point, other);
            if (hit) return true;
        }

        return false;
    }
}
