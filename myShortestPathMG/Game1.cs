using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace myShortestPathMG
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Placeholder texture name. Replace "tile" with your actual asset name
        private const string TileTextureName = "tile";

        private Texture2D _tileTexture;
        private List<Tile> _tiles;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load the tile texture. Ensure the asset is added to the Content
            // project with the same name as TileTextureName.
            _tileTexture = Content.Load<Texture2D>(TileTextureName);

            // Create a simple grid of tiles.
            const int tileSize = 64;
            const int rows = 5;
            const int cols = 5;
            _tiles = new List<Tile>();
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    var bounds = new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize);
                    _tiles.Add(new Tile(_tileTexture, bounds));
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            if (_tiles != null)
            {
                foreach (var tile in _tiles)
                {
                    tile.Draw(_spriteBatch);
                }
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
