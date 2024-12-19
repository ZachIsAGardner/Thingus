
using System.Numerics;
using Newtonsoft.Json;
using Raylib_cs;

namespace Thingus;

public enum DrawMode
{
    Relative,
    Absolute,
    Texture
}

public class Thing
{
    public int Id;
    static int id;
    public string Name;
    public HashSet<string> Tags = new HashSet<string>() { };
    public string Layer;

    public DrawMode DrawMode = DrawMode.Relative;
    public SubViewport SubViewport = null;

    // Relative Position
    public Vector2 Position;
    public Vector2 GlobalPosition => (Parent?.GlobalPosition ?? new Vector2()) + Position;

    public Vector2 Offset;
    public Vector2 GlobalOffset => (Parent?.GlobalOffset ?? new Vector2()) + Offset;

    // Order
    public float UpdateOrder;
    public float UpdateOrderOffset;
    // public float GlobalUpdateOrder => (Parent?.GlobalUpdateOrder ?? 0) + UpdateOrder;
    public float DrawOrder;
    public float DrawOrderOffset;
    // public float GlobalDrawOrder => (Parent?.GlobalDrawOrder ?? 0) + DrawOrder;

    // Status

    public bool Active { get; private set; } = true;
    public bool GlobalActive => (Parent == null || Parent.GlobalActive) && Active;

    public bool Visible { get; private set; } = true;
    public bool GlobalVisible => (Parent == null || Parent.GlobalVisible) && Visible && (!VisibleOnlyInDebug || Game.Mode == GameMode.Edit);

    public bool Removed = false;
    public bool GlobalRemoved => (Parent == null || Parent.GlobalRemoved) && Removed;

    public bool IsAvailable => !GlobalRemoved;

    public Func<float> Delta = () => Time.ModifiedDelta;

    // Extra

    public Thing Parent;
    public List<Thing> Children = new List<Thing>() { };

    public MapCell Cell;
    public ThingImport Import;
    public Map Map;
    // This Thing was created from a sub-resource map.
    public MapCell ParentCell;
    public Map ParentMap;

    public string TypeName;

    float lastUpdateOrder;
    float lastDrawOrder;
    bool lastActive = true;

    public static Thing Create(ThingModel model) => new Thing(model.Name, model.Position, model.DrawMode);

    public Thing() : this(null) { }
    public Thing(string name = null, Vector2? position = null, DrawMode drawMode = DrawMode.Relative, float drawOrder = 0, float updateOrder = 0)
    {
        Id = id;
        id++;

        Game.Things.Add(this);
        if (Game.Root?.Dynamic != null) Game.Root?.Dynamic.AddChild(this);

        TypeName = GetType().Name;
        Name = name;
        if (Name?.HasValue() != true) Name = TypeName;
        Position = position ?? Vector2.Zero;
        DrawMode = drawMode;
        DrawOrder = drawOrder;
        UpdateOrder = updateOrder;
        
        Type type = GetType();
        while (type != typeof(Thing))
        {
            string rootTypeName = type.Name;
            if (!Game.TypeThings.ContainsKey(rootTypeName)) Game.TypeThings[rootTypeName] = new List<Thing>() { };
            Game.TypeThings[rootTypeName].Add(this);
            type = type.BaseType;
        }

        Game.QueueReorder();
    }

    public void ApplyImport(ThingImport import)
    {
        Import = import;
        AddTags(import.Tags);
        Layer = import.Layer;
        if (Name?.HasValue() != true || Name == TypeName) Name = import.Name;
    }

    public T Downcast<T>()
    {
        string serializedParent = JsonConvert.SerializeObject(this);
        T result = JsonConvert.DeserializeObject<T>(serializedParent);
        return result;
    }

    public virtual Thing AddChild(Thing child)
    {
        if (child.Parent != null) child.Parent.RemoveChild(child);
        Children.Add(child);
        child.Parent = this;
        return child;
    }

