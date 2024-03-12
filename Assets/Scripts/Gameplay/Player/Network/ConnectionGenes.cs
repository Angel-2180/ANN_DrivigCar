public class ConnectionGenes
{
    private int inNode; //index  of the input node
    private int outNode; //index of the output node
    private float weight; //weight of the connection
    private bool expressed; //whether the connection is expressed
    private int innovationNumber; //innovation number of the connection

    public ConnectionGenes(int inNode, int outNode, float weight, bool expressed, int innovationNumber)
    {
        this.inNode = inNode;
        this.outNode = outNode;
        this.weight = weight;
        this.expressed = expressed;
        this.innovationNumber = innovationNumber;
    }

    public int GetInNode() => inNode;

    public int GetOutNode() => outNode;

    public float GetWeight() => weight;

    public bool IsExpressed() => expressed;

    public int GetInnovationNumber() => innovationNumber;

    public ConnectionGenes Clone()
    {
        return new ConnectionGenes(inNode, outNode, weight, expressed, innovationNumber);
    }
}