using System.Collections.Generic;

public class NodeGenes
{
    private NodeType type; //type of the node
    private int id; //id of the node

    public NodeGenes(NodeType type, int id)
    {
        this.type = type;
        this.id = id;
    }

    public NodeType GetNodeType() => type;

    public int GetId() => id;

    public NodeGenes Clone()
    {
        return new NodeGenes(type, id);
    }
}