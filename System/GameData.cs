using Microsoft.Xna.Framework.Input;

namespace WinterLantern;

public static class GameData
{
    public static KeyboardState previous {get;set;}
    public static bool Move = true;
    public static bool Quit {get;set;}
}