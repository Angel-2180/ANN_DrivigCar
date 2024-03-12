public class InnovationGenerator
{
    private int innovation;
    private int[] innovationNumbers;
    private int innovationIndex;

    public InnovationGenerator()
    {
        innovation = 0;
        innovationNumbers = new int[1000];
        innovationIndex = 0;
    }

    public int GetInnovation()
    {
        innovation++;
        innovationNumbers[innovationIndex] = innovation;
        innovationIndex++;
        return innovation;
    }

    public int GetInnovation(int index)
    {
        return innovationNumbers[index];
    }
}