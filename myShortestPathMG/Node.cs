using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace myShortestPathMG
{
    public enum NodeType
    {
        Normal,
        Start,
        End
    }

    public class Node : Tile
    {
        public NodeType Type { get; set; }

        public Node(Texture2D texture, Rectangle bounds) : base(texture, bounds)
        {
            Type = NodeType.Normal;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            int alpha = 160;

            Color color = Type switch
            {
                NodeType.Start => Color.YellowGreen,
                NodeType.End => Color.Red,
                _ => new Color(Color.White.R, Color.White.G, Color.White.B, alpha)
            };

            spriteBatch.Draw(Texture, Bounds, color);
        }
    }
}