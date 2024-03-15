using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Genome
{
    private Dictionary<int, ConnectionGenes> connectionGenes;
    private Dictionary<int, NodeGenes> nodeGenes;

    public Genome()
    {
        connectionGenes = new Dictionary<int, ConnectionGenes>();
        nodeGenes = new Dictionary<int, NodeGenes>();
    }

    public Dictionary<int, ConnectionGenes> GetConnectionGenes() => connectionGenes;

    public Dictionary<int, NodeGenes> GetNodeGenes() => nodeGenes;

    public void AddNodeGene(NodeGenes node)
    {
        nodeGenes.Add(node.GetId(), node);
    }

    public void AddConnectionGene(ConnectionGenes connection)
    {
        connectionGenes.Add(connection.GetInnovationNumber(), connection);
    }

    public void AddConnectionMutation(InnovationGenerator innovation)
    {
        NodeGenes node1 = nodeGenes[UnityEngine.Random.Range(0, nodeGenes.Count)];
        NodeGenes node2 = nodeGenes[UnityEngine.Random.Range(0, nodeGenes.Count)];
        float weight = UnityEngine.Random.Range(-1f, 1f);

        bool isReversed = false;
        if (node1.GetNodeType() == NodeType.Hidden && node2.GetNodeType() == NodeType.Input)
        {
            isReversed = true;
        }
        else if (node1.GetNodeType() == NodeType.Output && node2.GetNodeType() == NodeType.Hidden)
        {
            isReversed = true;
        }
        else if (node1.GetNodeType() == NodeType.Output && node2.GetNodeType() == NodeType.Input)
        {
            isReversed = true;
        }

        bool connectionExists = false;
        foreach (ConnectionGenes connection in connectionGenes.Values)
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

        if (connectionExists)
        {
            return;
        }

        ConnectionGenes newConnection = new ConnectionGenes(
            isReversed ? node2.GetId() : node1.GetId(),
            isReversed ? node1.GetId() : node2.GetId(),
            weight,
            true,
            innovation.GetInnovation());
        connectionGenes.Add(newConnection.GetInnovationNumber(), newConnection);
    }

    public void AddNodeMutation(InnovationGenerator innovation)
    {
        ConnectionGenes connection = connectionGenes[UnityEngine.Random.Range(0, connectionGenes.Count)];

        NodeGenes inputNode = nodeGenes.GetValueOrDefault(connection.GetInNode());
        NodeGenes outputNode = nodeGenes.GetValueOrDefault(connection.GetOutNode());
        NodeGenes newNode = new NodeGenes(NodeType.Hidden, nodeGenes.Count);

        connection.Disable();

        ConnectionGenes newConnection1 = new ConnectionGenes(
            inputNode.GetId(),
            newNode.GetId(),
            1,
            true,
            innovation.GetInnovation());

        ConnectionGenes newConnection2 = new ConnectionGenes(
            newNode.GetId(),
            outputNode.GetId(),
            connection.GetWeight(),
            true,
            innovation.GetInnovation());

        nodeGenes.Add(newNode.GetId(), newNode);
        connectionGenes.Add(newConnection1.GetInnovationNumber(), newConnection1);
        connectionGenes.Add(newConnection2.GetInnovationNumber(), newConnection2);
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
}