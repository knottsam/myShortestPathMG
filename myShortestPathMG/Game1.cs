using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Drawing.Text;

namespace myShortestPathMG
{

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _tileTexture, _panelTexture, _buttonTexture;
        private SpriteFont _myFont;
        private Rectangle _findButtonRectangle, _clearButtonRectangle;
        private List<Node> _nodes;
        private Node _startNode;
        private Node _endNode;
        private Node _wallNode;
        private PathFinder _pathFinder;
        private bool _isPathFinding = false;
        private MouseState _previousMouseState;
        private PathFindingAlgorithm _algorithm = PathFindingAlgorithm.Dijkstra;


        //Set tile properties
        private const int TILE_SIZE = 50;
        private const int ROWS = 12;
        private const int COLS = 22;
        private const int BUTTON_HEIGHT = 40;
        private const int BUTTON_WIDTH = 130;
        private const int PANEL_WIDTH = 150;

        //For selection of pathfinding algorithm
        private Rectangle _dijkstraButtonRectangle;
        private Rectangle _astarButtonRectangle;
        private Rectangle _manhattanButtonRectangle;
        private Rectangle _euclideanButtonRectangle;
        private HeuristicType _heuristic = HeuristicType.Manhattan;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = COLS * TILE_SIZE + PANEL_WIDTH;
            _graphics.PreferredBackBufferHeight = ROWS * TILE_SIZE;
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

