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
            _connectionGenes.Add(index, genome.GetConnectionGenes()[index].Clone());
        }

        foreach (int index in genome.GetNodeGenes().Keys)
        {
            _nodeGenes.Add(index, genome.GetNodeGenes()[index].Clone());
        }
    }

    public Genome Clone()
    {
        return new Genome(this);
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

    public void AddConnectionMutation(Counter innovation)
    {
        List<int> ints = new List<int>(_nodeGenes.Keys);
        int inNode = ints[UnityEngine.Random.Range(0, ints.Count)];
        ints.Remove(inNode);
        int outNode = ints[UnityEngine.Random.Range(0, ints.Count)];

        NodeGenes node1 = _nodeGenes.GetValueOrDefault(inNode);
        NodeGenes node2 = _nodeGenes.GetValueOrDefault(outNode);

        float weight = UnityEngine.Random.Range(-1f, 1f);

        bool isReversed = false;
        if (node1.GetNodeType() == NodeType.HIDDEN && node2.GetNodeType() == NodeType.INPUT)
        {
            isReversed = true;
        }
        else if (node1.GetNodeType() == NodeType.OUTPUT && node2.GetNodeType() == NodeType.HIDDEN)
        {
            isReversed = true;
        }
        else if (node1.GetNodeType() == NodeType.OUTPUT && node2.GetNodeType() == NodeType.INPUT)
        {
            isReversed = true;
        }

        if (isReversed)
        {
            NodeGenes temp = node1;
            node1 = node2;
            node2 = temp;
        }

        bool connectionExists = false;
        foreach (ConnectionGenes connection in _connectionGenes.Values)
        {
            if (connection.GetInNode() == node1.GetId() && connection.GetOutNode() == node2.GetId())
            {
                connectionExists = true;
                break;
            }
            else if (connection.GetInNode() == node2.GetId() && connection.GetOutNode() == node1.GetId())
            {
                connectionExists = true;
                break;
            }
        }

        bool connectionImpossible = false;
        if (node1.GetNodeType() == NodeType.INPUT && node2.GetNodeType() == NodeType.INPUT)
        {
            connectionImpossible = true;
        }
        else if (node1.GetNodeType() == NodeType.OUTPUT && node2.GetNodeType() == NodeType.OUTPUT)
        {
            connectionImpossible = true;
        }
        else if (node1 == node2)
        {
            connectionImpossible = true;
        }

        if (connectionExists || connectionImpossible)
        {
            return;
        }

        ConnectionGenes newConnection = new ConnectionGenes(
           node1.GetId(),
           node2.GetId(),
            weight,
            true,
            innovation.GetInnovation());
        _connectionGenes.Add(newConnection.GetInnovationNumber(), newConnection);
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

        ConnectionGenes connection = suitableConnections[UnityEngine.Random.Range(0, suitableConnections.Count)];

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

    public static Genome Crossover(Genome parent1, Genome parent2)
    {
        Genome child = new Genome();
        foreach (NodeGenes parentNode in parent1.GetNodeGenes().Values)
        {
            child.AddNodeGene(parentNode.Clone());
        }

        foreach (ConnectionGenes parent1Connection in parent1.GetConnectionGenes().Values)
        {
            if (parent2.GetConnectionGenes().ContainsKey(parent1Connection.GetInnovationNumber()))
            {
                ConnectionGenes childConGen =
                    UnityEngine.Random.Range(0, 1) == 1 ?
                    parent1Connection.Clone() :
                    parent2.GetConnectionGenes()[parent1Connection.GetInnovationNumber()].Clone();
                child.AddConnectionGene(childConGen);
            }
            else //disjoint or excess gene
            {
                ConnectionGenes childConGen = parent1Connection.Clone();
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

    internal void Mutate()
    {
        foreach (ConnectionGenes connection in _connectionGenes.Values)
        {
            if (UnityEngine.Random.Range(0f, 1f) < 0.8f)
            {
                connection.SetWeight(connection.GetWeight() * UnityEngine.Random.Range(-0.1f, 0.1f));
            }
            else
            {
                connection.SetWeight(UnityEngine.Random.Range(0, 1) * 4f - 2f);
            }
        }
    }
}