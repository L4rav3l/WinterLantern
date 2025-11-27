using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System;

namespace WinterLantern;

public class Menu : IScene
{
    private GraphicsDevice _graphics;
    private SceneManager _sceneManager;
    private ContentManager _content;
    
    private SpriteFont _pixelfont;

    private int _Selected;
    private bool _blinking = false;
    private double _cooldown;

    public Menu(GraphicsDevice _graphics, SceneManager _sceneManager, ContentManager _content)
    {
        this._graphics = _graphics;
        this._sceneManager = _sceneManager;
        this._content = _content;
    }

    public void LoadContent()
    {
        _pixelfont = _content.Load<SpriteFont>("pixelfont");
    }

    public void Update(GameTime gameTime)
    {
        KeyboardState state = Keyboard.GetState();
        float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds * 1000;

        if(_cooldown >= 0)
        {
            _cooldown -= elapsed;
        } else {
            _blinking = !_blinking;
            _cooldown = 300;
        }

        if((state.IsKeyDown(Keys.Up) && !GameData.previous.IsKeyDown(Keys.Up)) || (state.IsKeyDown(Keys.Down) && !GameData.previous.IsKeyDown(Keys.Down)))
        {
            if(_Selected == 0)
            {
                _Selected = 1;
            } else {
                _Selected = 0;
            }
        }

        if(state.IsKeyDown(Keys.Enter) && !GameData.previous.IsKeyDown(Keys.Enter))
        {
            if(_Selected == 0)
            {
                _sceneManager.ChangeScene("startgame");
            } else {
                GameData.Quit = true;
            }
        }

        GameData.previous = state;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _graphics.Clear(Color.Purple);

        int Width = _graphics.Viewport.Width;
        int Height = _graphics.Viewport.Height;

        string playText = "";
        string quitText = "";

        if(_Selected == 0)
        {
            if(_blinking == true)
            {
                playText = "> Play <";
            } else {
                playText = "Play";
            }

            quitText = "Quit";

        } else {
            if(_blinking == true)
            {
                quitText = "> Quit <";
            } else {
                quitText = "Quit";
            }

            playText = "Play";
        }
        
        Vector2 LabelM = _pixelfont.MeasureString("Winter Lantern");
        Vector2 Label = new Vector2((Width / 2) - (LabelM.X / 2), (Height / 4) - (LabelM.Y / 2));

        spriteBatch.DrawString(_pixelfont, "Winter Lantern", Label, Color.White);

        Vector2 PlayM = _pixelfont.MeasureString(playText);
        Vector2 Play = new Vector2((Width / 2) - (PlayM.X / 2), (Height / 4) - (PlayM.Y / 2) + 100);

        spriteBatch.DrawString(_pixelfont, playText, Play, Color.White);

        Vector2 QuitM = _pixelfont.MeasureString(quitText);
        Vector2 Quit = new Vector2((Width / 2) - (QuitM.X / 2), (Height / 4) - (QuitM.Y / 2) + 150);

        spriteBatch.DrawString(_pixelfont, quitText, Quit, Color.White); 
    }
}