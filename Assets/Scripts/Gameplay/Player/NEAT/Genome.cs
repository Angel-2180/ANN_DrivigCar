using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Genome
{
    private Dictionary<int, ConnectionGenes> _connectionGenes;
    private Dictionary<int, NodeGenes> _nodeGenes;

    public Genome()
    {
        _connectionGenes = new Dictionary<int, ConnectionGenes>();
        _nodeGenes = new Dictionary<int, NodeGenes>();
    }

    public Genome(Genome genome)
    {
        _connectionGenes = new Dictionary<int, ConnectionGenes>();
        _nodeGenes = new Dictionary<int, NodeGenes>();

        foreach (int index in genome.GetConnectionGenes().Keys)
        {
            _connectionGenes.Add(index, new ConnectionGenes(genome.GetConnectionGenes()[index]));
        }

        foreach (int index in genome.GetNodeGenes().Keys)
        {
            _nodeGenes.Add(index, new NodeGenes(genome.GetNodeGenes()[index]));
        }
    }

    public Dictionary<int, ConnectionGenes> GetConnectionGenes() => _connectionGenes;

    public Dictionary<int, NodeGenes> GetNodeGenes() => _nodeGenes;

    public void AddNodeGene(NodeGenes node)
    {
        _nodeGenes.Add(node.GetId(), node);
    }

    public void AddConnectionGene(ConnectionGenes connection)
    {
        _connectionGenes.Add(connection.GetInnovationNumber(), connection);
    }

    public void AddConnectionMutation(Counter innovation, int maxAttempt)
    {
        int tries = 0;
        bool success = false;
        while (tries < maxAttempt && success == false)
        {
            tries++;

            List<int> ints = _nodeGenes.Keys.ToList();
            int inNode = ints[RandomHelper.RandomInt(0, ints.Count)];
            int outNode = ints[RandomHelper.RandomInt(0, ints.Count)];

            NodeGenes inputNode = _nodeGenes[inNode];
            NodeGenes outputNode = _nodeGenes[outNode];
            float weight = RandomHelper.RandomGaussian();

            bool reversed = false;
            if (inputNode.GetNodeType() == NodeType.HIDDEN && outputNode.GetNodeType() == NodeType.INPUT)
            {
                reversed = true;
            }
            else if (inputNode.GetNodeType() == NodeType.OUTPUT && outputNode.GetNodeType() == NodeType.HIDDEN)
            {
                reversed = true;
            }
            else if (inputNode.GetNodeType() == NodeType.OUTPUT && outputNode.GetNodeType() == NodeType.INPUT)
            {
                reversed = true;
            }

            if (reversed)
            {
                int temp = inNode;
                inNode = outNode;
                outNode = temp;
            }

            bool connectionImpossible = false;
            if (inputNode.GetNodeType() == NodeType.OUTPUT && outputNode.GetNodeType() == NodeType.INPUT)
            {
                connectionImpossible = true;
            }
            else if (inputNode.GetNodeType() == NodeType.OUTPUT && outputNode.GetNodeType() == NodeType.OUTPUT)
            {
                connectionImpossible = true;
            }
            else if (inputNode.GetNodeType() == NodeType.INPUT && outputNode.GetNodeType() == NodeType.INPUT)
            {
                connectionImpossible = true;
            }
            else if (inputNode == outputNode)
            {
                connectionImpossible = true;
            }

            // check for circular connection
            HashSet<int> visitedNodes = new HashSet<int>(); // Use HashSet to prevent duplicates
            Queue<int> needsCheck = new Queue<int>(); // Use Queue for efficient FIFO operations
            List<int> nodeIDs = new List<int>();

            // Add the output node initially
            needsCheck.Enqueue(outputNode.GetId());
            visitedNodes.Add(outputNode.GetId());

            // BFS traversal to find reachable nodes
            while (needsCheck.Count > 0)
            {
                int currentNode = needsCheck.Dequeue();
                foreach (int conID in _connectionGenes.Keys)
                {
                    ConnectionGenes con = _connectionGenes[conID];
                    if (con.GetInNode() == currentNode && !visitedNodes.Contains(con.GetOutNode()))
                    {
                        visitedNodes.Add(con.GetOutNode());
                        needsCheck.Enqueue(con.GetOutNode());
                        nodeIDs.Add(con.GetOutNode());
                    }
                }
            }

            // Check if input node is reachable
            connectionImpossible = nodeIDs.Contains(inputNode.GetId());
            bool connectionExists = false;
            foreach (ConnectionGenes con in _connectionGenes.Values)
            {
                if (con.GetInNode() == inputNode.GetId() && con.GetOutNode() == outputNode.GetId())
                {
                    connectionExists = true;
                    break;
                }
                else if (con.GetInNode() == outputNode.GetId() && con.GetOutNode() == inputNode.GetId())
                {
                    connectionExists = true;
                    break;
                }
            }

            if (connectionImpossible || connectionExists)
            {
                continue;
            }

            ConnectionGenes newConnection = new ConnectionGenes(inNode, outNode, weight, true, innovation.GetInnovation());
            _connectionGenes.Add(newConnection.GetInnovationNumber(), newConnection);
            success = true;
        }

        if (success == false)
        {
            //Debug.Log("Failed to add connection after " + maxAttempt + " tries");
        }
    }

    public void AddNodeMutation(Counter connectionInnovation, Counter nodeInnovation)
    {
        List<ConnectionGenes> suitableConnections = new List<ConnectionGenes>();
        foreach (ConnectionGenes con in _connectionGenes.Values)
        {
            if (con.IsExpressed())
            {
                suitableConnections.Add(con);
            }
        }

        if (suitableConnections.Count == 0)
        {
            return;
        }

        ConnectionGenes connection = suitableConnections[RandomHelper.RandomInt(0, suitableConnections.Count)];

        NodeGenes inputNode = _nodeGenes.GetValueOrDefault(connection.GetInNode());
        NodeGenes outputNode = _nodeGenes.GetValueOrDefault(connection.GetOutNode());
        NodeGenes newNode = new NodeGenes(NodeType.HIDDEN, nodeInnovation.GetInnovation());

        connection.Disable();

        ConnectionGenes newConnection1 = new ConnectionGenes(
            inputNode.GetId(),
            newNode.GetId(),
            1,
            true,
            connectionInnovation.GetInnovation());

        ConnectionGenes newConnection2 = new ConnectionGenes(
            newNode.GetId(),
            outputNode.GetId(),
            connection.GetWeight(),
            true,
            connectionInnovation.GetInnovation());

        _nodeGenes.Add(newNode.GetId(), newNode);
        _connectionGenes.Add(newConnection1.GetInnovationNumber(), newConnection1);
        _connectionGenes.Add(newConnection2.GetInnovationNumber(), newConnection2);
    }

    public static Genome Crossover(Genome parent1, Genome parent2, float DISABLED_GENE_INHERITING_CHANCE)
    {
        Genome child = new Genome();
        foreach (NodeGenes parentNode in parent1.GetNodeGenes().Values)
        {
            child.AddNodeGene(new NodeGenes(parentNode));
        }

        foreach (ConnectionGenes parent1Connection in parent1.GetConnectionGenes().Values)
        {
            if (parent2.GetConnectionGenes().ContainsKey(parent1Connection.GetInnovationNumber()))
            {
                ConnectionGenes parent2Conn = parent2.GetConnectionGenes()[parent1Connection.GetInnovationNumber()];
                bool disabled = !parent1Connection.IsExpressed() || !parent2Conn.IsExpressed();
                ConnectionGenes childCon = RandomHelper.RandomBool() ? new ConnectionGenes(parent1Connection) : new ConnectionGenes(parent2Conn);
                if (disabled && RandomHelper.RandomZeroToOne() < DISABLED_GENE_INHERITING_CHANCE)
                {
                    childCon.Disable();
                }
                child.AddConnectionGene(childCon);
            }
            else //disjoint or excess gene
            {
                ConnectionGenes childConGen = new ConnectionGenes(parent1Connection);
                child.AddConnectionGene(childConGen);
            }
        }
        return child;
    }

    public static float CompatibilityDistance(Genome genome1, Genome genome2, float c1, float c2, float c3)
    {
        int excessGenes = 0;
        int disjointGenes = 0;
        float avgWeightDiff = 0;
        float weightDifference = 0;
        int matchingGenes = 0;

        //nodes
        List<int> nodeKeys1 = genome1.GetNodeGenes().Keys.ToList();
        List<int> nodeKeys2 = genome2.GetNodeGenes().Keys.ToList();
        nodeKeys1.Sort();
        nodeKeys2.Sort();

        int highestInnovation1 = nodeKeys1[nodeKeys1.Count - 1];
        int highestInnovation2 = nodeKeys2[nodeKeys2.Count - 1];
        int indices = highestInnovation1 > highestInnovation2 ? highestInnovation1 : highestInnovation2;
        for (int i = 0; i <= indices; i++)
        {
            NodeGenes node1 = genome1.GetNodeGenes().GetValueOrDefault(i);
            NodeGenes node2 = genome2.GetNodeGenes().GetValueOrDefault(i);
            if (node1 != null && node2 == null)
            {
                if (highestInnovation2 < i)
                {
                    excessGenes++;
                }
                else
                {
                    disjointGenes++;
                }
            }
            else if (node1 == null && node2 != null)
            {
                if (highestInnovation1 < i)
                {
                    excessGenes++;
                }
                else
                {
                    disjointGenes++;
                }
            }
        }

        //connections
        List<int> conKeys1 = genome1.GetConnectionGenes().Keys.ToList();
        List<int> conKeys2 = genome2.GetConnectionGenes().Keys.ToList();
        conKeys1.Sort();
        conKeys2.Sort();

        highestInnovation1 = conKeys1[conKeys1.Count - 1];
        highestInnovation2 = conKeys2[conKeys2.Count - 1];
        indices = highestInnovation1 > highestInnovation2 ? highestInnovation1 : highestInnovation2;

        for (int i = 0; i <= indices; i++)
        {
            ConnectionGenes connection1 = genome1.GetConnectionGenes().GetValueOrDefault(i);
            ConnectionGenes connection2 = genome2.GetConnectionGenes().GetValueOrDefault(i);
            if (connection1 != null)
            {
                if (connection2 != null)
                {
                    matchingGenes++;
                    weightDifference += Math.Abs(connection1.GetWeight() - connection2.GetWeight());
                }
                else if (highestInnovation2 < i)
                {
                    excessGenes++;
                }
                else
                {
                    disjointGenes++;
                }
            }
            else if (connection2 != null)
            {
                if (highestInnovation1 < i)
                {
                    excessGenes++;
                }
                else
                {
                    disjointGenes++;
                }
            }
        }

        avgWeightDiff = weightDifference / matchingGenes;

        int n = genome1.GetConnectionGenes().Count > genome2.GetConnectionGenes().Count ? genome1.GetConnectionGenes().Count : genome2.GetConnectionGenes().Count;

        if (n < 20)
        {
            n = 1;
        }

        return (excessGenes * c1) / n + (disjointGenes * c2) / n + avgWeightDiff * c3;
    }

    public static void CountExcessDisMatchAvg(Genome genome1, Genome genome2, out int excess, out int disjoint, out int matching, out float averageW)
    {
        excess = 0;
        disjoint = 0;
        averageW = 0;
        matching = 0;
        float weightDifference = 0;

        //nodes
        List<int> nodeKeys1 = genome1.GetNodeGenes().Keys.ToList();
        List<int> nodeKeys2 = genome2.GetNodeGenes().Keys.ToList();
        nodeKeys1.Sort();
        nodeKeys2.Sort();

        int highestInnovation1 = nodeKeys1[nodeKeys1.Count - 1];
        int highestInnovation2 = nodeKeys2[nodeKeys2.Count - 1];
        int indices = highestInnovation1 > highestInnovation2 ? highestInnovation1 : highestInnovation2;
        for (int i = 0; i <= indices; i++)
        {
            NodeGenes node1 = genome1.GetNodeGenes().GetValueOrDefault(i);
            NodeGenes node2 = genome2.GetNodeGenes().GetValueOrDefault(i);
            if (node1 != null && node2 == null)
            {
                if (highestInnovation2 < i)
                {
                    excess++;
                }
                else
                {
                    disjoint++;
                }
            }
            else if (node1 == null && node2 != null)
            {
                if (highestInnovation1 < i)
                {
                    excess++;
                }
                else
                {
                    disjoint++;
                }
            }
        }

        //connections
        List<int> conKeys1 = genome1.GetConnectionGenes().Keys.ToList();
        List<int> conKeys2 = genome2.GetConnectionGenes().Keys.ToList();
        conKeys1.Sort();
        conKeys2.Sort();

        highestInnovation1 = conKeys1[conKeys1.Count - 1];
        highestInnovation2 = conKeys2[conKeys2.Count - 1];
        indices = highestInnovation1 > highestInnovation2 ? highestInnovation1 : highestInnovation2;

        for (int i = 0; i <= indices; i++)
        {
            ConnectionGenes connection1 = genome1.GetConnectionGenes().GetValueOrDefault(i);
            ConnectionGenes connection2 = genome2.GetConnectionGenes().GetValueOrDefault(i);
            if (connection1 != null)
            {
                if (connection2 != null)
                {
                    matching++;
                    weightDifference += Math.Abs(connection1.GetWeight() - connection2.GetWeight());
                }
                else if (highestInnovation2 < i)
                {
                    excess++;
                }
                else
                {
                    disjoint++;
                }
            }
            else if (connection2 != null)
            {
                if (highestInnovation1 < i)
                {
                    excess++;
                }
                else
                {
                    disjoint++;
                }
            }
        }

        averageW = weightDifference / matching;
    }

    public void Mutate(float PROBABILITY_PERTURBING)
    {
        foreach (ConnectionGenes connection in _connectionGenes.Values)
        {
            if (RandomHelper.RandomZeroToOne() < PROBABILITY_PERTURBING)
            {
                connection.SetWeight(connection.GetWeight() * RandomHelper.RandomGaussian());
            }
            else
            {
                connection.SetWeight(RandomHelper.RandomZeroToOne() * 4f - 2f);
            }
        }
    }
}