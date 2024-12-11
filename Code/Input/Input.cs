using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Raylib_cs;

namespace Thingus;

public static class Input
{
    public static List<InputState> KeyStates = new List<InputState>();

    public static bool CtrlIsPressed => IsPressed(KeyboardKey.LeftControl) || IsPressed(KeyboardKey.RightControl);
    public static bool CtrlIsHeld => IsHeld(KeyboardKey.LeftControl) || IsHeld(KeyboardKey.RightControl);
    public static bool CtrlIsReleased => IsReleased(KeyboardKey.LeftControl) || IsReleased(KeyboardKey.RightControl);

    public static bool AltIsPressed => IsPressed(KeyboardKey.LeftAlt) || IsPressed(KeyboardKey.RightAlt);
    public static bool AltIsHeld => IsHeld(KeyboardKey.LeftAlt) || IsHeld(KeyboardKey.RightAlt);
    public static bool AltIsReleased => IsReleased(KeyboardKey.LeftAlt) || IsReleased(KeyboardKey.RightAlt);

    public static bool ShiftIsPressed => IsPressed(KeyboardKey.LeftShift) || IsPressed(KeyboardKey.RightShift);
    public static bool ShiftIsHeld => IsHeld(KeyboardKey.LeftShift) || IsHeld(KeyboardKey.RightShift);
    public static bool ShiftIsReleased => IsReleased(KeyboardKey.LeftShift) || IsReleased(KeyboardKey.RightShift);

    public static bool LeftMouseButtonIsPressed => Raylib.IsMouseButtonPressed((Raylib_cs.MouseButton)MouseButton.Left);
    public static bool LeftMouseButtonIsHeld => Raylib.IsMouseButtonDown((Raylib_cs.MouseButton)MouseButton.Left);
    public static bool LeftMouseButtonIsReleased => Raylib.IsMouseButtonReleased((Raylib_cs.MouseButton)MouseButton.Left);

    public static bool RightMouseButtonIsPressed => Raylib.IsMouseButtonPressed((Raylib_cs.MouseButton)MouseButton.Right);
    public static bool RightMouseButtonIsHeld => Raylib.IsMouseButtonDown((Raylib_cs.MouseButton)MouseButton.Right);
    public static bool RightMouseButtonIsReleased => Raylib.IsMouseButtonReleased((Raylib_cs.MouseButton)MouseButton.Right);

    public static bool MiddleMouseButtonIsPressed => Raylib.IsMouseButtonPressed((Raylib_cs.MouseButton)MouseButton.Middle);
    public static bool MiddleMouseButtonIsHeld => Raylib.IsMouseButtonDown((Raylib_cs.MouseButton)MouseButton.Middle);
    public static bool MiddleMouseButtonIsReleased => Raylib.IsMouseButtonReleased((Raylib_cs.MouseButton)MouseButton.Middle);

    public static float MouseWheel => Raylib.GetMouseWheelMove();

    public static bool IsGamepadFocused = true;
    public static float Deadzone = 0.1f;

    public static bool Holdup = false;

