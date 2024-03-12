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
        NodeGenes node1 = nodeGenes[Random.Range(0, nodeGenes.Count)];
        NodeGenes node2 = nodeGenes[Random.Range(0, nodeGenes.Count)];
        float weight = Random.Range(-1f, 1f);

        bool isReversed = false;
        if (node1.GetType() == NodeType.Hidden && node2.GetType() == NodeType.Input)
        {
            isReversed = true;
        }
        else if (node1.GetType() == NodeType.Output && node2.GetType() == NodeType.Hidden)
        {
            isReversed = true;
        }
        else if (node1.GetType() == NodeType.Output && node2.GetType() == NodeType.Input)
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
        //existing connection is split
        //new node is added where the connection was
        //two new connections are added
        //the new connection from the input node to the new node has a weight of 1
        //the new connection from the new node to the output node has the same weight as the original connection

        ConnectionGenes connection = connectionGenes[Random.Range(0, connectionGenes.Count)];

        NodeGenes inputNode = nodeGenes.GetValueOrDefault(connection.GetInNode());
        NodeGenes outputNode = nodeGenes.GetValueOrDefault(connection.GetOutNode());
        NodeGenes newNode = new NodeGenes(NodeType.Hidden, nodeGenes.Count);

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

    /**
     * @param parent1 - the more fit parent genome
     * @param parent2 - the less fit parent genome
     **/

    public Genome Crossover(Genome parent1, Genome parent2)
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
                    Random.Range(0, 1) == 1 ?
                    parent1Connection.Clone() :
                    parent2.GetConnectionGenes()[parent1Connection.GetInnovationNumber()].Clone();
            }
        }
        return null;
    }
}