using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkTrainer : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start() //debug genome here
    {
        InnovationGenerator innovation = new InnovationGenerator();
        Genome parent1 = new Genome();
        Genome parent2 = new Genome();

        NodeGenes input1 = new NodeGenes(NodeType.Input, 1);
        NodeGenes input2 = new NodeGenes(NodeType.Input, 2);
        NodeGenes input3 = new NodeGenes(NodeType.Input, 3);
        NodeGenes hidden5 = new NodeGenes(NodeType.Hidden, 5);
        NodeGenes hidden6 = new NodeGenes(NodeType.Hidden, 6);
        NodeGenes output4 = new NodeGenes(NodeType.Output, 4);

        //parent1 : 1 -> 4, 2 -> 4 (disable), 3 -> 4, 2 -> 5, 5 -> 4, 1 -> 5
        //parent2 : 1 -> 4, 2 -> 4 (disable), 3 -> 4, 2 -> 5, 5 -> 4, 5 -> 6, 6 -> 4, 1 -> 5, 3 -> 5, 1 -> 6
        ConnectionGenes connection1 = new ConnectionGenes(input1.GetId(), output4.GetId(), 0.5f, true, innovation.GetInnovation());
        ConnectionGenes connection2 = new ConnectionGenes(input2.GetId(), output4.GetId(), 0.5f, false, innovation.GetInnovation());
        ConnectionGenes connection3 = new ConnectionGenes(input3.GetId(), output4.GetId(), 0.5f, true, innovation.GetInnovation());
        ConnectionGenes connection4 = new ConnectionGenes(input2.GetId(), hidden5.GetId(), 0.5f, true, innovation.GetInnovation());
        ConnectionGenes connection5 = new ConnectionGenes(hidden5.GetId(), output4.GetId(), 0.5f, true, innovation.GetInnovation());
        ConnectionGenes connection6 = new ConnectionGenes(hidden5.GetId(), hidden6.GetId(), 0.5f, true, innovation.GetInnovation());
        ConnectionGenes connection7 = new ConnectionGenes(hidden6.GetId(), output4.GetId(), 0.5f, true, innovation.GetInnovation());
        ConnectionGenes connection8 = new ConnectionGenes(input1.GetId(), hidden5.GetId(), 0.5f, true, innovation.GetInnovation());
        ConnectionGenes connection9 = new ConnectionGenes(input3.GetId(), hidden5.GetId(), 0.5f, true, innovation.GetInnovation());
        ConnectionGenes connection10 = new ConnectionGenes(input1.GetId(), hidden6.GetId(), 0.5f, true, innovation.GetInnovation());

        parent1.AddNodeGene(input1);
        parent1.AddNodeGene(input2);
        parent1.AddNodeGene(input3);
        parent1.AddNodeGene(hidden5);
        parent1.AddNodeGene(output4);
        parent1.AddConnectionGene(connection1);// matching
        parent1.AddConnectionGene(connection2);// matching
        parent1.AddConnectionGene(connection3);// matching
        parent1.AddConnectionGene(connection4);// matching
        parent1.AddConnectionGene(connection5);// matching
        parent1.AddConnectionGene(connection8);// disjoint

        parent2.AddNodeGene(input1);
        parent2.AddNodeGene(input2);
        parent2.AddNodeGene(input3);
        parent2.AddNodeGene(hidden5);
        parent2.AddNodeGene(hidden6);
        parent2.AddNodeGene(output4);
        parent2.AddConnectionGene(connection1);// matching
        parent2.AddConnectionGene(connection2);// matching
        parent2.AddConnectionGene(connection3);// matching
        parent2.AddConnectionGene(connection4);// matching
        parent2.AddConnectionGene(connection5);// matching
        parent2.AddConnectionGene(connection6);// disjoint
        parent2.AddConnectionGene(connection7);// disjoint
        parent2.AddConnectionGene(connection9);// excess
        parent2.AddConnectionGene(connection10);// excess

        GenomePrinter.PrintGenome(parent1);
        GenomePrinter.PrintGenome(parent2);

        Genome child = Genome.Crossover(parent2, parent1);

        GenomePrinter.PrintGenome(child);

        // count the number of excess and disjoint genes in parent1 and parent2
        int excess, disjoint, matching;
        float average;
        Genome.CountExcessDisMatchAvg(parent1, parent2, out excess, out disjoint, out matching, out average);
        Debug.Log("Excess: " + excess + " Disjoint: " + disjoint + " Matching: " + matching + " Average: " + average);
    }

    // Update is called once per frame
    private void Update()
    {
    }
}