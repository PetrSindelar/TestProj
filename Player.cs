using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace AchtungDieKurve
{
    public class Player
    {
        public static Random Nahoda = new Random();
        public Texture2D PlayerTexture;
        public Vector2 PlayerTextureOrigin;
        public Texture2D LightTexture;
        public Vector2 LightTextureOrigin;

        public bool IsAtive = true;
        public float radius = 4;
        public Color Color;
        public List<Vector2> Positions = new List<Vector2>();
        public float Direction = 0;
        public float DirectionSpeed = 0.0025f;
        public float Speed = 1.3f / 16.666f;
        public float PartsDistance = 2;
        public Vector2 ActualPosition;
        #region Mezery
        public int SpaceParts = 0;
        public int SpacePartsMax = 20;
        public double SpaceTime = 1000; 
	#endregion
        public bool ShowLight = true;
        public Keys KeyLeft;
        public Keys KeyRight;
        public ScoreCounter Score;

        public Player(ContentManager Content, Color color, Vector2 startPosition, Keys keyLeft, Keys keyRight, Vector2 ScorePosition)
        {
            this.Color = color;
            ActualPosition = startPosition;
            Positions.Add(ActualPosition);
            PlayerTexture = Content.Load<Texture2D>("kolecko8");
            PlayerTextureOrigin = new Vector2(PlayerTexture.Width / 2, PlayerTexture.Height / 2);
            LightTexture = Content.Load<Texture2D>("Light");
            LightTextureOrigin = new Vector2(LightTexture.Width / 2, LightTexture.Height / 2);
            KeyLeft = keyLeft;
            KeyRight = keyRight;
            Score = new ScoreCounter(Content, ScorePosition);
        }
        public void Update(GameTime gameTime, KeyboardState keyState)
        {
            if (IsAtive)
            {
                if (keyState.GetPressedKeys().Contains(KeyLeft))
                {
                    this.Direction += DirectionSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                }
                if (keyState.GetPressedKeys().Contains(KeyRight))
                {

                    this.Direction -= DirectionSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                }
                Vector2 newPosition = ActualPosition + new Vector2((float)Math.Sin(Direction) * Speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds, (float)Math.Cos(Direction) * Speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds);
                float Distance = Vector2.Distance(newPosition, Positions[Positions.Count - 1]);
                if (SpaceParts <= 0)
                {
                    for (int i = (int)(Distance / PartsDistance); i > 0; i--)
                    {
                        Vector2 newPart = Positions[Positions.Count - 1] + new Vector2((float)Math.Sin(Direction) * PartsDistance, (float)Math.Cos(Direction) * PartsDistance);
                        Positions.Add(newPart);
                    }
                }
                else
                {
                    SpaceParts--;
                    if (SpaceParts == 0)
                    {
                        Positions.Add(ActualPosition);
                    }
                }
                ActualPosition = newPosition;
                SpaceTime -= gameTime.ElapsedGameTime.TotalMilliseconds;
                if (SpaceTime <= 0)
                {
                    SpaceParts = SpacePartsMax;
                    SpaceTime = 2000 + Nahoda.Next(0, 3000);
                } 
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (ShowLight&&IsAtive)
            {
                spriteBatch.Draw(LightTexture, ActualPosition, null, Color * 0.5f, 0f, LightTextureOrigin, 1.3f, SpriteEffects.None, 0f);
            }
            foreach (Vector2 position in Positions)
                spriteBatch.Draw(PlayerTexture, position, null, Color, 0f, PlayerTextureOrigin, (radius) / PlayerTextureOrigin.X, SpriteEffects.None, 0f);
            spriteBatch.Draw(PlayerTexture, ActualPosition, null, Color, 0f, PlayerTextureOrigin, (radius) / PlayerTextureOrigin.X, SpriteEffects.None, 0f);

        }
        public void Refresh(Vector2 startPosition, float direction)
        {
            if (SpaceParts <= 0)
            {
                this.Positions.Clear();
                this.ActualPosition = startPosition;
                this.Direction = direction;
                this.Positions.Add(ActualPosition);
            }
        }
        public bool CheckCollisionWith(Player player)
        {
            if (Vector2.Distance(this.ActualPosition, player.ActualPosition) <= this.radius + player.radius) //Kolize s hlavou
            {
                player.IsAtive = false;
                this.IsAtive = false;
                return true;
            }
            foreach (Vector2 position in player.Positions)
            {
                if (Vector2.Distance(position, this.ActualPosition) <= player.radius + this.radius) //this najel do playera
                {
                    this.IsAtive = false;
                    return true;
                }
            }
            return false;
        }
        public bool CheckRingCollision(Ring ring)
        {
            if (ring.Position.X - ring.Size.X / 2 >= this.ActualPosition.X - this.radius || ring.Position.X + ring.Size.X / 2 <= this.ActualPosition.X + this.radius ||
                ring.Position.Y - ring.Size.Y / 2 >= this.ActualPosition.Y - this.radius || ring.Position.X + ring.Size.Y / 2 <= this.ActualPosition.Y + this.radius)
            {
                this.IsAtive = false;
                return true;
            }
            return false;
        }
        public bool CheckSelfCollision()
        {
            for (int i = Positions.Count-1-20; i >= 0;i--)
            { 
                Vector2 position = Positions[i];
                if (Vector2.Distance(position, this.ActualPosition) <= radius * 2)
                {
                    IsAtive = false;
                    return true;
                }
            }
            return false;
        }
    }
}
