using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkTrainer : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start() //debug genome here
    {
        InnovationGenerator innovation = new InnovationGenerator();
        Genome genome = new Genome();

        NodeGenes input1 = new NodeGenes(NodeType.Input, 0);
        NodeGenes input2 = new NodeGenes(NodeType.Input, 1);
        NodeGenes output1 = new NodeGenes(NodeType.Output, 2);

        ConnectionGenes connection1 = new ConnectionGenes(0, 2, 1f, true, innovation.GetInnovation());
        ConnectionGenes connection2 = new ConnectionGenes(1, 2, 1f, true, innovation.GetInnovation());

        genome.AddNodeGene(input1);
        genome.AddNodeGene(input2);
        genome.AddNodeGene(output1);

        genome.AddConnectionGene(connection1);
        genome.AddConnectionGene(connection2);

        GenomePrinter.PrintGenome(genome, "Assets/Genome_pre_NodeMut.png");

        genome.AddNodeMutation(innovation);

        GenomePrinter.PrintGenome(genome, "Assets/Genome_post_NodeMut.png");
    }

    // Update is called once per frame
    private void Update()
    {
    }
}