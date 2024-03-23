using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GenomePrinter
{
    public static void PrintGenome(Genome genome, string path)
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

        //you have to create the file before calling this method
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            using (FileStream fs = File.Create(path))
            {
                byte[] info = new System.Text.UTF8Encoding(true).GetBytes(genomeString);
                fs.Write(info, 0, info.Length);
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
    }
}