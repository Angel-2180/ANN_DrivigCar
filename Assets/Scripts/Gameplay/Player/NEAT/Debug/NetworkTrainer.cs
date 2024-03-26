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
        Genome genome;
        NeuralNetwork net;

        float[] input;
        float[] output;
        Debug.Log("==========================TEST 1==========================");
        genome = new Genome();
        genome.AddNodeGene(new NodeGenes(NodeType.INPUT, 0));
        genome.AddNodeGene(new NodeGenes(NodeType.OUTPUT, 1));

        genome.AddConnectionGene(new ConnectionGenes(0, 1, 0.5f, true, 0));

        net = new NeuralNetwork(genome);
        input = new float[] { 1 };
        for (int i = 0; i < 3; i++)
        {
            output = net.Calculate(input);

            Debug.Log("output is of length=" + output.Length + " and has output[0]=" + output[0] + " expecting 0.9192");
        }

        Debug.Log("==========================TEST 2==========================");

        genome = new Genome();
        genome.AddNodeGene(new NodeGenes(NodeType.INPUT, 0));                    // node id is 0
        genome.AddNodeGene(new NodeGenes(NodeType.OUTPUT, 1));                   // node id is 1

        genome.AddConnectionGene(new ConnectionGenes(0, 1, 0.1f, true, 0));  // conn id is 0

        net = new NeuralNetwork(genome);
        input = new float[] { -0.5f };
        for (int i = 0; i < 3; i++)
        {
            output = net.Calculate(input);

            Debug.Log("output is of length=" + output.Length + " and has output[0]=" + output[0] + " expecting 0.50973");
        }

        Debug.Log("==========================TEST 3==========================");

        genome = new Genome();
        genome.AddNodeGene(new NodeGenes(NodeType.INPUT, 0));                    // node id is 0
        genome.AddNodeGene(new NodeGenes(NodeType.OUTPUT, 1));                   // node id is 1
        genome.AddNodeGene(new NodeGenes(NodeType.HIDDEN, 2));                   // node id is 2

        genome.AddConnectionGene(new ConnectionGenes(0, 2, 0.4f, true, 0));  // conn id is 0
        genome.AddConnectionGene(new ConnectionGenes(2, 1, 0.7f, true, 1));  // conn id is 1

        net = new NeuralNetwork(genome);
        input = new float[] { 0.9f };
        for (int i = 0; i < 3; i++)
        {
            output = net.Calculate(input);
            Debug.Log("output is of length=" + output.Length + " and has output[0]=" + output[0] + " expecting 0.9524");
        }
    }

    // Update is called once per frame
    private void Update()
    {
    }
}