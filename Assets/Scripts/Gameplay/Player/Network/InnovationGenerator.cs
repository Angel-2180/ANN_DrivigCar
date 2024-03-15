public class InnovationGenerator
{
    private int innovation;

    public InnovationGenerator()
    {
        innovation = 0;
    }

    public int GetInnovation()
    {
        return innovation++;
    }
}