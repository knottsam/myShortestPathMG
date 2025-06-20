using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace myShortestPathMG
{
    public enum PathFindingAlgorithm
    {
        Dijkstra,
        AStar
    }
    public enum  HeuristicType
    {
        Manhattan,
        Euclidean
    }

    internal class PathFinder
    {
        private int _rows, _cols;
        private List<Node> _nodes;

        public List<Node> MyQueue { get; private set; }
        public List<Node> Visited { get; private set; }
        public Dictionary<Node, int> Distance { get; private set; }
        public Dictionary<Node, Node> Previous { get; private set; }
        public bool Finished { get; private set; }
        public Node EndNode { get; private set; }

        private HeuristicType _heuristic = HeuristicType.Manhattan;


        public PathFinder(List<Node> nodes, int rows, int cols)
        {
            _nodes = nodes;
            _rows = rows;
            _cols = cols;
            ResetState();
        }

        public void ResetState()
        {
            // Reset the state of the pathfinder and all data structures
            MyQueue = new List<Node>();
            Visited = new List<Node>();
            Distance = new Dictionary<Node, int>();
            Previous = new Dictionary<Node, Node>();
            Finished = false;
            EndNode = null;
        }

        public void StartAStar(Node startNode, Node endNode, HeuristicType heuristic)
        { 
            ResetState();
            foreach (Node n in _nodes)
            {
                Distance[n] = int.MaxValue; // Initialize distances to infinity
            }

            Distance[startNode] = 0; // Distance to start node is 0
            MyQueue.Add(startNode); // Add start node to the queue
            EndNode = endNode; // Set the end node
            _heuristic = heuristic; // Set the heuristic type
        }

        public void StepAStar()
        { 
            if(Finished || MyQueue.Count == 0)
            {
                Finished = true;
                return;
            }

            MyQueue.Sort((a, b) => (Distance[a] + 
            Heuristic(a, EndNode, _heuristic))
            .CompareTo(Distance[b] + Heuristic(b, EndNode, _heuristic)));

            Node current = MyQueue[0];
            MyQueue.RemoveAt(0);

            if (current == EndNode)
            {
                Finished = true;
                return; // Path found
            }

            Visited.Add(current);

            if (current.Type == NodeType.Normal)
            {
                current.Type = NodeType.Inspected; // Mark as inspected
            }

            foreach(Node neighbour in GetNeigbours(current))
            {
                if (Visited.Contains(neighbour))
                {
                    continue; // Skip already visited nodes
                }
                int tentative = Distance[current] + 1; // All edges cost 1
                if (tentative < Distance[neighbour])
                {
                    Distance[neighbour] = tentative;
                    Previous[neighbour] = current;
                    if (!MyQueue.Contains(neighbour))
                    {
                        MyQueue.Add(neighbour);
                    }
                }
            }
        }

        private int Heuristic(Node a, Node b, HeuristicType heuristic)
        {
            int dx = Math.Abs(a.gridX - b.gridX);//Calculate the difference in x-coordinates
            int dy = Math.Abs(a.gridY - b.gridY);//Calculate the difference in y-coordinates

            int heuristicValue;//Initialise the heuristic value
            switch (heuristic)
            {
                case HeuristicType.Manhattan:
                    heuristicValue = dx + dy; //Manhattan distance
                    break;
                case HeuristicType.Euclidean:
                    heuristicValue = (int)Math.Sqrt(dx * dx + dy * dy); //Euclidean distance
                    break;
                default:
                    heuristicValue = 0;
                    break;
            }
            return heuristicValue;
        }

        public void StartDijkstra(Node startNode, Node endNode)
        { 
            ResetState();// Reset the state of the pathfinder
            foreach (Node n in _nodes)
            {
                Distance[n] = int.MaxValue;// Initialize distances to infinity
            }
            Distance[startNode] = 0;// Distance to start node is 0
            MyQueue.Add(startNode);// Add start node to the queue
            EndNode = endNode;// Set the end node
        }
                
        public void StepDijkstra()
        {
            if (Finished || MyQueue.Count == 0)
            {
                Finished = true;// No more nodes to process
                return;// Exit the method
            }

            MyQueue.Sort((a, b) => Distance[a].CompareTo(Distance[b]));// Sort the queue based on distances
            Node current = MyQueue[0];// Get the node with the smallest distance
            MyQueue.RemoveAt(0);// Remove it from the queue

            if (current == EndNode)
            {
                Finished = true;
                return; // Path found
            }

            Visited.Add(current);
            if (current.Type == NodeType.Normal)
            {
                current.Type = NodeType.Inspected; // Mark as inspected
            }

            foreach (Node neighbour in GetNeigbours(current))
            {
                if (Visited.Contains(neighbour))
                {
                    continue; // Skip already visited nodes
                }

                int tentative = Distance[current] + 1; // All edges cost 1
                if (tentative < Distance[neighbour])
                {
                    Distance[neighbour] = tentative;
                    Previous[neighbour] = current;

                    if (!MyQueue.Contains(neighbour))
                    {
                        MyQueue.Add(neighbour);
                    }
                }
            }
        }

        private List<Node> GetNeigbours(Node node)
        {
            //Create an array of tuples to store the offsets for the 8 possible directions
            (int dX, int dY)[] offsets = new (int, int)[]
            {
                (-1, 0),  // Left
                (1, 0),   // Right
                (0, -1),  // Up
                (0, 1),   // Down
            };

            //Create a list of nodes to store the neighbours
            List<Node> neighbours = new List<Node>();

            //For each tuple in the offsets array, calculate the neighbour's position
            foreach ((int dX, int dY) in offsets)
            {
                int neighbourX = node.gridX + dX;
                int neighbourY = node.gridY + dY;

                if (neighbourX >= 0 && neighbourX < _cols
                    && neighbourY >= 0 && neighbourY < _rows)
                {
                    Node neighbour = _nodes[neighbourY * _cols + neighbourX];
                    if (neighbour.Type != NodeType.Wall)
                    {
                        neighbours.Add(neighbour);
                    }
                }
            }
            return neighbours;
        }

        public List<Node> GetPath(Node startNode, Node endNode)
        { 
            List<Node> path = new List<Node>();
            Node temp = endNode;

            if(!Previous.ContainsKey(temp))
            {
                return path; // No path found
            }

            while(temp != startNode)
            {
                path.Add(temp);
                temp = Previous[temp];
            }

            path.Add(startNode);
            path.Reverse();
            return path;
        }
    }
}
