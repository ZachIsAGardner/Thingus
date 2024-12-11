namespace Thingus;


/// <summary>Gamepad axis</summary>
public enum GamepadAxis
{
    /// <summary>
    /// Gamepad left stick X axis
    /// </summary>
    LeftStickX = 0,

    /// <summary>
    /// Gamepad left stick Y axis
    /// </summary>
    LeftStickY = 1,

    /// <summary>
    /// Gamepad right stick X axis
    /// </summary>
    RightStickX = 2,

    /// <summary>
    /// Gamepad right stick Y axis
    /// </summary>
    RightStickY = 3,

    /// <summary>
    /// Gamepad back trigger left, pressure level: [1..-1]
    /// </summary>
    LeftTrigger = 4,

    /// <summary>
    /// Gamepad back trigger right, pressure level: [1..-1]
    /// </summary>
    RightTrigger = 5
}