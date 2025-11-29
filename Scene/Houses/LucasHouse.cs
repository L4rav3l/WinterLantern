using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using TiledSharp;
using System;
using System.IO;

namespace WinterLantern;

public class LucasHouse : IScene
{
    private GraphicsDevice _graphics;
    private SceneManager _sceneManager;
    private ContentManager _content;
    private MapManager _mapmanager;

    private Camera2D _camera;
    private Player _player;

    private TmxMap _map;

    private List<Rectangle> _solidTiles;
    private List<Rectangle> _momTiles;
    private List<Rectangle> _doorTiles;

    private string[] _momFirstSentences = new string[3];

    private bool _momSentencecWindow = false;
    private int _sentencesNumber = 0;

    private SpriteFont _pixelfont;
    
    private Texture2D _tileset;
    private Texture2D _playerTexture;
    private Texture2D _momSentencecWindowTexture;

    public List<Rectangle> LoadListObject(string path, string objectName)
    {
        TmxMap map = new TmxMap(path);
        var solidTiles = new List<Rectangle>();

        foreach(var objectGroups in map.ObjectGroups)
        {
            if(objectGroups.Name == objectName)
            {
                foreach(var obj in objectGroups.Objects)
                {
                    if(obj.Width > 0 && obj.Height > 0)
                    {
                        var rect = new Rectangle(
                            (int)obj.X,
                            (int)obj.Y,
                            (int)obj.Width,
                            (int)obj.Height
                        );

                        solidTiles.Add(rect);
                    }
                }
            }
        }

        return solidTiles;
    }

    public LucasHouse(GraphicsDevice _graphics, SceneManager _sceneManager, ContentManager _content)
    {
        this._graphics = _graphics;
        this._sceneManager = _sceneManager;
        this._content = _content;
    }

    public void LoadContent()
    {
        _map = new TmxMap("Content/lucas-house.tmx");
        _solidTiles = LoadListObject("Content/lucas-house.tmx", "Collision");
        _momTiles = LoadListObject("Content/lucas-house.tmx", "mom");
        _doorTiles = LoadListObject("Content/lucas-house.tmx", "Door");

        _playerTexture = _content.Load<Texture2D>("player");
        _pixelfont = _content.Load<SpriteFont>("pixelfont");
        
        _momSentencecWindowTexture = _content.Load<Texture2D>("sentenceswindow");

        var dict = new Dictionary<int, Texture2D>();

        foreach (var ts in _map.Tilesets)
        {
            string asset = Path.GetFileNameWithoutExtension(ts.Image.Source);
            dict[ts.FirstGid] = _content.Load<Texture2D>(asset);
        }

        _mapmanager = new MapManager(_map, dict);

        _player = new Player(new Vector2(807, 1126));
        _camera = new Camera2D(_graphics.Viewport);

        //mom first sentences

        _momFirstSentences[0] = "Mommy:\nHi sweetie. You woke up very\nlate. Today the winter lantern\nmight flare up. Can you check?\nIt's at the Christmas market.";
        _momFirstSentences[1] = "You:\nNow?";
        _momFirstSentences[2] = "Mommy:\nNo, only this afternoon.\nOf course it's now.";
    }

    public void Update(GameTime gameTime)
    {
        KeyboardState state = Keyboard.GetState();

        _player.Update(gameTime, _solidTiles, _camera);
        _camera.Follow(_player.Position, new Vector2(_map.Width * 16, _map.Height * 16));

        if(state.IsKeyDown(Keys.E) && !GameData.previous.IsKeyDown(Keys.E) && _sentencesNumber < 2 && _momSentencecWindow)
        {
            _sentencesNumber++;
        } else if(_sentencesNumber+1 == 3 && state.IsKeyDown(Keys.E) && !GameData.previous.IsKeyDown(Keys.E))
        {
            _momSentencecWindow = false;
            GameData.Move = true;
            _sentencesNumber = 0;
            GameData.TaskNumber++;
        }

        foreach(Rectangle door in _doorTiles)
        {
            if(_player.Hitbox.Intersects(door) && state.IsKeyDown(Keys.E) && !GameData.previous.IsKeyDown(Keys.E))
            {
                _sceneManager.ChangeScene("outdoor");
            }
        }

        foreach(Rectangle mom in _momTiles)
        {
            if(_player.Hitbox.Intersects(mom) && state.IsKeyDown(Keys.E) && !GameData.previous.IsKeyDown(Keys.E) && GameData.TaskNumber == 0)
            {
                _momSentencecWindow = true;
                GameData.Move = false;
            }
        }

        GameData.previous = state;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        int Width = _graphics.Viewport.Width;
        int Height = _graphics.Viewport.Height;

        _graphics.Clear(Color.Black);

        float layerDeep = 0.01f;

        foreach(var layer in _map.Layers)
        {
            if(layer.Visible)
            {
                _mapmanager.DrawLayer(layer, spriteBatch, layerDeep, _camera);
                layerDeep += 0.01f;
            }
        }

        if(_momSentencecWindow)
        {
            spriteBatch.Draw(_momSentencecWindowTexture, new Vector2(Width / 2 - 300, ((Height / 4) * 3) - 150), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.3f);
            
            
            if(GameData.TaskNumber == 0)
            {
                spriteBatch.DrawString(_pixelfont, _momFirstSentences[_sentencesNumber], new Vector2(Width / 2 - 280, ((Height / 4) * 3) - 140), Color.White, 0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0.4f);
            }

            if(GameData.TaskNumber == 2)
            {
                spriteBatch.DrawString(_pixelfont, _momFirstSentences[_sentencesNumber], new Vector2(Width / 2 - 280, ((Height / 4) * 3) - 140), Color.White, 0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0.4f);
            }

        }

        Vector2 TaskPositionM = _pixelfont.MeasureString(GameData.Task[GameData.TaskNumber]) * 0.75f;
        Vector2 TaskPosition = new Vector2((Width / 2) - (TaskPositionM.X / 2), 50);
        
        spriteBatch.DrawString(_pixelfont, $"Object: {GameData.Task[GameData.TaskNumber]}", TaskPosition, Color.Purple, 0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0.2f);

        _player.Draw(spriteBatch, _playerTexture, _camera);
    }
}