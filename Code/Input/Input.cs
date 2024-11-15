using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Raylib_cs;

namespace Thingus;

public static class Input
{
    public static List<InputState> KeyStates = new List<InputState>();

    public static bool CtrlIsPressed => Input.IsPressed(KeyboardKey.LeftControl) || Input.IsPressed(KeyboardKey.RightControl);
    public static bool CtrlIsHeld => Input.IsHeld(KeyboardKey.LeftControl) || Input.IsHeld(KeyboardKey.RightControl);
    public static bool CtrlIsReleased => Input.IsReleased(KeyboardKey.LeftControl) || Input.IsReleased(KeyboardKey.RightControl);

    public static bool AltIsPressed => Input.IsPressed(KeyboardKey.LeftAlt) || Input.IsPressed(KeyboardKey.RightAlt);
    public static bool AltIsHeld => Input.IsHeld(KeyboardKey.LeftAlt) || Input.IsHeld(KeyboardKey.RightAlt);
    public static bool AltIsReleased => Input.IsReleased(KeyboardKey.LeftAlt) || Input.IsReleased(KeyboardKey.RightAlt);

    public static bool ShiftIsPressed => Input.IsPressed(KeyboardKey.LeftShift) || Input.IsPressed(KeyboardKey.RightShift);
    public static bool ShiftIsHeld => Input.IsHeld(KeyboardKey.LeftShift) || Input.IsHeld(KeyboardKey.RightShift);
    public static bool ShiftIsReleased => Input.IsReleased(KeyboardKey.LeftShift) || Input.IsReleased(KeyboardKey.RightShift);

    public static bool LeftMouseButtonIsPressed => Raylib.IsMouseButtonPressed(MouseButton.Left);
    public static bool LeftMouseButtonIsHeld => Raylib.IsMouseButtonDown(MouseButton.Left);
    public static bool LeftMouseButtonIsReleased => Raylib.IsMouseButtonReleased(MouseButton.Left);

    public static bool RightMouseButtonIsPressed => Raylib.IsMouseButtonPressed(MouseButton.Right);
    public static bool RightMouseButtonIsHeld => Raylib.IsMouseButtonDown(MouseButton.Right);
    public static bool RightMouseButtonIsReleased => Raylib.IsMouseButtonReleased(MouseButton.Right);

    public static bool MiddleMouseButtonIsPressed => Raylib.IsMouseButtonPressed(MouseButton.Middle);
    public static bool MiddleMouseButtonIsHeld => Raylib.IsMouseButtonDown(MouseButton.Middle);
    public static bool MiddleMouseButtonIsReleased => Raylib.IsMouseButtonReleased(MouseButton.Middle);

    public static Vector2 MousePosition()
    {
        // :D
        return Raylib.GetScreenToWorld2D(
            Viewport.ScalePixels
                ? Raylib.GetMousePosition() - ((Viewport.Margin * Viewport.VirtualRatio.Value) * (1 + Viewport.RelativeLayer.Camera.Zoom))
                : (Raylib.GetMousePosition() / Viewport.VirtualRatio.Value) - (Viewport.Margin * (1 + Viewport.VirtualRatio.Value)),
            Viewport.RelativeLayer.Camera
        );
    }

    public static Vector2 MousePositionAbsolute()
    {
        return (Raylib.GetMousePosition() / Viewport.RelativeLayer.Camera.Zoom);
    }

    public static bool IsPressed(KeyboardKey key)
    {
        return Raylib.IsKeyPressed((Raylib_cs.KeyboardKey)key);
    }

    public static bool IsHeld(KeyboardKey key)
    {
        return Raylib.IsKeyDown((Raylib_cs.KeyboardKey)key);
    }

    public static bool IsReleased(KeyboardKey key)
    {
        return Raylib.IsKeyReleased((Raylib_cs.KeyboardKey)key);
    }
}

public enum KeyboardKey
{
    Null = 0,
    Apostrophe = 39,
    Comma = 44,
    Minus = 45,
    Period = 46,
    Slash = 47,
    Zero = 48,
    One = 49,
    Two = 50,
    Three = 51,
    Four = 52,
    Five = 53,
    Six = 54,
    Seven = 55,
    Eight = 56,
    Nine = 57,
    Semicolon = 59,
    Equal = 61,
    A = 65,
    B = 66,
    C = 67,
    D = 68,
    E = 69,
    F = 70,
    G = 71,
    H = 72,
    I = 73,
    J = 74,
    K = 75,
    L = 76,
    M = 77,
    N = 78,
    O = 79,
    P = 80,
    Q = 81,
    R = 82,
    S = 83,
    T = 84,
    U = 85,
    V = 86,
    W = 87,
    X = 88,
    Y = 89,
    Z = 90,
    Space = 32,
    Escape = 256,
    Enter = 257,
    Tab = 258,
    Backspace = 259,
    Insert = 260,
    Delete = 261,
    Right = 262,
    Left = 263,
    Down = 264,
    Up = 265,
    PageUp = 266,
    PageDown = 267,
    Home = 268,
    End = 269,
    CapsLock = 280,
    ScrollLock = 281,
    NumLock = 282,
    PrintScreen = 283,
    Pause = 284,
    F1 = 290,
    F2 = 291,
    F3 = 292,
    F4 = 293,
    F5 = 294,
    F6 = 295,
    F7 = 296,
    F8 = 297,
    F9 = 298,
    F10 = 299,
    F11 = 300,
    F12 = 301,
    LeftShift = 340,
    LeftControl = 341,
    LeftAlt = 342,
    LeftSuper = 343,
    RightShift = 344,
    RightControl = 345,
    RightAlt = 346,
    RightSuper = 347,
    KeyboardMenu = 348,
    LeftBracket = 91,
    Backslash = 92,
    RightBracket = 93,
    Grave = 96,
    Kp0 = 320,
    Kp1 = 321,
    Kp2 = 322,
    Kp3 = 323,
    Kp4 = 324,
    Kp5 = 325,
    Kp6 = 326,
    Kp7 = 327,
    Kp8 = 328,
    Kp9 = 329,
    KpDecimal = 330,
    KpDivide = 331,
    KpMultiply = 332,
    KpSubtract = 333,
    KpAdd = 334,
    KpEnter = 335,
    KpEqual = 336,
    Back = 4,
    Menu = 82,
    VolumeUp = 24,
    VolumeDown = 25
}

