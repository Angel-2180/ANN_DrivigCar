using System;
using System.Collections.Generic;

public class Evaluator
{
    private NEATConfig config;

    private List<Genome> _genomePopulation;
    private List<Genome> _nextGeneration;
    private List<FitnessGenome> _evaluatedGenomes;
    private FitnessGenome _bestGenome;

    private List<FitnessGenome> _lastGenResults;

    private Counter _nodeInnovation;
    private Counter _connectionInnovation;

    private Evaluator(NEATConfig config, IGenesisGenomeProvider generator, Counter nodeInnovation, Counter connectionInnovation)
    {
        this.config = config;
        _genomePopulation = new List<Genome>(config.PopulationSize);
        for (int i = 0; i < config.PopulationSize; i++)
        {
            Genome g = generator.GenerateGenesisGenome();
            _genomePopulation.Add(g);
        }

        _evaluatedGenomes = new List<FitnessGenome>();
        _nextGeneration = new List<Genome>();

        _lastGenResults = new List<FitnessGenome>();

        _nodeInnovation = nodeInnovation;
        _connectionInnovation = connectionInnovation;
    }
}