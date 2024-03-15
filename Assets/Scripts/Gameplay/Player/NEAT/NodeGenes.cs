using System.Collections.Generic;

public class NodeGenes
{
    private NodeType _type; //type of the node
    private int _id; //id of the node

    public NodeGenes(NodeType type, int id)
    {
        this._type = type;
        this._id = id;
    }

    public NodeType GetNodeType() => _type;

    public int GetId() => _id;

    public NodeGenes Clone()
    {
        return new NodeGenes(_type, _id);
    }
}