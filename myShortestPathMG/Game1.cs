using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Xml.Linq;

namespace myShortestPathMG
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D _tileTexture;
        private List<Node> _nodes;
        private Node? _startNode;
        private Node? _endNode;
        private MouseState _previousMouseState;

        //Set tile properties
        private const int tileSize = 35;
        private const int rows = 20;
        private const int cols = 20;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = cols * tileSize;
            _graphics.PreferredBackBufferHeight = rows * tileSize;
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
            _tileTexture = Content.Load<Texture2D>("GrassTile");

            // Create a simple grid of nodes.
            _nodes = new List<Node>();
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    Rectangle bounds = new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize);
                    _nodes.Add(new Node(_tileTexture, bounds));
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            HandleMouseInput();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            if (_nodes != null)
            {
                    foreach (Node node in _nodes)
                    {
                        node.Draw(_spriteBatch);
                    }
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void HandleMouseInput()
        {
            MouseState mouseState = Mouse.GetState();
            KeyboardState keyboardState = Keyboard.GetState();

            if (mouseState.LeftButton == ButtonState.Pressed
                && _previousMouseState.LeftButton == ButtonState.Released)
            {
                Point position = mouseState.Position;
                foreach (Node node in _nodes)
                {
                    if (node.Bounds.Contains(position))
                    {
                        if (keyboardState.IsKeyDown(Keys.S) && _startNode == null)
                        {
                            ClearNode(node);
                            _startNode = node;
                            node.Type = NodeType.Start;
                        }
                        else if (keyboardState.IsKeyDown(Keys.E)
                                 && _endNode == null
                                 && node != _startNode)
                        {
                            ClearNode(node);
                            _endNode = node;
                            node.Type = NodeType.End;
                        }
                        else if (node == _startNode)
                        {
                            node.Type = NodeType.Normal;
                            _startNode = null;
                        }
                        else if (node == _endNode)
                        {
                            node.Type = NodeType.Normal;
                            _endNode = null;
                        }
                        break;
                    }
                }
            }

            _previousMouseState = mouseState;
        }


        private void ClearNode(Node node)
        {
            if (node == _startNode)
            {
                _startNode.Type = NodeType.Normal;
                _startNode = null;
            }
            if (node == _endNode)
            {
                _endNode.Type = NodeType.Normal;
                _endNode = null;
            }
        }
    }
}