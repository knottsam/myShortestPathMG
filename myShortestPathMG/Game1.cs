using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.MediaFoundation;
using System.Collections.Generic;

namespace myShortestPathMG
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D _tileTexture, _panelTexture, _buttonTexture;
        private SpriteFont _myFont;
        private Rectangle _buttonRectangle;
        private List<Node> _nodes;
        private Node _startNode;
        private Node _endNode;
        private Node _wallNode;
        private MouseState _previousMouseState;
        private const int _panelHeight = 60;

        //Set tile properties
        private const int tileSize = 50;
        private const int rows = 12;
        private const int cols = 22;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = cols * tileSize;
            _graphics.PreferredBackBufferHeight = rows * tileSize + _panelHeight;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _panelTexture = new Texture2D(GraphicsDevice, 1, 1);
            _buttonTexture = new Texture2D(GraphicsDevice, 1, 1);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load the tile texture. Ensure the asset is added to the Content
            _tileTexture = Content.Load<Texture2D>("Oak");
            _myFont = Content.Load<SpriteFont>("MyFont");
            _panelTexture.SetData(new[] { Color.DarkGray });
            _buttonTexture.SetData(new[] { Color.LightGray });

            // Create a simple grid of nodes.
            _nodes = new List<Node>();
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    Rectangle bounds = new Rectangle(x * tileSize, y * tileSize + _panelHeight, tileSize, tileSize);
                    Node node = new Node(_tileTexture, bounds)
                    {
                        gridX = x,
                        gridY = y
                    };
                    _nodes.Add(node);
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
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();
            //Draw the panel
            _spriteBatch.Draw(_panelTexture,
                new Rectangle(0, 0, cols * tileSize, _panelHeight),
                Color.DarkGray);

            //Draw the button
            _buttonRectangle = new Rectangle(10, 10, 130, 40);
            _spriteBatch.Draw(_buttonTexture, _buttonRectangle, Color.LightGray);
            _spriteBatch.DrawString(_myFont, "Find Path", new Vector2(30, 20), Color.Black);

            //Draw the grid
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

        private List<Node> GetNeigbours(Node node)
        {
            (int dX, int dY)[] offsets = new (int, int)[]
            {
                (-1, 0),  // Left
                (1, 0),   // Right
                (0, -1),  // Up
                (0, 1),   // Down
                (-1, -1), // Top-left
                (1, -1),  // Top-right
                (-1, 1),  // Bottom-left
                (1, 1)    // Bottom-right
            };

            List<Node> neighbours = new List<Node>();

            foreach ((int dX, int dY) in offsets)
            { 
                int neighbourX = node.gridX + dX;
                int neighbourY = node.gridY + dY;

                if (neighbourX >= 0 && neighbourY < cols
                    && neighbourY >= 0 && neighbourY < rows)
                {
                    Node neighbour = _nodes[neighbourY * cols + neighbourX];
                    if (neighbour.Type != NodeType.Wall)
                    {
                        neighbours.Add(neighbour);
                    }
                }
            }
            
            return neighbours; // Placeholder for future implementation
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
                        else if (keyboardState.IsKeyDown(Keys.W)
                                && node != _endNode
                                && node != _startNode)
                        {
                            if (node.Type == NodeType.Wall)
                            {
                                node.Type = NodeType.Normal;
                                if (_wallNode == node)
                                    _wallNode = null;
                            }
                            else
                            {
                                ClearNode(node);
                                _wallNode = node;
                                node.Type = NodeType.Wall;
                            }
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
                        else if (node.Type != NodeType.Normal)
                        {
                            if (node == _startNode)
                                _startNode = null;
                            if (node == _endNode)
                                _endNode = null;
                            if (node == _wallNode)
                                _wallNode = null;
                            node.Type = NodeType.Normal;
                        }
                        break;
                    }
                }
            }
            if(_buttonRectangle.Contains(mouseState.Position))
            {
                if (mouseState.LeftButton == ButtonState.Pressed
                    && _previousMouseState.LeftButton == ButtonState.Released)
                {
                    // For now, just print a message to the console
                    System.Diagnostics.Debug.WriteLine("Pathfinding button clicked!");
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