    static Vector2 lastAbsoluteMousePosition;
    static float mouseSwitchSensitivity = 7.5f;

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
            if (IsReleased(currentKey))
            {
                currentKey = KeyboardKey.Null;
            }
        }

        if (!LeftMouseButtonIsHeld)
        {
            Holdup = false;
        }

        Vector2 difference = lastAbsoluteMousePosition - MousePositionAbsolute();
        if (difference.X.Abs() > mouseSwitchSensitivity || difference.Y.Abs() > mouseSwitchSensitivity)
        {
            IsGamepadFocused = false;
            lastAbsoluteMousePosition = MousePositionAbsolute();
        }

        axisDirectionHeld.ToList().ForEach(b =>
        {
            if (!IsHeld(b.Button))
            {
                axisDirectionReleased.Add(b);
                axisDirectionHeld.Remove(b);
            }
        });
    }

    public static void LateUpdate()
    {
        axisDirectionReleased.Clear();
    }

    public static Vector2 MousePosition(DrawMode drawMode = DrawMode.Relative)
    {
        Vector2 mouse = Vector2.Zero;
        if (drawMode == DrawMode.Relative) mouse = MousePositionRelative();
        if (drawMode == DrawMode.Absolute) mouse = MousePositionAbsolute();
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

    public static bool IsPressed(GamepadButton button, int index = -1)
    {
        if (index == -1)
        {
            for (int i = 0; i < 4; i++)
            {
                bool value = false;
                if (Raylib.IsGamepadAvailable(i) && Raylib.IsGamepadButtonPressed(i, (Raylib_cs.GamepadButton)button)) value = true;
                if (value)
                {
                    IsGamepadFocused = true;
                    return true;
                }
            }

            return false;
        }
        else
        {
            if (Raylib.IsGamepadAvailable(index))
            {
                bool value = Raylib.IsGamepadButtonPressed(index, (Raylib_cs.GamepadButton)button);
                if (value) IsGamepadFocused = true;
                return value;
            }
            else
            {
                return false;
            }
        }
    }

    public static bool IsHeld(GamepadButton button, int index = -1)
    {
        if (index == -1)
        {
            for (int i = 0; i < 4; i++)
            {
                bool value = false;
                if (Raylib.IsGamepadAvailable(i) && Raylib.IsGamepadButtonDown(i, (Raylib_cs.GamepadButton)button)) value = true;
                if (value)
                {
                    IsGamepadFocused = true;
                    return true;
                }
            }

            return false;
        }
        else
        {
            if (Raylib.IsGamepadAvailable(index))
            {
                bool value = Raylib.IsGamepadButtonDown(index, (Raylib_cs.GamepadButton)button);
                if (value) IsGamepadFocused = true;
                return value;
            }
            else
            {
                return false;
            }
        }
    }

    public static bool IsReleased(GamepadButton button, int index = -1)
    {
        if (index == -1)
        {
            for (int i = 0; i < 4; i++)
            {
                if (Raylib.IsGamepadAvailable(i) && Raylib.IsGamepadButtonReleased(i, (Raylib_cs.GamepadButton)button)) return true;
            }

            return false;
        }
        else
        {
            if (Raylib.IsGamepadAvailable(index)) return Raylib.IsGamepadButtonReleased(index, (Raylib_cs.GamepadButton)button);
            else return false;
        }
    }

    public static float Axis(GamepadAxis axis, int index = -1)
    {
        if (index == -1)
        {
            for (int i = 0; i < 4; i++)
            {
                if (Raylib.IsGamepadAvailable(i))
                {
                    float result = Raylib.GetGamepadAxisMovement(i, (Raylib_cs.GamepadAxis)axis);
                    if (result.Abs() < Deadzone) result = 0;
                    if (result != 0)
                    {
                        IsGamepadFocused = true;
                        return result;
                    }
                } 
            }

            return 0;
        }
        else
        {
            if (Raylib.IsGamepadAvailable(index))
            {
                float result = Raylib.GetGamepadAxisMovement(index, (Raylib_cs.GamepadAxis)axis);
                if (result.Abs() < Deadzone) result = 0;
                if (result != 0) IsGamepadFocused = true;
                return result;
            }

            return 0;
        }
    }

    public static Vector2 Daxis(GamepadDaxis axis, int index = -1)
    {
        Raylib_cs.GamepadAxis axisX = (Raylib_cs.GamepadAxis)GamepadAxis.LeftStickX;
        Raylib_cs.GamepadAxis axisY = (Raylib_cs.GamepadAxis)GamepadAxis.LeftStickY;

        if (axis == GamepadDaxis.LeftStick)
        {
            axisX = (Raylib_cs.GamepadAxis)GamepadAxis.LeftStickX;
            axisY = (Raylib_cs.GamepadAxis)GamepadAxis.LeftStickY;
        }
        if (axis == GamepadDaxis.RightStick)
        {
            axisX = (Raylib_cs.GamepadAxis)GamepadAxis.RightStickX;
            axisY = (Raylib_cs.GamepadAxis)GamepadAxis.RightStickY;
        }

        if (index == -1)
        {
            for (int i = 0; i < 4; i++)
            {
                if (Raylib.IsGamepadAvailable(i))
                {
                    Vector2 result = new Vector2(
                        Raylib.GetGamepadAxisMovement(i, axisX), 
                        Raylib.GetGamepadAxisMovement(i, axisY)
                    );
                    if (result.X.Abs() < Deadzone && result.Y.Abs() < Deadzone) result = Vector2.Zero;
                    if (result != Vector2.Zero)
                    {
                        IsGamepadFocused = true;
                        return result;
                    }
                } 
            }

            return Vector2.Zero;
        }
        else
        {
            if (Raylib.IsGamepadAvailable(index))
            {
                Vector2 result = new Vector2(
                    Raylib.GetGamepadAxisMovement(index, axisX),
                    Raylib.GetGamepadAxisMovement(index, axisY)
                );

                if (result.X.Abs() < Deadzone && result.Y.Abs() < Deadzone) result = Vector2.Zero;

                if (result != Vector2.Zero) IsGamepadFocused = true;

                return result;
            }
            else
            {
                return Vector2.Zero;
            } 
        }
    }

    static List<(GamepadAxisDirection Button, int Index)> axisDirectionHeld = new List<(GamepadAxisDirection Button, int Index)>() { };
    static List<(GamepadAxisDirection Button, int Index)> axisDirectionReleased = new List<(GamepadAxisDirection Button, int Index)>() { };

    static (GamepadAxis? Axis, int Direction) SplitGamePadAxisDirection(GamepadAxisDirection button)
    {
        GamepadAxis? axis = null;
        int direction = 0;

        if (button == GamepadAxisDirection.LeftStickLeft)
        {
            axis = GamepadAxis.LeftStickX;
            direction = -1;
        }
        if (button == GamepadAxisDirection.LeftStickRight)
        {
            axis = GamepadAxis.LeftStickX;
            direction = 1;
        }
        if (button == GamepadAxisDirection.LeftStickUp)
        {
            axis = GamepadAxis.LeftStickY;
            direction = -1;
        }
        if (button == GamepadAxisDirection.LeftStickDown)
        {
            axis = GamepadAxis.LeftStickY;
            direction = 1;
        }
        if (button == GamepadAxisDirection.RightStickLeft)
        {
            axis = GamepadAxis.RightStickX;
            direction = -1;
        }
        if (button == GamepadAxisDirection.RightStickRight)
        {
            axis = GamepadAxis.RightStickX;
            direction = 1;
        }
        if (button == GamepadAxisDirection.RightStickUp)
        {
            axis = GamepadAxis.RightStickY;
            direction = -1;
        }
        if (button == GamepadAxisDirection.RightStickDown)
        {
            axis = GamepadAxis.RightStickY;
            direction = 1;
        }

        return (axis, direction);
    }

    public static bool IsPressed(GamepadAxisDirection button, int index = -1)
    {
        var split = SplitGamePadAxisDirection(button);
        
        if (split.Axis == null) return false;
        if (axisDirectionHeld.Any(b => b.Button == button)) return false;

        if (index == -1)
        {
            for (int i = 0; i < 4; i++)
            {
                float value = 0;
                if (Raylib.IsGamepadAvailable(i))
                {
                    value = Axis(split.Axis.Value);
                    if (value.Sign() != split.Direction) value = 0;
                } 
                if (value != 0)
                {
                    axisDirectionHeld.Add((button, index));
                    IsGamepadFocused = true;
                    return true;
                }
            }

            return false;
        }
        else
        {
            if (Raylib.IsGamepadAvailable(index))
            {
                float value = Axis(split.Axis.Value);
                if (value.Sign() != split.Direction) value = 0;
                if (value != 0)
                {
                    axisDirectionHeld.Add((button, index));
                    IsGamepadFocused = true;
                }
                return value != 0;
            }
            else
            {
                return false;
            }
        }
    }

    public static bool IsHeld(GamepadAxisDirection button, int index = -1)
    {
        var split = SplitGamePadAxisDirection(button);
        
        if (split.Axis == null) return false;

        if (index == -1)
        {
            for (int i = 0; i < 4; i++)
            {
                float value = 0;
                if (Raylib.IsGamepadAvailable(i))
                {
                    value = Axis(split.Axis.Value);
                    if (value.Sign() != split.Direction) value = 0;
                } 
                if (value != 0)
                {
                    IsGamepadFocused = true;
                    return true;
                }
            }

            return false;
        }
        else
        {
            if (Raylib.IsGamepadAvailable(index))
            {
                float value = Axis(split.Axis.Value);
                if (value.Sign() != split.Direction) value = 0;
                if (value != 0)
                {
                    IsGamepadFocused = true;
                }
                return value != 0;
            }
            else
            {
                return false;
            }
        }
    }

    public static bool IsReleased(GamepadAxisDirection button, int index = -1)
    {
        return axisDirectionReleased.Any(b => b.Button == button && index == -1 || b.Index == index);
    }

    public static bool IsPressed(KeyboardKey key)
    {
        bool value = Raylib.IsKeyPressed((Raylib_cs.KeyboardKey)key);
        if (value) IsGamepadFocused = false;
        return value;
    }

    public static bool IsHeld(KeyboardKey key)
    {
        return Raylib.IsKeyDown((Raylib_cs.KeyboardKey)key);
    }

    public static bool IsReleased(KeyboardKey key)
    {
        return Raylib.IsKeyReleased((Raylib_cs.KeyboardKey)key);
    }

    public static bool IsPressed(MouseButton button)
    {
        bool value = Raylib.IsMouseButtonPressed((Raylib_cs.MouseButton)button);
        if (value) IsGamepadFocused = false;
        return value;
    }

    public static bool IsHeld(MouseButton button)
    {
        return Raylib.IsMouseButtonDown((Raylib_cs.MouseButton)button);
    }

    public static bool IsReleased(MouseButton button)
    {
        return Raylib.IsMouseButtonReleased((Raylib_cs.MouseButton)button);
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
        if (IsRepeating(currentKey)) keyboardKey = currentKey;
        string result = ShiftIsHeld
            ? KeyboardKeyStringShift[keyboardKey] 
            : KeyboardKeyString[keyboardKey];
        return result;
    }

    public static string ApplyKeyboardToString(string text)
    {
        text += GetKeyboardString();
        if (IsRepeating(KeyboardKey.Backspace) && text.Count() > 0)
        {
            if (AltIsHeld || CtrlIsHeld) text = "";
            else text = text.Remove(text.Count() - 1);
        }
        return text;
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
