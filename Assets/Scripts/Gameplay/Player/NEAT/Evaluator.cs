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

    public Evaluator(NEATConfig config, GenesisGenomeProvider generator, Counter nodeInnovation, Counter connectionInnovation)
    {
        this.EvaluateGenome = EvaluateGenomeImpl;
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

    public Func<Genome, float> EvaluateGenome;

    public virtual float EvaluateGenomeImpl(Genome genome)
    {
        throw new NotImplementedException();
    }

    public void EvaluateGeneration()
    {
        _lastGenResults.Clear();
        _evaluatedGenomes.Clear();

        // Evaluate all genomes
        foreach (Genome genome in _genomePopulation)
        {
            float fitness = EvaluateGenome(genome);
            _evaluatedGenomes.Add(new FitnessGenome(genome, fitness));
        }

        // Sort genomes by fitness then reverse the list
        _evaluatedGenomes.Sort((a, b) => b.fitness.CompareTo(a.fitness));
        _evaluatedGenomes.Reverse();

        // Get the best genome
        _bestGenome = _evaluatedGenomes[0];

        // kill 9/10 worst genomes
        int cutoffIndex = _evaluatedGenomes.Count / 10;

        for (int i = cutoffIndex; i < _evaluatedGenomes.Count; i++)
        {
            _evaluatedGenomes[i].genome = null;
        }
        //remove all null genomes
        _evaluatedGenomes.RemoveAll(x => x.genome == null);

        _nextGeneration.Clear();

        // Add the best genome to the next generation
        _nextGeneration.Add(_bestGenome.genome);

        //Breeding loop
        while (_nextGeneration.Count < config.PopulationSize)
        {
            float r = RandomHelper.RandomZeroToOne();
            if (r > config.ASEXUAL_REPRODUCTION_RATE)
            {
                FitnessGenome parent1 = _evaluatedGenomes[RandomHelper.RandomInt(0, _evaluatedGenomes.Count)];
                FitnessGenome parent2 = _evaluatedGenomes[RandomHelper.RandomInt(0, _evaluatedGenomes.Count)];

                Genome child;
                if (parent1.fitness > parent2.fitness)
                {
                    child = Genome.Crossover(parent1.genome, parent2.genome, config.DISABLED_GENE_INHERITING_CHANCE);
                }
                else
                {
                    child = Genome.Crossover(parent1.genome, parent2.genome, config.DISABLED_GENE_INHERITING_CHANCE);
                }

                //Mutations
                if (RandomHelper.RandomZeroToOne() < config.MUTATION_RATE)
                {
                    child.Mutate(config.PERTURBING_RATE);
                }
                if (RandomHelper.RandomZeroToOne() < config.ADD_NODE_RATE)
                {
                    child.AddNodeMutation(_connectionInnovation, _nodeInnovation);
                }
                if (RandomHelper.RandomZeroToOne() < config.ADD_CONNECTION_RATE)
                {
                    child.AddConnectionMutation(_connectionInnovation, 1);
                }
                _nextGeneration.Add(child);
            }
            else
            {
                FitnessGenome parent = _evaluatedGenomes[RandomHelper.RandomInt(0, _evaluatedGenomes.Count)];
                Genome child = new Genome(parent.genome);
                child.Mutate(config.PERTURBING_RATE);
                _nextGeneration.Add(child);
            }
        }
        _genomePopulation.Clear();
        _genomePopulation.AddRange(_nextGeneration);
    }

    public List<Genome> GenomesPopulation
    {
        get { return _genomePopulation; }
    }

    public FitnessGenome BestGenome
    {
        get { return _bestGenome; }
    }

    public int GenomesAmount
    {
        get { return _genomePopulation.Count; }
    }

    public List<FitnessGenome> LastGenResults
    {
        get { return _lastGenResults; }
    }
}