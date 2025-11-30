using Microsoft.Xna.Framework.Input;

namespace WinterLantern;

public static class GameData
{
    public static KeyboardState previous {get;set;}
    public static bool Move = true;
    public static bool Quit {get;set;}
    public static string[] Task {get;set;}
    public static int TaskNumber = 0;
    public static bool LightShard = false;
    public static bool LightShard1 = true;
    public static bool LightShard2 = true;
    public static bool LightShard3 = true;
    public static bool LightShard4 = true;
    public static bool LightShard5 = true;
}