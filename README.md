# Thingus

Everything is a Thing.

## Getting Started

1) Make new Project.
```
dotnet new console
```

2) Make your .csproj looks something like this.

```
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <!-- Packages -->
  <ItemGroup>
    <PackageReference Include="Raylib-cs" Version="6.1.1" />
    <PackageReference Include="Newtonsoft.Json" Version="13.0.1" />
  </ItemGroup>

  <!-- Fixes differences between cultures -->
  <PropertyGroup>
    <InvariantGlobalization>true</InvariantGlobalization>
  </PropertyGroup>

  <!-- Icons -->
  <PropertyGroup>
    <ApplicationIcon>Icon.ico</ApplicationIcon>
  </PropertyGroup>

  <ItemGroup>
    <None Remove="Icon.ico" />
    <None Remove="Icon.bmp" />
  </ItemGroup>

  <ItemGroup>
    <EmbeddedResource Include="Icon.ico" />
    <EmbeddedResource Include="Icon.bmp" />
  </ItemGroup>

</Project>
```

3) Get Thingus.
```
git submodule add https://github.com/ZachIsAGardner/Thingus.git _Thingus
```

4) Make your Program.cs look something like this.
```
using Raylib_cs;
using Thingus;

namespace MyCoolGame;

class Program
{
    public static void Main()
    {
        Game.Process();
    }
}
```

5) Create a CONSTANTS.json file.
```
{
    "NAMESPACE": "MyCoolGame",
    "START_ROOM": "Test",
    "VIRTUAL_WIDTH": 320,
    "VIRTUAL_HEIGHT": 180,
    "DEFAULT_SCREEN_MULTIPLIER": 3,
    "TILE_SIZE": 16
}
```