    public virtual Thing RemoveChild(Thing child)
    {
        Children.Remove(child);
        child.Parent = null;
        return child;
    }

    public T GetThing<T>() where T : Thing
    {
        return this as T ?? Children.Find(c => c.GetType() == typeof(T) || c.GetType().IsSubclassOf(typeof(T))) as T;
    }

    public List<T> GetThings<T>() where T : Thing
    {
        return Children.Where(c => c.GetType() == typeof(T) || c.GetType().IsSubclassOf(typeof(T))).Select(c => c as T).ToList();
    }

    public T GetThingWithTag<T>(string tag) where T : Thing
    {
        T result = null;
        if (Tags.Contains(tag)) result = this as T;
        if (result != null) return result;
        result = Children.Find(c => c.Tags.Contains(tag) && c.GetType() == typeof(T) || c.GetType().IsSubclassOf(typeof(T))) as T;
        return result;
    }

    public void AddTag(string tag)
    {
        Tags.Add(tag);
    }

    public void AddTags(List<string> tags)
    {
        Tags.AddRange(tags);
    }

    public void RemoveTag(string tag)
    {
        Tags.Remove(tag);
    }

    public virtual void AfterCreatedFromMap()
    {

    }

    public bool UpdateInEditMode = false;
    public bool VisibleOnlyInDebug = false;
    public bool ExclusiveUpdateInEditMode = false;
    public bool GlobalUpdateInEditMode => (Parent != null && Parent.GlobalUpdateInEditMode) || UpdateInEditMode;
    public bool ShouldUpdate => IsAvailable && Game.Mode != GameMode.Edit && Game.Root.DeveloperTools?.Cli?.Active != true || GlobalUpdateInEditMode || ExclusiveUpdateInEditMode;

    public float? AlphaOverride = null;
    public float? GlobalAlphaOverride => (Parent != null ? Parent?.AlphaOverride : null) ?? AlphaOverride;

    public bool DidStart { get; private set; }
    public virtual void Start()
    {
        DidStart = true;

        if (Import?.VisibleOnlyInEditor == true && Game.Mode == GameMode.Play)
        {
            SetVisible(false);
        }
    }

    public virtual void Update()
    {

    }

    public virtual void LateUpdate()
    {

    }

    public virtual void PersistentUpdate()
    {
        if (Layer != null && Game.Mode == GameMode.Edit)
        {
            // Focused
            if (Layer == LayerControl.CurrentLayer)
            {
                AlphaOverride = null;
                // DrawOrder = 100;
            }
            // Not Focused
            else
            {
                AlphaOverride = 0.1f;
                // DrawOrder = Game.Layers[Layer];
            }
        }

        if (lastUpdateOrder != UpdateOrder) Game.QueueUpdateReorder();
        lastUpdateOrder = UpdateOrder;

        if (lastDrawOrder != DrawOrder + DrawOrderOffset) Game.QueueDrawReorder();
        lastDrawOrder = DrawOrder + DrawOrderOffset;
    }

    public virtual void DrawToTexture()
    {

    }

    public virtual void Draw()
    {

    }

    public virtual void LateDraw()
    {
        if (Game.Mode == GameMode.Edit && Layer != null && Layer == LayerControl.CurrentLayer)
        {
            DrawSprite(
                Library.Textures["GizmoPoint"]
            );
        }
    }

    public virtual void RefreshProjection()
    {

    }

    public virtual void Destroy()
    {
        Removed = true;
        if (Parent != null)
        {
            Parent.RemoveChild(this);
        }
        Children.ToList().ForEach(c => c.Destroy());
        Game.Things.Remove(this);
        // Game.UpdateThings.Remove(this);
        // Game.DrawThings.Remove(this);
        Type type = GetType();
        while (type != typeof(Thing))
        {
            string rootTypeName = type.Name;
            if (!Game.TypeThings.ContainsKey(rootTypeName)) Game.TypeThings[rootTypeName] = new List<Thing>() { };
            Game.TypeThings[rootTypeName].Remove(this);
            type = type.BaseType;
        }
        Game.QueueReorder();
    }

