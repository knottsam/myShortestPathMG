using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace myShortestPathMG
{

    public enum NodeType
    {
        Normal,
        Start,
        End,
        Wall
    }

    public class Node : Tile
    {
        public NodeType Type { get; set; }
        public int gridX, gridY;

        public Node(Texture2D texture, Rectangle bounds) : base(texture, bounds)
        {
            Type = NodeType.Normal;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Color color = Type switch
            {
                NodeType.Start => Color.LawnGreen,
                NodeType.End => Color.PaleVioletRed,
                NodeType.Wall => Color.SlateGray,
                _ => Color.White
            };

            spriteBatch.Draw(Texture, Bounds, color);
        }
    }
}