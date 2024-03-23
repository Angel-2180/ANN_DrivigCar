using System;
using System.Collections.Generic;

public class Evaluator
{
    private int _populationSize;
    private Dictionary<Genome, Species> _speciesDic;
    private Dictionary<Genome, float> _fitnessDic;
    private List<Genome> _population;
    private List<Species> _species;
    private List<Genome> _nextGeneration;

    private Counter _nodeInnovation;
    private Counter _connectionInnovation;

    /* constants */
    private const float C1 = 1.0f;
    private const float C2 = 1.0f;
    private const float C3 = 0.4f;
    private const float DELTA_T = 3.0f;
    private const float MUTATION_RATE = 0.1f;
    private const float ADD_CONNECTION_RATE = 0.75f;
    private const float ADD_NODE_RATE = 0.05f;

    private float _highestFitness = 0;
    private Genome _bestGenome;

    public Evaluator(int populationSize, Genome seedGenome, Counter nodeInnovation, Counter connectionInnovation)
    {
        this.EvaluateGenome = EvaluateGenomeImpl;
        _populationSize = populationSize;
        _speciesDic = new Dictionary<Genome, Species>();
        _fitnessDic = new Dictionary<Genome, float>();
        _population = new List<Genome>();
        _species = new List<Species>();
        _nextGeneration = new List<Genome>();

        _nodeInnovation = nodeInnovation;
        _connectionInnovation = connectionInnovation;

        for (int i = 0; i < populationSize; i++)
        {
            _population.Add(new Genome(seedGenome));
        }
    }

    public void Evaluate()
    {
        //reset species
        foreach (Species species in _species)
        {
            species.Reset();
        }
        _fitnessDic.Clear();
        _speciesDic.Clear();
        _nextGeneration.Clear();
        _highestFitness = float.MinValue;
        _bestGenome = null;

        //place genomes into species by using the compatibility distance
        foreach (Genome genome in _population)
        {
            bool foundSpecies = false;
            foreach (Species species in _species)
            {
                if (Genome.CompatibilityDistance(genome, species.bestGenome, C1, C2, C3) < DELTA_T)
                {
                    species.members.Add(genome);
                    _speciesDic.Add(genome, species);
                    foundSpecies = true;
                    break;
                }
            }
            if (!foundSpecies)
            {
                Species newSpecies = new Species(genome);
                _species.Add(newSpecies);
                _speciesDic.Add(genome, newSpecies);
            }
        }

        //remove empty species
        var iterator = _species.GetEnumerator();
        while (iterator.MoveNext())
        {
            if (iterator.Current.members.Count == 0)
            {
                _species.Remove(iterator.Current);
            }
        }

        //evaluate each genome and assign fitness
        foreach (Genome genome in _population)
        {
            Species species = _speciesDic[genome];
            float fitness = EvaluateGenome(genome);
            float adjustedFitness = fitness / species.members.Count; //The sharing function

            species.AddAdjustedFitness(adjustedFitness);
            species.fitnessPopulation.Add(new FitnessGenome(genome, adjustedFitness));
            _fitnessDic.Add(genome, fitness);
            if (fitness > _highestFitness)
            {
                _highestFitness = fitness;
                _bestGenome = genome;
            }
        }

        //put the best genome of each species into the next generation
        //Sort the fitnessPopulation of each species
        foreach (Species species in _species)
        {
            species.fitnessPopulation.Sort(new FitnessGenomeComparer());
            species.fitnessPopulation.Reverse();
            FitnessGenome daVeryBest = species.fitnessPopulation[0];
            _nextGeneration.Add(daVeryBest.genome);
        }

        //breed the rest of the genomes
        while (_nextGeneration.Count < _populationSize)
        {
            Species species = GetRandomSpeciesBiasedAdjustedFitness();

            Genome parent1 = GetRandomGenomeBiasedAdjustedFitness();
            Genome parent2 = GetRandomGenomeBiasedAdjustedFitness();

            Genome child;
            if (_fitnessDic[parent1] > _fitnessDic[parent2])
            {
                child = Genome.Crossover(parent1, parent2);
            }
            else
            {
                child = Genome.Crossover(parent2, parent1);
            }

            if (UnityEngine.Random.Range(0, 1) < MUTATION_RATE)
            {
                child.Mutate();
            }
            if (UnityEngine.Random.Range(0, 1) < ADD_CONNECTION_RATE)
            {
                child.AddConnectionMutation(_connectionInnovation);
            }

            if (UnityEngine.Random.Range(0, 1) < ADD_NODE_RATE)
            {
                child.AddNodeMutation(_nodeInnovation);
            }

            _nextGeneration.Add(child);
        }

        _population = _nextGeneration;
        _nextGeneration = new List<Genome>();
    }

    public Func<Genome, float> EvaluateGenome;

    protected virtual float EvaluateGenomeImpl(Genome genome)
    {
        return 0;
    }

    private Species GetRandomSpeciesBiasedAdjustedFitness()
    {
        double completeWeight = 0;
        foreach (Species species in _species)
        {
            completeWeight += species.totalAdjustedFitness;
        }

        double rand = UnityEngine.Random.Range(0,1) * completeWeight;
        double runningSum = 0;
        foreach (Species species in _species)
        {
            runningSum += species.totalAdjustedFitness;
            if (runningSum >= rand)
            {
                return species;
            }
        }
        throw new Exception("This should never happen");
    }

    private Genome GetRandomGenomeBiasedAdjustedFitness()
    {
        double completeWeight = 0;
        foreach (Species species in _species)
        {
            completeWeight += species.totalAdjustedFitness;
        }
        double rand = UnityEngine.Random.Range(0, 1) * completeWeight;
        double runningSum = 0;
        foreach (Species species in _species)
        {
            runningSum += species.totalAdjustedFitness;
            if (runningSum >= rand)
            {
                return species.fitnessPopulation[UnityEngine.Random.Range(0, species.fitnessPopulation.Count)].genome;
            }
        }
        throw new Exception("This should never happen");
    }

    public Genome GetBestGenome()
    {
        return _bestGenome;
    }

    public float GetHighestFitness()
    {
        return _highestFitness;
    }

    public int GetGenomeAmount()
    {
        return _population.Count;
    }

    public int GetSpeciesAmount()
    {
        return _species.Count;
    }

    public class FitnessGenome
    {
        public Genome genome;
        public float fitness;

        public FitnessGenome(Genome genome, float fitness)
        {
            this.genome = genome;
            this.fitness = fitness;
        }
    }

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

    public class Species
    {
        public List<Genome> members;
        public List<FitnessGenome> fitnessPopulation;
        public Genome bestGenome;
        public float totalAdjustedFitness;

        public Species(Genome daBest)
        {
            bestGenome = daBest;
            members = new List<Genome>
            {
                daBest
            };
            fitnessPopulation = new List<FitnessGenome>();
            totalAdjustedFitness = 0;
        }

        public void AddAdjustedFitness(float fitness)
        {
            totalAdjustedFitness += fitness;
        }

        public void Reset()
        {
            int newBestIndex = UnityEngine.Random.Range(0, members.Count);
            bestGenome = members[newBestIndex];
            members.Clear();
            fitnessPopulation.Clear();
            totalAdjustedFitness = 0;
        }
    }
}