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
    
    private Texture2D _tileset;
    private Texture2D _playerTexture;

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

        var dict = new Dictionary<int, Texture2D>();

        foreach (var ts in _map.Tilesets)
        {
            string asset = Path.GetFileNameWithoutExtension(ts.Image.Source);
            dict[ts.FirstGid] = _content.Load<Texture2D>(asset);
        }

        _mapmanager = new MapManager(_map, dict);

        _player = new Player(new Vector2(807, 1126));
        _camera = new Camera2D(_graphics.Viewport);
    }

    public void Update(GameTime gameTime)
    {
        _player.Update(gameTime, _solidTiles, _camera);
        _camera.Follow(_player.Position, new Vector2(_map.Width * 16, _map.Height * 16));

        Console.WriteLine(_player.Position);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
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

        _player.Draw(spriteBatch, _playerTexture, _camera);
    }
}