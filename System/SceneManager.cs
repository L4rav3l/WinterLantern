using System.Collections.Generic;

namespace WinterLantern;

public class SceneManager
{
    public List<IScene> scenes;
    public IScene CurrentScene;

    public SceneManager()
    {
        scenes = new();
    }

    public void AddScene(IScene scene, string name)
    {
        scene.LoadScene();

        scenes[name] = scene;
    }

    public void ChangeScene(string name)
    {
        CurrentScene = scenes[name];
    }

    public IScene GetCurrentScene()
    {
        return CurrentScene;
    }
}