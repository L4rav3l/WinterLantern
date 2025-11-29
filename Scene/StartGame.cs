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
        _sceneManager.ChangeScene("LucasHouse");
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        
    }
}