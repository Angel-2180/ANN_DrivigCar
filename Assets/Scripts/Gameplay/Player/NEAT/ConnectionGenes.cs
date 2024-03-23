public class ConnectionGenes
{
    private int _inNode; //index  of the input node
    private int _outNode; //index of the output node
    private float _weight; //weight of the connection
    private bool _expressed; //whether the connection is expressed
    private int _innovationNumber; //innovation number of the connection

    public ConnectionGenes(int inNode, int outNode, float weight, bool expressed, int innovationNumber)
    {
        this._inNode = inNode;
        this._outNode = outNode;
        this._weight = weight;
        this._expressed = expressed;
        this._innovationNumber = innovationNumber;
    }

    public int GetInNode() => _inNode;

    public int GetOutNode() => _outNode;

    public float GetWeight() => _weight;

    public bool IsExpressed() => _expressed;

    public int GetInnovationNumber() => _innovationNumber;

    public ConnectionGenes Clone()
    {
        return new ConnectionGenes(_inNode, _outNode, _weight, _expressed, _innovationNumber);
    }

    public void Disable()
    {
        _expressed = false;
    }

    public void SetWeight(float weight)
    {
        _weight = weight;
    }
}