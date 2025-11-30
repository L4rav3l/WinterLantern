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
    private List<Rectangle> _bedTiles;
    private List<Rectangle> _lightTile;

    private string[] _momFirstSentences = new string[3];
    private string[] _momSecondSentences = new string[6];
    private string[] _momThirdSentences = new string[2];
    private string[] _momFourthSentences = new string[4];

    private bool _momSentencecWindow = false;
    private int _sentencesNumber = 0;

    private SpriteFont _pixelfont;
    
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
        _bedTiles = LoadListObject("Content/lucas-house.tmx", "Bed");
        _lightTile = LoadListObject("Content/lucas-house.tmx", "LightShard");

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

        //mom second sentences

        _momSecondSentences[0] = "Mommy:\nHello, you are so fast. So,\ndid the winter lantern flare\nup?";
        _momSecondSentences[1] = "You:\nHi mom, I have bad news.\nThe lantern didn't flare up.";
        _momSecondSentences[2] = "Mommy:\nWHAT? WHAT IS IT DOING?\nTHIS IS A BIG PROBLEM.";
        _momSecondSentences[3] = "Mommy:\nGO TO BED NOW.";
        _momSecondSentences[4] = "You:\nBut mom...";
        _momSecondSentences[5] = "Mommy:\nGO TO BED NOW.";

        //mom third sentences
        _momThirdSentences[0] = "You:\nMom, You know anything about\nWinter Lantern?";
        _momThirdSentences[1] = "Mommy:\nDont worry about it. Honey";

        //mom fourth sentences
        _momFourthSentences[0] = "You:\nHi mom, I found a light shard.\nDo you know anything about it?";
        _momFourthSentences[1] = "Mommy:\nWhat did you find? Is this real?";
        _momFourthSentences[2] = "You:\nYes, it is real.";
        _momFourthSentences[3] = "Mommy:\nRecently, for 30 years it hasn't\nflared up. Someone collected\nthe shard and the Lantern\nflared up again. You are chosen.";
    }

    public void Update(GameTime gameTime)
    {
        KeyboardState state = Keyboard.GetState();

        _player.Update(gameTime, _solidTiles, _camera);
        _camera.Follow(_player.Position, new Vector2(_map.Width * 16, _map.Height * 16));

        if(GameData.TaskNumber == 0)
        {
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
        } else if(GameData.TaskNumber == 2)
        {
            if(state.IsKeyDown(Keys.E) && !GameData.previous.IsKeyDown(Keys.E) && _sentencesNumber < 5 && _momSentencecWindow)
            {
                _sentencesNumber++;
            } else if(_sentencesNumber+1 == 6 && state.IsKeyDown(Keys.E) && !GameData.previous.IsKeyDown(Keys.E))
            {
                _momSentencecWindow = false;
                GameData.Move = true;
                _sentencesNumber = 0;
                GameData.TaskNumber++;
            }
        } else if(GameData.TaskNumber == 5)
        {
            if(state.IsKeyDown(Keys.E) && !GameData.previous.IsKeyDown(Keys.E) && _sentencesNumber < 1 && _momSentencecWindow)
            {
                _sentencesNumber++;
            } else if(_sentencesNumber+1 == 2 && state.IsKeyDown(Keys.E) && !GameData.previous.IsKeyDown(Keys.E))
            {
                _momSentencecWindow = false;
                GameData.Move = true;
                _sentencesNumber = 0;
                GameData.TaskNumber++;
            }
        } else if(GameData.TaskNumber == 8)
        {
            if(state.IsKeyDown(Keys.E) && !GameData.previous.IsKeyDown(Keys.E) && _sentencesNumber < 3 && _momSentencecWindow)
            {
                _sentencesNumber++;
            } else if(_sentencesNumber+1 == 4 && state.IsKeyDown(Keys.E) && !GameData.previous.IsKeyDown(Keys.E))
            {
                _momSentencecWindow = false;
                GameData.Move = true;
                _sentencesNumber = 0;
                GameData.TaskNumber++;
            }
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
            if(_player.Hitbox.Intersects(mom) && state.IsKeyDown(Keys.E) && !GameData.previous.IsKeyDown(Keys.E) && (GameData.TaskNumber == 0 || GameData.TaskNumber == 2 || GameData.TaskNumber == 5 || GameData.TaskNumber == 8))
            {
                _momSentencecWindow = true;
                GameData.Move = false;
            }
        }

        if((GameData.TaskNumber == 3 || GameData.TaskNumber == 9 || GameData.TaskNumber == 13) && state.IsKeyDown(Keys.E) && !GameData.previous.IsKeyDown(Keys.E))
        {
            foreach(Rectangle bed in _bedTiles)
            {
                if(_player.Hitbox.Intersects(bed))
                {
                    GameData.TaskNumber++;
                    _sceneManager.ChangeScene("instructor");
                }
            }
        }

        if(GameData.TaskNumber == 12 && GameData.LightShard5 && state.IsKeyDown(Keys.E) && !GameData.previous.IsKeyDown(Keys.E))
        {
            foreach(Rectangle light in _lightTile)
            {
                if(_player.Hitbox.Intersects(light))
                {
                    GameData.LightShard5 = false;
                    GameData.TaskNumber++;
                }
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
            if(layer.Visible && layer.Name != "LightShard")
            {
                _mapmanager.DrawLayer(layer, spriteBatch, layerDeep, _camera);
                layerDeep += 0.01f;
            } else if(layer.Name == "LightShard" && GameData.LightShard && GameData.LightShard5)
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
                spriteBatch.DrawString(_pixelfont, _momSecondSentences[_sentencesNumber], new Vector2(Width / 2 - 280, ((Height / 4) * 3) - 140), Color.White, 0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0.4f);
            }

            if(GameData.TaskNumber == 5)
            {
                spriteBatch.DrawString(_pixelfont, _momThirdSentences[_sentencesNumber], new Vector2(Width / 2 - 280, ((Height / 4) * 3) - 140), Color.White, 0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0.4f);
            }

            if(GameData.TaskNumber == 8)
            {
                spriteBatch.DrawString(_pixelfont, _momFourthSentences[_sentencesNumber], new Vector2(Width / 2 - 280, ((Height / 4) * 3) - 140), Color.White, 0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0.4f);
            }

        }

        Vector2 TaskPositionM = _pixelfont.MeasureString(GameData.Task[GameData.TaskNumber]) * 0.75f;
        Vector2 TaskPosition = new Vector2((Width / 2) - (TaskPositionM.X / 2), 50);
        
        spriteBatch.DrawString(_pixelfont, $"Object: {GameData.Task[GameData.TaskNumber]}", TaskPosition, Color.Purple, 0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0.2f);

        _player.Draw(spriteBatch, _playerTexture, _camera);
    }
}