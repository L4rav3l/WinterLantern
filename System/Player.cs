using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WinterLantern;

public class Player
{
    public Vector2 Position;
    public Vector2 screenPos;
    public float speed = 80f;

    public int Width = 16;
    public int Height = 16;

    public Rectangle Hitbox =>
        new Rectangle((int)(Position.X - Width / 2), (int)(Position.Y - Height / 2), Width, Height);

    public Player(Vector2 startPosition)
    {
        Position = startPosition;
    }

    public void Update(GameTime gameTime, List<Rectangle> solidTiles, Camera2D camera)
    {
        var kb = Keyboard.GetState();
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Vector2 oldPos = Position;

        if(GameData.Move)
        {
            Vector2 Movement = Vector2.Zero;

        if(kb.IsKeyDown(Keys.W)) Movement.Y--;
        if(kb.IsKeyDown(Keys.S)) Movement.Y++;
        if(kb.IsKeyDown(Keys.A)) Movement.X--;
        if(kb.IsKeyDown(Keys.D)) Movement.X++;

        if(Movement != Vector2.Zero)
        {
            Movement.Normalize();
        }

        Position += Movement * speed * dt;
        }

        foreach (var solid in solidTiles)
        {
            Rectangle hitbox = Hitbox;

            if (hitbox.Intersects(solid))
            {
                float dx = (hitbox.Center.X < solid.Center.X)
                    ? solid.Left - hitbox.Right
                    : solid.Right - hitbox.Left;

                float dy = (hitbox.Center.Y < solid.Center.Y)
                    ? solid.Top - hitbox.Bottom
                    : solid.Bottom - hitbox.Top;

                if (Math.Abs(dx) < Math.Abs(dy))
                    Position.X += dx;
                else
                    Position.Y += dy;
            }
        }

        camera.Position = Position;
    }

    public void Draw(SpriteBatch spriteBatch, Texture2D texture, Camera2D camera)
    {
        screenPos = camera.WorldToScreen(Position - new Vector2(Width / 2, Height / 2));
        spriteBatch.Draw(
            texture,
            screenPos,
            null,
            Color.White,
            0f,
            Vector2.Zero,
            5f,
            SpriteEffects.None,
            0.2f
        );
    }
}