            // Create a grid of nodes.
            _nodes = new List<Node>();
            for (int y = 0; y < ROWS; y++)
            {
                for (int x = 0; x < COLS; x++)
                {
                    Rectangle bounds = new Rectangle(x * TILE_SIZE + PANEL_WIDTH,
                        y * TILE_SIZE,
                        TILE_SIZE, TILE_SIZE);
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

            HandleMouseInput();//Handle the mouse

            // Check if pathfinding is in progress and step through the algorithm
            if (_isPathFinding && _pathFinder != null && !_pathFinder.Finished)
            {
                //Check which of the algorithms has been selected
                if (_algorithm == PathFindingAlgorithm.AStar)
                {
                    _pathFinder.StepAStar();//Step through A* algorithm
                }
                else if (_algorithm == PathFindingAlgorithm.Dijkstra)
                {
                    _pathFinder.StepDijkstra();// Step through Dijkstra's algorithm
                }
            }
            // If pathfinding is finished, draw the path
            if (_isPathFinding && _pathFinder != null && _pathFinder.Finished)
            {
                // Draw the path
                List<Node> path = _pathFinder.GetPath(_startNode, _endNode);
                foreach (Node n in path)
                {
                    if (n != _startNode && n != _endNode)
                    {
                        n.Type = NodeType.Path; // Mark the path nodes
                    }
                }
                _isPathFinding = false; // Stop path finding after the path is drawn

            }

            base.Update(gameTime);
        }

        public void HandleMouseInput()
        {
            MouseState mouseState = Mouse.GetState();
            KeyboardState keyboardState = Keyboard.GetState();

            Point position = mouseState.Position;// Get the current mouse position

            // Drag to create/remove walls while holding W and left mouse button
            if (keyboardState.IsKeyDown(Keys.W) && mouseState.LeftButton == ButtonState.Pressed)
            {
                foreach (Node node in _nodes)
                {
                    // Check if the mouse is within the bounds of a node
                    if (node.Bounds.Contains(position) && node != _startNode && node != _endNode)
                    {
                        // Toggle wall on drag: left click adds wall, right click removes wall
                        if (node.Type != NodeType.Wall)
                        {
                            ClearNode(node);//Clear any existing start or end node
                            node.Type = NodeType.Wall;
                        }
                        break;
                    }
                }
            }
            // Single click handling for start, end, wall, and clearing nodes
            else if (mouseState.LeftButton == ButtonState.Pressed
                && _previousMouseState.LeftButton == ButtonState.Released)
            {
                if (_dijkstraButtonRectangle.Contains(position))
                {
                    _algorithm = PathFindingAlgorithm.Dijkstra; // Set algorithm to Dijkstra
                }
                else if (_astarButtonRectangle.Contains(position))
                {
                    _algorithm = PathFindingAlgorithm.AStar; // Set algorithm to A*
                }
                if (_algorithm == PathFindingAlgorithm.AStar)
                {
                    if (_manhattanButtonRectangle.Contains(position))
                    { 
                        _heuristic = HeuristicType.Manhattan; // Set heuristic to Manhattan
                    }
                    else if (_euclideanButtonRectangle.Contains(position))
                    {
                        _heuristic = HeuristicType.Euclidean; // Set heuristic to Euclidean
                    }
                }


                    // Check if the mouse is within the bounds of a node - checks all nodes
                    foreach (Node node in _nodes)
                    {
                        if (node.Bounds.Contains(position))
                        {
                            // If the node is clicked, handle the type changes
                            if (keyboardState.IsKeyDown(Keys.S) && _startNode == null)
                            {
                                ClearNode(node);// Clear any existing start or end node
                                _startNode = node;// Set the clicked node as the start node
                                node.Type = NodeType.Start;// Change the node type to Start
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
                                }
                                else
                                {
                                    ClearNode(node);
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
                                node.Type = NodeType.Normal;
                            }
                            break;
                        }
                    }
            }

            // Handle button click for pathfinding
            if (_findButtonRectangle.Contains(position)
                && _startNode != null && _endNode != null)
            {
                if (mouseState.LeftButton == ButtonState.Pressed
                    && _previousMouseState.LeftButton == ButtonState.Released)
                {
                    foreach (Node n in _nodes)
                    { 
                        if(n.Type == NodeType.Path || n.Type == NodeType.Inspected)
                        {
                            n.Type = NodeType.Normal; // Reset path nodes to normal before starting new pathfinding
                        }
                    }
                    // Start pathfinding if the start and end nodes are set
                    _pathFinder = new PathFinder(_nodes, ROWS, COLS);

                    if (_algorithm == PathFindingAlgorithm.AStar)
                    {
                        // Start A* pathfinding with the selected heuristic
                        _pathFinder.StartAStar(_startNode, _endNode, _heuristic);
                    }
                    else if (_algorithm == PathFindingAlgorithm.Dijkstra)
                    {
                        // Start Dijkstra's pathfinding
                        _pathFinder.StartDijkstra(_startNode, _endNode);
                    }
                    _isPathFinding = true;// Set the flag to indicate pathfinding is in progress
                }
            }

            //Handle button click for clearing nodes
            if (_clearButtonRectangle.Contains(position))
            {
                if (mouseState.LeftButton == ButtonState.Pressed
                    && _previousMouseState.LeftButton == ButtonState.Released)
                {
                    foreach (Node node in _nodes)
                    {
                        node.Type = NodeType.Normal; // Reset all nodes to normal
                    }
                    _startNode = null;
                    _endNode = null;
                }
            }

            _previousMouseState = mouseState;
        }

        /// <summary>
        /// Clears the start or end node if it is the one being clicked.
        /// </summary>
        /// <param name="node"></param>
        private void ClearNode(Node node)
        {
            if (node == _startNode)
            {
                _startNode.Type = NodeType.Normal;// Change the type to normal
                _startNode = null;// Clear the reference to the start node
            }
            if (node == _endNode)
            {
                _endNode.Type = NodeType.Normal;// Change the type to normal
                _endNode = null;// Clear the reference to the end node
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();
            //Draw the panel
            _spriteBatch.Draw(_panelTexture,
                new Rectangle(0, 0, PANEL_WIDTH, ROWS * TILE_SIZE),
                Color.DarkGray);

            //Draw the find path button
            _findButtonRectangle = new Rectangle(10, 10, BUTTON_WIDTH, BUTTON_HEIGHT);
            _spriteBatch.Draw(_buttonTexture, _findButtonRectangle, Color.LightGray);
            _spriteBatch.DrawString(_myFont, "Find Path", new Vector2(30, 20), Color.Black);

            //Draw the clear path button 
            _clearButtonRectangle = new Rectangle(10, BUTTON_HEIGHT + 15, BUTTON_WIDTH, BUTTON_HEIGHT);
            _spriteBatch.Draw(_buttonTexture, _clearButtonRectangle, Color.LightGray);
            _spriteBatch.DrawString(_myFont, "Clear Nodes", new Vector2(30, BUTTON_HEIGHT + 25), Color.Black);

            //Draw a label for the algorithm selection
            _spriteBatch.DrawString(_myFont, "Algorithm:", new Vector2(10, 120), Color.Black);

            //Draw the algorithm radio buttons
            //Dijkstra algorithm:
            _dijkstraButtonRectangle = new Rectangle(10, 150, 20, 20);
            DrawRadioButton(_spriteBatch, _dijkstraButtonRectangle, _algorithm == PathFindingAlgorithm.Dijkstra,
                "Dijkstra", new Vector2(35, _dijkstraButtonRectangle.Y + 2));
            //A Star algorithm:
            _astarButtonRectangle = new Rectangle(10, 180, 20, 20);
            DrawRadioButton(_spriteBatch, _astarButtonRectangle, _algorithm == PathFindingAlgorithm.AStar,
                "A Star", new Vector2(35, _astarButtonRectangle.Y + 2));

            //Heuristic buttons for when A* is selected:
            if (_algorithm == PathFindingAlgorithm.AStar)
            {
                _spriteBatch.DrawString(_myFont, "Heuristic:", new Vector2(10, 210), Color.Black);
                _manhattanButtonRectangle = new Rectangle(10, 250, 20, 20);
                _euclideanButtonRectangle = new Rectangle(10, 280, 20, 20);

                DrawRadioButton(_spriteBatch, _manhattanButtonRectangle, _heuristic == HeuristicType.Manhattan,
                    "Manhattan", new Vector2(35, _manhattanButtonRectangle.Y + 2));
                DrawRadioButton(_spriteBatch, _euclideanButtonRectangle, _heuristic == HeuristicType.Euclidean,
                    "Euclidean", new Vector2(35, _euclideanButtonRectangle.Y + 2));
            }

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

        private void DrawRadioButton(SpriteBatch spriteBatch, Rectangle buttonRect, bool selected, string label, Vector2 labelPos)
        {
            // Draw the radio button circle
            int circleRadius = 10;
            // Calculate the center of the circle based on the button rectangle
            Vector2 circleCentre = new Vector2(buttonRect.X + (int)(circleRadius), buttonRect.Y + buttonRect.Height / 2);
            Texture2D circleTexture = GetCircleTexture(circleRadius, selected ? Color.Black : Color.White);
            
            spriteBatch.Draw(circleTexture, new Rectangle((int)circleCentre.X - circleRadius,
                (int)circleCentre.Y - circleRadius, circleRadius * 2, circleRadius * 2), Color.White);// Draw the circle texture

            spriteBatch.DrawString(_myFont, label, labelPos, Color.Black);// Draw the label next to the radio button
        }

        private Texture2D GetCircleTexture(int radius, Color color)
        {
            Texture2D texture = new Texture2D(GraphicsDevice, radius * 2, radius * 2);// Create a new texture with double the radius for width and height
            Color[] data = new Color[radius * 2 * radius * 2];// Initialize the texture data array with the size of the texture
            Vector2 centre = new Vector2(radius, radius);// Calculate the center of the circle in the texture

            // Fill the texture data with colors to create a circle
            for (int y = 0; y < radius * 2; y++)
            {
                for (int x = 0; x < radius * 2; x++)
                {
                    int idX = y * radius * 2 + x;// Calculate the index in the data array
                    Vector2 diff = new Vector2(x, y) - centre; // Calculate the distance from the center
                    if (diff.Length() <= radius)
                    {
                        data[idX] = color; // Set the pixel color if within the circle
                    }
                    else
                    {
                        data[idX] = Color.Transparent; // Set transparent for outside the circle
                    }
                }
            }
            texture.SetData(data);// Set the texture data to the created color array
            return texture;
        }
    }
}