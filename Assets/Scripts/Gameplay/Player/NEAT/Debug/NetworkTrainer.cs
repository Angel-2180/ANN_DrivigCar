using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Evaluator;

public class NetworkTester : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start() //debug genome here
    {
        Counter nodeInnovation = new Counter();
        Counter connectionInnovation = new Counter();

        Genome genome = new Genome();
        int n1 = nodeInnovation.GetInnovation();
        int n2 = nodeInnovation.GetInnovation();
        int n3 = nodeInnovation.GetInnovation();
        genome.AddNodeGene(new NodeGenes(NodeType.INPUT, n1));
        genome.AddNodeGene(new NodeGenes(NodeType.INPUT, n2));
        genome.AddNodeGene(new NodeGenes(NodeType.OUTPUT, n3));

        int c1 = connectionInnovation.GetInnovation();
        int c2 = connectionInnovation.GetInnovation();

        genome.AddConnectionGene(new ConnectionGenes(n1, n3, 0.5f, true, c1));
        genome.AddConnectionGene(new ConnectionGenes(n2, n3, 0.5f, true, c2));



        //override EvaluateGenome method inline

        Evaluator evaluator = new Evaluator(100, genome, nodeInnovation, connectionInnovation)
        {
            EvaluateGenome = (Genome x) => x.GetConnectionGenes().Values.Count
        };

        for (int i = 0; i < 100; i++)
        {
            evaluator.Evaluate();
            Debug.Log("Generation: " + i);
            Debug.Log("Highest Fitness: " + evaluator.GetHighestFitness());
            Debug.Log("Species Count: " + evaluator.GetSpeciesAmount());
            Debug.Log("\n");
            if (i % 10 == 0)
            {
                GenomePrinter.PrintGenome(evaluator.GetBestGenome(), "Output/Connection_amount_1/" + i + ".txt");
            }
        }
    }

    // Update is called once per frame
    private void Update()
    {
    }
}