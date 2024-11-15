using System.Numerics;
using Raylib_cs;

namespace Thingus;

public class ShaderValue
{
    public string Name;
    public ShaderUniformDataType Type;
    public Func<object> Get;

    public ShaderValue(string name, ShaderUniformDataType type, Func<object> get)
    {
        Name = name;
        Type = type;
        Get = get;
    }

    public void Apply(Raylib_cs.Shader shader)
    {
        if (Type == ShaderUniformDataType.Float)
        {
            Raylib.SetShaderValue(
                shader, 
                Raylib.GetShaderLocation(shader, Name), 
                (float)Get(), 
                Type
            );
        }
        else if (Type == ShaderUniformDataType.Float)
        {
            Raylib.SetShaderValue(
                shader, 
                Raylib.GetShaderLocation(shader, Name), 
                (float)Get(), 
                Type
            );
        }
        else if (Type == ShaderUniformDataType.Float)
        {
            Raylib.SetShaderValue(
                shader,
                Raylib.GetShaderLocation(shader, Name),
                (float)Get(),
                Type
            );
        }
        else if (Type == ShaderUniformDataType.Vec2)
        {
            Raylib.SetShaderValue(
                shader,
                Raylib.GetShaderLocation(shader, Name),
                (Vector2)Get(),
                Type
            );
        }
        else if (Type == ShaderUniformDataType.Vec3)
        {
            Raylib.SetShaderValue(
                shader,
                Raylib.GetShaderLocation(shader, Name),
                (Vector3)Get(),
                Type
            );
        }
        else if (Type == ShaderUniformDataType.Vec4)
        {
            Raylib.SetShaderValue(
                shader,
                Raylib.GetShaderLocation(shader, Name),
                (Vector4)Get(),
                Type
            );
        }
        else if (Type == ShaderUniformDataType.Int)
        {
            Raylib.SetShaderValue(
                shader,
                Raylib.GetShaderLocation(shader, Name),
                (int)Get(),
                Type
            );
        }
        else if (Type == ShaderUniformDataType.Sampler2D)
        {

            Raylib.SetShaderValueTexture(
                shader, 
                Raylib.GetShaderLocation(shader, Name), 
                (Texture2D)(Get())
            );
        }
    }
}

public class Shader
{
    public List<ShaderValue> Values = new List<ShaderValue>() { };

    Raylib_cs.Shader shader;

    public Shader(Raylib_cs.Shader shader)
    {
        this.shader = shader;
    }

    public void Update()
    {
        Values.ForEach(v => v.Apply(shader));
    }

    public Raylib_cs.Shader ToRaylib()
    {
        return shader;
    }
}