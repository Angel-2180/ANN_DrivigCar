using System;
using System.Collections.Generic;
using UnityEngine;

public class GenomePrinter
{
    public static void PrintGenome(Genome genome)
    {
        Debug.Log("Printing genome");
        foreach (NodeGenes node in genome.GetNodeGenes().Values)
        {
            Debug.Log("Node: " + node.GetId() + " Type: " + node.GetType());
        }
        foreach (ConnectionGenes connection in genome.GetConnectionGenes().Values)
        {
            if (!connection.IsExpressed())
            {
                continue;
            }
            Debug.Log("Connection: " + connection.GetInNode() + " -> " + connection.GetOutNode() + " Weight: " + connection.GetWeight() + " Expressed: " + connection.IsExpressed() + " Innovation: " + connection.GetInnovationNumber());
        }

        //draw genome in ascii art
        string genomeString = "";
        genomeString += "Nodes:\n";
        foreach (NodeGenes node in genome.GetNodeGenes().Values)
        {
            genomeString += "Node: " + node.GetId() + " Type: " + node.GetType() + "\n";
        }
        genomeString += "Connections:\n";
        foreach (ConnectionGenes connection in genome.GetConnectionGenes().Values)
        {
            if (!connection.IsExpressed())
            {
                continue;
            }
            genomeString += "Connection: " + connection.GetInNode() + " -> " + connection.GetOutNode() + " Weight: " + connection.GetWeight() + " Expressed: " + connection.IsExpressed() + " Innovation: " + connection.GetInnovationNumber() + "\n";
        }
        Debug.Log(genomeString);
    }
}