    public virtual void SetActive(bool active)
    {
        bool oldActive = Active;
        Active = active;
        if (oldActive != Active) Game.QueueUpdateReorder();
    }

    public virtual void SetVisible(bool visible)
    {
        bool oldVisible = Visible;
        Visible = visible;
        if (oldVisible != Visible) Game.QueueDrawReorder();
    }

    public void PlaySound(string name, float? volume = null, float? pitch = null, float? pan = null)
    {
        if (pan == null) pan = 1 - ((GlobalPosition.X - Viewport.CameraPosition.X) / CONSTANTS.VIRTUAL_WIDTH);
        if (pan < 0) pan = 0;
        if (pan > 1) pan = 1;

        if (pitch == null)
        {
            pitch = (1f
                - ((0.5f - ((GlobalPosition.X - Viewport.CameraPosition.X) / CONSTANTS.VIRTUAL_WIDTH)).Abs() * 0.5f)
                - ((0.5f - ((GlobalPosition.Y - Viewport.CameraPosition.Y) / CONSTANTS.VIRTUAL_HEIGHT)).Abs() * 0.5f)
            ) + Chance.Range(-0.125f, 0.125f);
        }
        if (pitch < 0) pitch = 0;
        if (pitch > 20) pitch = 20;

        if (volume == null)
        {
            volume = (1f
                - ((0.5f - ((GlobalPosition.X - Viewport.CameraPosition.X) / CONSTANTS.VIRTUAL_WIDTH)).Abs() * 0.5f)
                - ((0.5f - ((GlobalPosition.Y - Viewport.CameraPosition.Y) / CONSTANTS.VIRTUAL_HEIGHT)).Abs() * 0.5f)
            ) + Chance.Range(-0.125f, 0.125f);
        }

        if (volume < 0) volume = 0;
        if (volume > 1) volume = 1;


        Game.PlaySound(name, volume.Value, pitch.Value, pan.Value);
    }

    public void Clear()
    {
        Children.ToList().ForEach(c => c.Destroy());
        Children = new List<Thing>() { };
    }

    public void DrawSprite(Texture2D? texture, Vector2? position = null, int tileNumber = 0, int tileSize = 0, float rotation = 0, Color? color = null, Vector2? scale = null, Vector2? origin = null, bool flipHorizontally = false, bool flipVertically = false, Rectangle? source = null, Rectangle? destination = null)
    {
        Shapes.DrawSprite(texture, position ?? GlobalPosition, tileNumber, tileSize, rotation, color, scale, DrawMode, origin, flipHorizontally, flipVertically, source, destination);
    }

    public void DrawText(object text, Vector2? position = null, Font? font = null, Color? color = null, Color? outlineColor = null, OutlineStyle outlineStyle = OutlineStyle.Full, TextJustification justification = TextJustification.TopLeft)
    {
        Shapes.DrawText(text?.ToString() ?? "null", position ?? GlobalPosition, font, color, DrawMode, outlineColor, outlineStyle, justification);
    }

    public void DrawNineSlice(Texture2D? texture, Vector2? position = null, int tileSize = 5, int width = 15, int height = 15, Vector2? origin = null, Color? color = null)
    {
        Shapes.DrawNineSlice(texture, position ?? GlobalPosition, tileSize, width, height, origin, DrawMode, color);
    }

    public bool IsOnScreen()
    {
        Vector2 p = Utility.WorldToScreenPosition(GlobalPosition);
        if (p.X > 0 && p.X < CONSTANTS.VIRTUAL_WIDTH && p.Y > 0 && p.Y < CONSTANTS.VIRTUAL_HEIGHT)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
