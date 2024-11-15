namespace Thingus;

/// <summary>The current state of any kind of input (keyboard key, gamepad button, mouse click, etc.)</summary>
public class InputState
{
    public string Name;
    public InputStateType State;

    public InputState() { }
    public InputState(string name, InputStateType state)
    {
        Name = name;
        State = state;
    }
}
