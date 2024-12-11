using System.Numerics;

namespace Thingus;

public class Daxis
{
    public List<Button> UpButtons = new List<Button>() { };
    public List<Button> DownButtons = new List<Button>() { };
    public List<Button> LeftButtons = new List<Button>() { };
    public List<Button> RightButtons = new List<Button>() { };

    public List<GamepadDaxis> Daxises = new List<GamepadDaxis>() { };

    public Vector2 Value()
    {
        GamepadDaxis daxis = Daxises.Find(a => Input.Daxis(a) != Vector2.Zero);
        if (daxis != GamepadDaxis.None) return Input.Daxis(daxis);

        Vector2 value = Vector2.Zero;

        if (UpButtons.Any(b => b.IsHeld)) value.Y--;
        if (DownButtons.Any(b => b.IsHeld)) value.Y++;
        if (LeftButtons.Any(b => b.IsHeld)) value.X--;
        if (RightButtons.Any(b => b.IsHeld)) value.X++;

        return value;
    }

    public Daxis(List<Button> upButtons = null, List<Button> downButtons = null, List<Button> leftButtons = null, List<Button> rightButtons = null, List<GamepadDaxis> daxises = null)
    {
        if (upButtons != null) UpButtons = upButtons; 
        if (downButtons != null) DownButtons = downButtons; 
        if (leftButtons != null) LeftButtons = leftButtons; 
        if (rightButtons != null) RightButtons = rightButtons; 
        if (daxises != null) Daxises = daxises; 
    }
}