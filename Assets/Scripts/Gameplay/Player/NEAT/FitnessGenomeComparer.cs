using static Evaluator;
using System.Collections.Generic;

public class FitnessGenomeComparer : IComparer<FitnessGenome>
{
    public int Compare(FitnessGenome x, FitnessGenome y)
    {
        if (x.fitness > y.fitness)
        {
            return 1;
        }
        else if (x.fitness < y.fitness)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }
}