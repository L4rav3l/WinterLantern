using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using TiledSharp;
using System;
using System.IO;

namespace WinterLantern;

public class AbandonedHouse : IScene
{
    private GraphicsDevice _graphics;
    private SceneManager _sceneManager;
    private ContentManager _content;

    private Camera2D _camera;
    private Player _player;

    private TmxMap _map;

    private Texture2D _playerTexture;

    private SpriteFont _pixelfont;

    private List<Rectangle> _solidTile;
    private List<Rectangle> _lightTile;
    private List<Rectangle> _doorTile;

    private MapManager _mapmanager;

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

    public AbandonedHouse(GraphicsDevice _graphics, SceneManager _sceneManager, ContentManager _content)
    {
        this._graphics = _graphics;
        this._sceneManager = _sceneManager;
        this._content = _content;
    }

    public void LoadContent()
    {
        _pixelfont = _content.Load<SpriteFont>("pixelfont");
        _playerTexture = _content.Load<Texture2D>("player");

        _map = new TmxMap("Content/abandoned.tmx");

        var dict = new Dictionary<int, Texture2D>();

        foreach (var ts in _map.Tilesets)
        {
            string asset = Path.GetFileNameWithoutExtension(ts.Image.Source);
            dict[ts.FirstGid] = _content.Load<Texture2D>(asset);
        }

        _mapmanager = new MapManager(_map, dict);

        _solidTile = LoadListObject("Content/abandoned.tmx", "Collision");
        _lightTile = LoadListObject("Content/abandoned.tmx", "LightShard");
        _doorTile = LoadListObject("Content/abandoned.tmx", "Door");

        _player = new Player(new Vector2(896, 1072));
        _camera = new Camera2D(_graphics.Viewport);
    }

    public void Update(GameTime gameTime)
    {
        KeyboardState state = Keyboard.GetState();

        _player.Update(gameTime, _solidTile, _camera);
        _camera.Follow(_player.Position, new Vector2(_map.Width * 16, _map.Height * 16));

        if(GameData.TaskNumber == 11 && state.IsKeyDown(Keys.E) && !GameData.previous.IsKeyDown(Keys.E) && GameData.LightShard4)
        {
            foreach(Rectangle shard in _lightTile)
            {
                if(_player.Hitbox.Intersects(shard))
                {
                    GameData.LightShard4 = false;
                    GameData.TaskNumber++;
                }
            }
        }

        if(state.IsKeyDown(Keys.E) && !GameData.previous.IsKeyDown(Keys.E))
        {
            foreach(Rectangle door in _doorTile)
            {
                if(_player.Hitbox.Intersects(door))
                {
                    _sceneManager.ChangeScene("outdoor");
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
            }

            if(GameData.LightShard && GameData.LightShard4 && layer.Name == "LightShard")
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
