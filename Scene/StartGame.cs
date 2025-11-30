using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace WinterLantern;

public class StartGame : IScene
{
    private GraphicsDevice _graphics;
    private SceneManager _sceneManager;
    private ContentManager _content;

    public StartGame(GraphicsDevice _graphics, SceneManager _sceneManager, ContentManager _content)
    {
        this._graphics = _graphics;
        this._sceneManager = _sceneManager;
        this._content = _content;
    }   

    public void LoadContent()
    {

    }

    public void Update(GameTime gameTime)
    {
        _sceneManager.AddScene(new LucasHouse(_graphics, _sceneManager, _content), "LucasHouse");
        _sceneManager.AddScene(new Outdoor(_graphics, _sceneManager, _content), "outdoor");
        _sceneManager.AddScene(new Instructor(_graphics, _sceneManager, _content), "instructor");
        _sceneManager.ChangeScene("LucasHouse");

        GameData.Task = new string[9];

        GameData.Task[0] = "Talk your mum."; 
        GameData.Task[1] = "Check out the winter lantern at the Christmas market.";
        GameData.Task[2] = "Go back to the house and talk to your mom.";
        GameData.Task[3] = "Go to bed";
        GameData.Task[4] = "";
        GameData.Task[5] = "Ask your mum.";
        GameData.Task[6] = "Ask about it around the village.";
        GameData.Task[7] = "Explore the village.";
        GameData.Task[8] = "Talk to your mom about the light shard.";
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _graphics.Clear(Color.Black);
    }
}