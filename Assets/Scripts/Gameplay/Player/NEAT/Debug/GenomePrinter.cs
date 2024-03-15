using System;
using System.Collections.Generic;
using UnityEngine;

public class GenomePrinter
{
    public static void PrintGenome(Genome genome)
    {
        string genomeString = "";
        genomeString += "Nodes:\n";
        foreach (NodeGenes node in genome.GetNodeGenes().Values)
        {
            genomeString += "Node: " + node.GetId() + " Type: " + node.GetNodeType() + "\n";
        }
        genomeString += "Connections:\n";
        foreach (ConnectionGenes connection in genome.GetConnectionGenes().Values)
        {
            genomeString += "Connection: " + connection.GetInNode() + " -> " + connection.GetOutNode() + " Weight: " + connection.GetWeight() + " Expressed: " + connection.IsExpressed() + " Innovation: " + connection.GetInnovationNumber() + "\n";
        }
        Debug.Log(genomeString);
    }
}