using System.Collections.Generic;

namespace myShortestPathMG
{
    public class DijkstraPF
    {
        private List<Node> _nodes;
        private int _rows;
        private int _cols;

        public DijkstraPF(List<Node> nodes, int rows, int cols)
        {
            //Initialise all of the values
            _nodes = nodes;
            _rows = rows;
            _cols = cols;
        }

        public List<Node> FindPath(Node startNode, Node endNode)
        {
            //Create containers for the nodes and their different states
            Dictionary<Node, int> distance = new Dictionary<Node, int>();
            Dictionary<Node, Node> previous = new Dictionary<Node, Node>();
            List<Node> queue = new List<Node>();
            List<Node> visited = new List<Node>();

            // Set initial distances
            foreach (Node node in _nodes)
            {
                distance[node] = int.MaxValue;
            }
            //Set the distance to the start node (0)
            distance[startNode] = 0;
            //Add the startnode to the queue
            queue.Add(startNode);

            while (queue.Count > 0)
            {
                // Get node with smallest distance
                queue.Sort((a, b) => distance[a].CompareTo(distance[b]));//Lambda expression to sort the queue based on distance
                Node current = queue[0];
                queue.RemoveAt(0);

                //If the current node is the end node then stop looping
                if (current == endNode)
                {
                    break;//Have to do this so that we can pop the node then stop the loop
                }

                //Add the current node to the visited list
                visited.Add(current);
                //Change the node type to Inspected only if the node is normal and visited
                if (current.Type == NodeType.Normal)
                {
                    current.Type = NodeType.Inspected; // Mark as part of the path
                }

                //Go through every node in the neighbours list returned by the 'GetNeighbours' of the current node
                foreach (Node neighbour in GetNeigbours(current))
                {
                    //If the neighbour has already been visited
                    if (visited.Contains(neighbour))
                    {
                        continue;// Skip already visited nodes
                    }

                    //Set a tentative distance
                    int tentative = distance[current] + 1; // All edges cost 1
                    //Check if the tentative distance is smaller than the distance to the neighbour
                    if (tentative < distance[neighbour])
                    {
                        //If it is, then replace the distance in the list
                        distance[neighbour] = tentative;
                        //Set the previous to the current node in preparation for the next iteration
                        previous[neighbour] = current;
                        //Check the queue doesn't already contain the neighbour
                        if (!queue.Contains(neighbour))
                        {
                            //Add to the queue
                            queue.Add(neighbour);
                        }
                    }
                }
            }

            // Reconstruct path
            List<Node> path = new List<Node>();
            //Set the temp node to the endNode
            Node temp = endNode;
            if (!previous.ContainsKey(temp))
            {
                return path; // No path found
            }

            //While we're not looking at the start node
            while (temp != startNode)
            {
                //Add the temp node to the path
                path.Add(temp);
                //Make the temp the previous node
                temp = previous[temp];
            }
            //Add the startnode to the path
            path.Add(startNode);
            //Reverse the path to get it in the correct order
            path.Reverse();
            return path;    
        }

        private List<Node> GetNeigbours(Node node)
        {
            //Create an array of tuples to store the offsets for the 4 possible directions
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
                int neighbourX = node.gridX + dX;// Calculate the neighbour's X position
                int neighbourY = node.gridY + dY;// Calculate the neighbour's Y position

                // Check if the neighbour's position is within bounds
                if (neighbourX >= 0 && neighbourX < _cols
                    && neighbourY >= 0 && neighbourY < _rows)
                {
                    // Get the neighbour node from the nodes list
                    Node neighbour = _nodes[neighbourY * _cols + neighbourX];
                    // If the neighbour is not a wall, add it to the neighbours list
                    if (neighbour.Type != NodeType.Wall)
                    {
                        neighbours.Add(neighbour);
                    }
                }
            }
            return neighbours;
        }
    }

}
