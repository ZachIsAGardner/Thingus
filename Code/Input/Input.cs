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

    public static float MouseWheel => Raylib.GetMouseWheelMove();

    static KeyboardKey currentKey = KeyboardKey.Null;
    public static void Update()
    {
        KeyboardKey newKey = (KeyboardKey)Raylib.GetKeyPressed();

        if (newKey != KeyboardKey.Null)
        {
            currentKey = newKey;
        }
        else 
        {
            if (Input.IsReleased(currentKey))
            {
                currentKey = KeyboardKey.Null;
            }
        }
    }

    public static Vector2 MousePosition(DrawMode drawMode)
    {
        Vector2 mouse = Vector2.Zero;
        if (drawMode == DrawMode.Relative) mouse = Input.MousePositionRelative();
        if (drawMode == DrawMode.Absolute) mouse = Input.MousePositionAbsolute();
        return mouse;
    }

    public static Vector2 MousePositionRelative()
    {
        return Raylib.GetScreenToWorld2D(
            Viewport.ScalePixels
                ? Raylib.GetMousePosition() - ((Viewport.Margin * Viewport.VirtualRatio.Value) * (1 + Viewport.RelativeLayer.Camera.Zoom))
                : (Raylib.GetMousePosition() / Viewport.VirtualRatio.Value) - (Viewport.Margin * (1 + Viewport.VirtualRatio.Value)),
            Viewport.RelativeLayer.Camera
        );
    }

    public static Vector2 MousePositionAbsolute()
    {
        return (Raylib.GetMousePosition() / Viewport.VirtualRatio.Value) 
            - new Vector2(
                Viewport.Margin.X, 
                Viewport.Margin.Y
            );
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

    public static bool IsRepeating(KeyboardKey key)
    {
        return Raylib.IsKeyPressed((Raylib_cs.KeyboardKey)key) || Raylib.IsKeyPressedRepeat((Raylib_cs.KeyboardKey)key);
    }

    public static void ToggleCursor(bool show)
    {
        if (show) Raylib.ShowCursor();
        else Raylib.HideCursor();
    }

    public static bool IsMouseInsideWindow()
    {
        return Raylib.IsCursorOnScreen();
    }

    public static KeyboardKey GetKeyboardKey()
    {
        return (KeyboardKey)Raylib.GetKeyPressed();
    }

    public static string GetKeyboardString()
    {
        KeyboardKey keyboardKey = KeyboardKey.Null;
        if (Input.IsRepeating(currentKey)) keyboardKey = currentKey;
        string result = Input.ShiftIsHeld
            ? KeyboardKeyStringShift[keyboardKey] 
            : KeyboardKeyString[keyboardKey];
        return result;
    }
    
    public static Dictionary<KeyboardKey, string> KeyboardKeyString = new Dictionary<KeyboardKey, string>()
    {
        { KeyboardKey.Null, "" },
        { KeyboardKey.Apostrophe, "'" },
        { KeyboardKey.Comma, "," },
        { KeyboardKey.Minus, "-" },
        { KeyboardKey.Period, "." },
        { KeyboardKey.Slash, "/" },
        { KeyboardKey.Zero, "0" },
        { KeyboardKey.One, "1" },
        { KeyboardKey.Two, "2" },
        { KeyboardKey.Three, "3" },
        { KeyboardKey.Four, "4" },
        { KeyboardKey.Five, "5" },
        { KeyboardKey.Six, "6" },
        { KeyboardKey.Seven, "7" },
        { KeyboardKey.Eight, "8" },
        { KeyboardKey.Nine, "9" },
        { KeyboardKey.Semicolon, ";" },
        { KeyboardKey.Equal, "=" },
        { KeyboardKey.A, "a" },
        { KeyboardKey.B, "b" },
        { KeyboardKey.C, "c" },
        { KeyboardKey.D, "d" },
        { KeyboardKey.E, "e" },
        { KeyboardKey.F, "f" },
        { KeyboardKey.G, "g" },
        { KeyboardKey.H, "h" },
        { KeyboardKey.I, "i" },
        { KeyboardKey.J, "j" },
        { KeyboardKey.K, "k" },
        { KeyboardKey.L, "l" },
        { KeyboardKey.M, "m" },
        { KeyboardKey.N, "n" },
        { KeyboardKey.O, "o" },
        { KeyboardKey.P, "p" },
        { KeyboardKey.Q, "q" },
        { KeyboardKey.R, "r" },
        { KeyboardKey.S, "s" },
        { KeyboardKey.T, "t" },
        { KeyboardKey.U, "u" },
        { KeyboardKey.V, "v" },
        { KeyboardKey.W, "w" },
        { KeyboardKey.X, "x" },
        { KeyboardKey.Y, "y" },
        { KeyboardKey.Z, "z" },
        { KeyboardKey.Space, " " },
        { KeyboardKey.Escape, "" },
        { KeyboardKey.Enter, "" },
        { KeyboardKey.Tab, "" },
        { KeyboardKey.Backspace, "" },
        { KeyboardKey.Insert, "" },
        { KeyboardKey.Delete, "" },
        { KeyboardKey.Right, "" },
        { KeyboardKey.Left, "" },
        { KeyboardKey.Down, "" },
        { KeyboardKey.Up, "" },
        { KeyboardKey.PageUp, "" },
        { KeyboardKey.PageDown, "" },
        { KeyboardKey.Home, "" },
        { KeyboardKey.End, "" },
        { KeyboardKey.CapsLock, "" },
        { KeyboardKey.ScrollLock, "" },
        { KeyboardKey.NumLock, "" },
        { KeyboardKey.PrintScreen, "" },
        { KeyboardKey.Pause, "" },
        { KeyboardKey.F1, "" },
        { KeyboardKey.F2, "" },
        { KeyboardKey.F3, "" },
        { KeyboardKey.F4, "" },
        { KeyboardKey.F5, "" },
        { KeyboardKey.F6, "" },
        { KeyboardKey.F7, "" },
        { KeyboardKey.F8, "" },
        { KeyboardKey.F9, "" },
        { KeyboardKey.F10, "" },
        { KeyboardKey.F11, "" },
        { KeyboardKey.F12, "" },
        { KeyboardKey.LeftShift, "" },
        { KeyboardKey.LeftControl, "" },
        { KeyboardKey.LeftAlt, "" },
        { KeyboardKey.LeftSuper, "" },
        { KeyboardKey.RightShift, "" },
        { KeyboardKey.RightControl, "" },
        { KeyboardKey.RightAlt, "" },
        { KeyboardKey.RightSuper, "" },
        { KeyboardKey.KeyboardMenu, "" },
        { KeyboardKey.LeftBracket, "" },
        { KeyboardKey.Backslash, "\\" },
        { KeyboardKey.RightBracket, "]" },
        { KeyboardKey.Grave, "`" },
        { KeyboardKey.Kp0, "0" },
        { KeyboardKey.Kp1, "1" },
        { KeyboardKey.Kp2, "2" },
        { KeyboardKey.Kp3, "3" },
        { KeyboardKey.Kp4, "4" },
        { KeyboardKey.Kp5, "5" },
        { KeyboardKey.Kp6, "6" },
        { KeyboardKey.Kp7, "7" },
        { KeyboardKey.Kp8, "8" },
        { KeyboardKey.Kp9, "9" },
        { KeyboardKey.KpDecimal, "." },
        { KeyboardKey.KpDivide, "/" },
        { KeyboardKey.KpMultiply, "*" },
        { KeyboardKey.KpSubtract, "-" },
        { KeyboardKey.KpAdd, "+" },
        { KeyboardKey.KpEnter, "" },
        { KeyboardKey.KpEqual, "" },
        { KeyboardKey.Back, "" },
        // { KeyboardKey.Menu, "" },
        { KeyboardKey.VolumeUp, "" },
        { KeyboardKey.VolumeDown, "" }
    };

    public static Dictionary<KeyboardKey, string> KeyboardKeyStringShift = new Dictionary<KeyboardKey, string>()
    {
        { KeyboardKey.Null, "" },
        { KeyboardKey.Apostrophe, "\"" },
        { KeyboardKey.Comma, "<" },
        { KeyboardKey.Minus, "_" },
        { KeyboardKey.Period, ">" },
        { KeyboardKey.Slash, "?" },
        { KeyboardKey.Zero, ")" },
        { KeyboardKey.One, "!" },
        { KeyboardKey.Two, "@" },
        { KeyboardKey.Three, "#" },
        { KeyboardKey.Four, "$" },
        { KeyboardKey.Five, "%" },
        { KeyboardKey.Six, "^" },
        { KeyboardKey.Seven, "&" },
        { KeyboardKey.Eight, "*" },
        { KeyboardKey.Nine, "(" },
        { KeyboardKey.Semicolon, ":" },
        { KeyboardKey.Equal, "+" },
        { KeyboardKey.A, "A" },
        { KeyboardKey.B, "B" },
        { KeyboardKey.C, "C" },
        { KeyboardKey.D, "D" },
        { KeyboardKey.E, "E" },
        { KeyboardKey.F, "F" },
        { KeyboardKey.G, "G" },
        { KeyboardKey.H, "H" },
        { KeyboardKey.I, "I" },
        { KeyboardKey.J, "J" },
        { KeyboardKey.K, "K" },
        { KeyboardKey.L, "L" },
        { KeyboardKey.M, "M" },
        { KeyboardKey.N, "N" },
        { KeyboardKey.O, "O" },
        { KeyboardKey.P, "P" },
        { KeyboardKey.Q, "Q" },
        { KeyboardKey.R, "R" },
        { KeyboardKey.S, "S" },
        { KeyboardKey.T, "T" },
        { KeyboardKey.U, "U" },
        { KeyboardKey.V, "V" },
        { KeyboardKey.W, "W" },
        { KeyboardKey.X, "X" },
        { KeyboardKey.Y, "Y" },
        { KeyboardKey.Z, "Z" },
        { KeyboardKey.Space, " " },
        { KeyboardKey.Escape, "" },
        { KeyboardKey.Enter, "" },
        { KeyboardKey.Tab, "" },
        { KeyboardKey.Backspace, "" },
        { KeyboardKey.Insert, "" },
        { KeyboardKey.Delete, "" },
        { KeyboardKey.Right, "" },
        { KeyboardKey.Left, "" },
        { KeyboardKey.Down, "" },
        { KeyboardKey.Up, "" },
        { KeyboardKey.PageUp, "" },
        { KeyboardKey.PageDown, "" },
        { KeyboardKey.Home, "" },
        { KeyboardKey.End, "" },
        { KeyboardKey.CapsLock, "" },
        { KeyboardKey.ScrollLock, "" },
        { KeyboardKey.NumLock, "" },
        { KeyboardKey.PrintScreen, "" },
        { KeyboardKey.Pause, "" },
        { KeyboardKey.F1, "" },
        { KeyboardKey.F2, "" },
        { KeyboardKey.F3, "" },
        { KeyboardKey.F4, "" },
        { KeyboardKey.F5, "" },
        { KeyboardKey.F6, "" },
        { KeyboardKey.F7, "" },
        { KeyboardKey.F8, "" },
        { KeyboardKey.F9, "" },
        { KeyboardKey.F10, "" },
        { KeyboardKey.F11, "" },
        { KeyboardKey.F12, "" },
        { KeyboardKey.LeftShift, "" },
        { KeyboardKey.LeftControl, "" },
        { KeyboardKey.LeftAlt, "" },
        { KeyboardKey.LeftSuper, "" },
        { KeyboardKey.RightShift, "" },
        { KeyboardKey.RightControl, "" },
        { KeyboardKey.RightAlt, "" },
        { KeyboardKey.RightSuper, "" },
        { KeyboardKey.KeyboardMenu, "" },
        { KeyboardKey.LeftBracket, "{" },
        { KeyboardKey.Backslash, "|" },
        { KeyboardKey.RightBracket, "}" },
        { KeyboardKey.Grave, "" },
        { KeyboardKey.Kp0, "0" },
        { KeyboardKey.Kp1, "1" },
        { KeyboardKey.Kp2, "2" },
        { KeyboardKey.Kp3, "3" },
        { KeyboardKey.Kp4, "4" },
        { KeyboardKey.Kp5, "5" },
        { KeyboardKey.Kp6, "6" },
        { KeyboardKey.Kp7, "7" },
        { KeyboardKey.Kp8, "8" },
        { KeyboardKey.Kp9, "9" },
        { KeyboardKey.KpDecimal, "." },
        { KeyboardKey.KpDivide, "/" },
        { KeyboardKey.KpMultiply, "*" },
        { KeyboardKey.KpSubtract, "-" },
        { KeyboardKey.KpAdd, "+" },
        { KeyboardKey.KpEnter, "" },
        { KeyboardKey.KpEqual, "" },
        { KeyboardKey.Back, "" },
        // { KeyboardKey.Menu, "" },
        { KeyboardKey.VolumeUp, "" },
        { KeyboardKey.VolumeDown, "" }
    };
}
