using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace WinterLantern;

public class Instructor : IScene
{
    private GraphicsDevice _graphics;
    private SceneManager _sceneManager;
    private ContentManager _content;

    private SpriteFont _pixelfont;

    public Instructor(GraphicsDevice _graphics, SceneManager _sceneManager, ContentManager _content)
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

        if(state.IsKeyDown(Keys.E) && !GameData.previous.IsKeyDown(Keys.E))
        {
            if(GameData.TaskNumber < 14)
            {
                GameData.TaskNumber++;
                GameData.LightShard = true;
                _sceneManager.ChangeScene("LucasHouse");
            } else {
                _sceneManager.ChangeScene("menu");
            }
        }

        GameData.previous = state;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _graphics.Clear(Color.Black);

        int Width = _graphics.Viewport.Width;
        int Height = _graphics.Viewport.Height;

        string instructorText = "";

        if(GameData.TaskNumber == 4)
        {
            instructorText = "Tomorrow you will ask around the village to find out what people know about it.";
        }

        if(GameData.TaskNumber == 10)
        {
            instructorText = "Tomorrow you will explore the abandoned house.";
        }

        if(GameData.TaskNumber == 14)
        {
            instructorText = "You were saved the world from cold because\nyou collected the all winter lantern shard aroud the village.\nCongratulation";
        }

        Vector2 InstructorM = _pixelfont.MeasureString(instructorText);
        Vector2 Instructor = new Vector2((Width / 2) - (InstructorM.X / 2), (Height / 2) - (InstructorM.Y / 2));

        spriteBatch.DrawString(_pixelfont, instructorText, Instructor, Color.White);
    }
}