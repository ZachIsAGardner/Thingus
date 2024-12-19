using System;
using Raylib_cs;

namespace Thingus;

public static class Time
{
    public static float Delta = 1f;

    public static float ModifiedDelta = 1f;
    public static float Modifier = 1f;

    public static float Elapsed = 0;
    public static int Frame = 0;

    public static void Update()
    {
        Delta = Raylib.GetFrameTime();
        ModifiedDelta = Delta * Modifier;

        Elapsed += Delta;

        Frame++;
    }
}
