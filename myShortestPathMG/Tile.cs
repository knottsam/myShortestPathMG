using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace myShortestPathMG
{
    public class Tile
    {
        public Texture2D Texture { get; }
        public Rectangle Bounds { get; }

        public Tile(Texture2D texture, Rectangle bounds)
        {
            Texture = texture;
            Bounds = bounds;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Bounds, Color.White);
        }
    }
}
