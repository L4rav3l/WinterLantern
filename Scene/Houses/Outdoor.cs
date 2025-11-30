using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using TiledSharp;
using System;
using System.Collections.Generic;
using System.IO;

namespace WinterLantern;

public class Outdoor : IScene
{
    private GraphicsDevice _graphics;
    private SceneManager _sceneManager;
    private ContentManager _content;
    private MapManager _mapmanager;

    private TmxMap _map;

    private List<Rectangle> _solidTiles;
    private List<Rectangle> _lucasTiles;
    private List<Rectangle> _winterTiles;
    private List<Rectangle> _npcTiles;

    private List<Rectangle> _lightTile1;
    private List<Rectangle> _lightTile2;
    private List<Rectangle> _lightTile3;

    private SpriteFont _pixelfont;

    private Texture2D _playerTexture;
    private Texture2D _sentencesWindowTexture;

    private string[] _npcSentences = new string[6];
    private int _sentencesNumber;
    private bool _sentencesWindow = false;

    private Camera2D _camera;
    private Player _player;

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

    public Outdoor(GraphicsDevice _graphics, SceneManager _sceneManager, ContentManager _content)
    {
        this._graphics = _graphics;
        this._sceneManager = _sceneManager;
        this._content = _content;
    }

    public void LoadContent()
    {
        _map = new TmxMap("Content/outdoor.tmx");

        _camera = new Camera2D(_graphics.Viewport);
        _player = new Player(new Vector2(1184, 624));

        _pixelfont = _content.Load<SpriteFont>("pixelfont");

        _playerTexture = _content.Load<Texture2D>("player");
        _sentencesWindowTexture = _content.Load<Texture2D>("sentenceswindow");

        var dict = new Dictionary<int, Texture2D>();

        foreach (var ts in _map.Tilesets)
        {
            string asset = Path.GetFileNameWithoutExtension(ts.Image.Source);
            dict[ts.FirstGid] = _content.Load<Texture2D>(asset);
        }

        _mapmanager = new MapManager(_map, dict);
        _solidTiles = LoadListObject("Content/outdoor.tmx", "Collision");
        _lucasTiles = LoadListObject("Content/outdoor.tmx", "LucasHome");
        _winterTiles = LoadListObject("Content/outdoor.tmx", "WinterLantern");
        _npcTiles = LoadListObject("Content/outdoor.tmx", "NPC_INT");

        _lightTile1 = LoadListObject("Content/outdoor.tmx", "LightShard1");
        _lightTile2 = LoadListObject("Content/outdoor.tmx", "LightShard2");
        _lightTile3 = LoadListObject("Content/outdoor.tmx", "LightShard3");

        //first npc sentences

        _npcSentences[0] = "You:\nHello, can you help me?";
        _npcSentences[1] = "Shopper:\nYeah, where can I help?";
        _npcSentences[2] = "You:\nWhat you know about the\nWinterLantern?";
        _npcSentences[3] = "Shopper:\nThe Winter Lantern controls\nthe temperature. If it doesn't\nflare up, the temperature\ngoes down. The last time this\nhappened was 30 years ago.";
        _npcSentences[4] = "Shopper:\nThen a man had to collect a lot\nof shards,and the Winter\nLantern flared up again.";
        _npcSentences[5] = "You:\nThank you for your help.";
    }

