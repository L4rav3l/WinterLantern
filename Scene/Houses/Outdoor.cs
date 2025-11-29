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

    private SpriteFont _pixelfont;

    private Texture2D _playerTexture;
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
    }

    public void Update(GameTime gameTime)
    {
        KeyboardState state = Keyboard.GetState();

        _player.Update(gameTime, _solidTiles, _camera);
        _camera.Follow(_player.Position, new Vector2(_map.Width * 16, _map.Height * 16));

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

        if(state.IsKeyDown(Keys.E) && !GameData.previous.IsKeyDown(Keys.E))
        {
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

        Vector2 TaskPositionM = _pixelfont.MeasureString(GameData.Task[GameData.TaskNumber]) * 0.75f;
        Vector2 TaskPosition = new Vector2((Width / 2) - (TaskPositionM.X / 2), 50);
        
        spriteBatch.DrawString(_pixelfont, $"Object: {GameData.Task[GameData.TaskNumber]}", TaskPosition, Color.Purple, 0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0.2f);

        _player.Draw(spriteBatch, _playerTexture, _camera);
    }
}