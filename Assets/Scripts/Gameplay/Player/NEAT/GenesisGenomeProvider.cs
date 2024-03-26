using System;

public class GenesisGenomeProvider
{
    public GenesisGenomeProvider()
    {
        GenerateGenesisGenome = GenerateGenesisGenomeImpl;
    }

    public Func<Genome> GenerateGenesisGenome;

    public virtual Genome GenerateGenesisGenomeImpl()
    {
        throw new NotImplementedException();
    }
}