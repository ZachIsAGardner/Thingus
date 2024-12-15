namespace Thingus;

public class Button
{
    public List<KeyboardKey> Keys = new List<KeyboardKey>() { };
    public List<GamepadButton> Buttons = new List<GamepadButton>() { };
    public List<GamepadAxisDirection> Axises = new List<GamepadAxisDirection>() { };
    public List<MouseButton> Clicks = new List<MouseButton>() { };

    public Button(List<KeyboardKey> keys, List<GamepadButton> buttons = null, List<GamepadAxisDirection> axises = null, List<MouseButton> clicks = null)
    {
        Keys = keys;
        if (buttons != null) Buttons = buttons; 
        if (axises != null) Axises = axises; 
        if (clicks != null) Clicks = clicks; 
    }

    public bool IsPressed =>
        Keys.Any(k => Input.IsPressed(k))
            || Buttons.Any(b => Input.IsPressed(b))
            || Axises.Any(a => Input.IsPressed(a))
            || Clicks.Any(c => Input.IsPressed(c));

    public bool IsHeld =>
        Keys.Any(k => Input.IsHeld(k))
            || Buttons.Any(b => Input.IsHeld(b))
            || Axises.Any(a => Input.IsHeld(a))
            || Clicks.Any(c => Input.IsHeld(c));

    public bool IsReleased =>
        Keys.Any(k => Input.IsReleased(k))
            || Buttons.Any(b => Input.IsReleased(b))
            || Axises.Any(a => Input.IsReleased(a))
            || Clicks.Any(c => Input.IsReleased(c));

    public bool IsRepeating =>
        Keys.Any(k => Input.IsRepeating(k))
            || Buttons.Any(b => Input.IsRepeating(b))
            || Axises.Any(a => Input.IsRepeating(a))
            || Clicks.Any(c => Input.IsRepeating(c));

    public string DisplayName()
    {
        if (Input.IsGamepadFocused)
        {
            string key = Buttons.FirstOrDefault().ToString();
            return Input.GetGamepadPrompt(key);
        }
        else
        {
            return Keys.FirstOrDefault().ToString();
        }
    }
}