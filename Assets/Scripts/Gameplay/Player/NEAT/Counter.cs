public class Counter
{
    private int _innovation;

    public Counter()
    {
        _innovation = 0;
    }

    public int GetInnovation()
    {
        return _innovation++;
    }
}