    public void Update(GameTime gameTime)
    {
        KeyboardState state = Keyboard.GetState();

        _player.Update(gameTime, _solidTiles, _camera);
        _camera.Follow(_player.Position, new Vector2(_map.Width * 16, _map.Height * 16));

        if(GameData.TaskNumber == 6)
        {
            if(state.IsKeyDown(Keys.E) && !GameData.previous.IsKeyDown(Keys.E) && _sentencesNumber < 5 && _sentencesWindow)
            {
                _sentencesNumber++;
            } else if(_sentencesNumber+1 == 6 && state.IsKeyDown(Keys.E) && !GameData.previous.IsKeyDown(Keys.E))
            {
                _sentencesWindow = false;
                GameData.Move = true;
                _sentencesNumber = 0;
                GameData.TaskNumber++;
            }
        }

        if(state.IsKeyDown(Keys.E) && !GameData.previous.IsKeyDown(Keys.E))
        {
            foreach(Rectangle door in _lucasTiles)
            {
                if(_player.Hitbox.Intersects(door))
                {
                    _sceneManager.ChangeScene("LucasHouse");
                }
            }
        }

        foreach(Rectangle tile in _winterTiles)
        {
            if(_player.Hitbox.Intersects(tile))
            {
                if(GameData.TaskNumber == 1)
                {
                    GameData.TaskNumber++;
                }
            }
        }

        if(state.IsKeyDown(Keys.E) && !GameData.previous.IsKeyDown(Keys.E))
        {
            foreach(Rectangle npc in _npcTiles)
            {
                if(_player.Hitbox.Intersects(npc))
                {
                    if(GameData.TaskNumber == 6)
                    {
                        GameData.Move = false;
                        _sentencesWindow = true;
                    }
                }
            }
        }

        if(state.IsKeyDown(Keys.E) && !GameData.previous.IsKeyDown(Keys.E))
        {
            foreach(Rectangle tile in _lightTile1)
            {
                if(_player.Hitbox.Intersects(tile))
                {
                    if(GameData.TaskNumber == 7)
                    {
                        GameData.LightShard1 = false;
                    }
                }
            }

            foreach(Rectangle tile in _lightTile2)
            {
                if(_player.Hitbox.Intersects(tile))
                {
                    if(GameData.TaskNumber == 7)
                    {
                        GameData.LightShard2 = false;
                    }
                }
            }

            foreach(Rectangle tile in _lightTile3)
            {
                if(_player.Hitbox.Intersects(tile))
                {
                    if(GameData.TaskNumber == 7)
                    {
                        GameData.LightShard3 = false;
                    }
                }
            }
        }

        if(GameData.LightShard1 == false && GameData.LightShard2 == false && GameData.LightShard3 == false && GameData.TaskNumber == 7)
        {
            GameData.TaskNumber++;
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
            if(layer.Visible && layer.Name != "LightShard1" && layer.Name != "LightShard2" && layer.Name != "LightShard3")
            {
                _mapmanager.DrawLayer(layer, spriteBatch, layerDeep, _camera);
                layerDeep += 0.01f;
            }

            if(GameData.LightShard && GameData.LightShard1 && layer.Name == "LightShard1")
            {
                _mapmanager.DrawLayer(layer, spriteBatch, layerDeep, _camera);
                layerDeep += 0.01f;
            }

            if(GameData.LightShard && GameData.LightShard2 && layer.Name == "LightShard2")
            {
                _mapmanager.DrawLayer(layer, spriteBatch, layerDeep, _camera);
                layerDeep += 0.01f;
            }

            if(GameData.LightShard && GameData.LightShard3 && layer.Name == "LightShard3")
            {
                _mapmanager.DrawLayer(layer, spriteBatch, layerDeep, _camera);
                layerDeep += 0.01f;
            }
        }

        if(_sentencesWindow)
        {
            spriteBatch.Draw(_sentencesWindowTexture, new Vector2(Width / 2 - 300, ((Height / 4) * 3) - 150), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.3f);

            if(GameData.TaskNumber == 6)
            {
                spriteBatch.DrawString(_pixelfont, _npcSentences[_sentencesNumber], new Vector2(Width / 2 - 280, ((Height / 4) * 3) - 140), Color.White, 0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0.4f);
            }
        }

        Vector2 TaskPositionM = _pixelfont.MeasureString(GameData.Task[GameData.TaskNumber]) * 0.75f;
        Vector2 TaskPosition = new Vector2((Width / 2) - (TaskPositionM.X / 2), 50);
        
        spriteBatch.DrawString(_pixelfont, $"Object: {GameData.Task[GameData.TaskNumber]}", TaskPosition, Color.Purple, 0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0.2f);

        _player.Draw(spriteBatch, _playerTexture, _camera);
    }
}