using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine.Events;

public class NetworkTester : EditorWindow
{
    // Start is called before the first frame update
    private Genome genome;

    private float[][] inputs;

    private float[] correctResults;
    private Counter nodeInnovation;
    private Counter connectionInnovation;
    private GenesisGenomeProvider provider;
    private NEATConfig conf;
    private Evaluator evaluator;
    private int generation = 0;

    private void Init()
    {
        generation = 0;
        genome = new Genome();

        inputs = new float[4][];
        inputs[0] = new float[] { 0, 0, 1 };
        inputs[1] = new float[] { 0, 1, 1 };
        inputs[2] = new float[] { 1, 0, 1 };
        inputs[3] = new float[] { 1, 1, 1 };

        correctResults = new float[] { 0, 1, 1, 0 };

        nodeInnovation = new Counter();
        connectionInnovation = new Counter();
        ConfigGenome(genome);
        provider = new GenesisGenomeProvider()
        {
            GenerateGenesisGenome = () =>
            {
                Genome g = new Genome(genome);
                foreach (ConnectionGenes connection in g.GetConnectionGenes().Values)
                {
                    connection.SetWeight(RandomHelper.RandomGaussian());
                }
                return g;
            }
        };

        conf = new NEATConfig(100);

        evaluator = new Evaluator(conf, provider, nodeInnovation, connectionInnovation)
        {
            EvaluateGenome = (Genome genome) =>
            {
                NeuralNetwork network = new NeuralNetwork(genome);
                float fitness = 0;
                for (int i = 0; i < inputs.Length; i++)
                {
                    float[] input = new float[] { inputs[i][0], inputs[i][1], inputs[i][2] };
                    float[] output = network.Calculate(input);
                    float dist = Mathf.Abs(correctResults[i] - output[0]);
                    fitness += Mathf.Pow(dist, 2);
                }
                if (genome.GetConnectionGenes().Count > 20)
                {
                    fitness += 1f * (genome.GetConnectionGenes().Count - 20);
                }
                return 100 - fitness * 5;
            }
        };

        LearnFinished.AddListener(() =>
        {
            WaitForSeconds wait = new WaitForSeconds(10);
            Learn(10);
        });

        Debug.Log("Init");
    }

    private void ConfigGenome(Genome genome)
    {
        genome.AddNodeGene(new NodeGenes(NodeType.INPUT, nodeInnovation.GetInnovation())); //0
        genome.AddNodeGene(new NodeGenes(NodeType.INPUT, nodeInnovation.GetInnovation())); //1
        genome.AddNodeGene(new NodeGenes(NodeType.INPUT, nodeInnovation.GetInnovation())); //2
        genome.AddNodeGene(new NodeGenes(NodeType.OUTPUT, nodeInnovation.GetInnovation())); //3

        genome.AddConnectionGene(new ConnectionGenes(0, 3, 1, true, connectionInnovation.GetInnovation()));
        genome.AddConnectionGene(new ConnectionGenes(1, 3, 1, true, connectionInnovation.GetInnovation()));
        genome.AddConnectionGene(new ConnectionGenes(2, 3, 1, true, connectionInnovation.GetInnovation()));

        Debug.Log("Configured Genome");
    }

    private UnityEvent LearnFinished = new UnityEvent();

    private void Learn(int nbrLoop)
    {
        Debug.Log("Start Learning");
        for (int i = 0; i < nbrLoop; i++)
        {
            evaluator.EvaluateGeneration();
            Debug.Log("Generation: " + i + " Best Fitness: " + evaluator.BestGenome.fitness + " Amount of Genomes: " + evaluator.GenomesAmount + " Guess from best: ");
            NeuralNetwork bestNetwork = new NeuralNetwork(evaluator.BestGenome.genome);
            for (int j = 0; j < inputs.Length; j++)
            {
                float[] input = new float[] { inputs[j][0], inputs[j][1], inputs[j][2] };
                float[] output = bestNetwork.Calculate(input);
                Debug.Log("Input: " + inputs[j][0] + " " + inputs[j][1] + " " + inputs[j][2] + " Output: " + output[0] + " Expected Output: " + correctResults[j]);
            }

            if (i % 50 == 0)
            {
                GenomePrinter.PrintGenome(evaluator.BestGenome.genome, "Output/XORTest/" + i + ".txt");
            }
            generation++;
        }
        Debug.Log("Done Learning");
        LearnFinished.Invoke();
    }

    private void CreateGUI()
    {
        int nbrOfLoop = 100;
        var root = rootVisualElement;

        Button btn = new Button();
        btn.name = "Init";
        btn.text = "Init";
        btn.clickable.clicked += () =>
        {
            Init();
        };
        root.Add(btn);

        //input fields
        TextField inputField = new TextField();
        inputField.name = "Number Of Loops";
        inputField.label = "Number Of Loops: ";
        inputField.value = nbrOfLoop.ToString();

        inputField.RegisterValueChangedCallback(evt =>
        {
            nbrOfLoop = int.Parse(evt.newValue);
        });
        root.Add(inputField);

        btn = new Button();
        btn.name = "Learn";
        btn.text = "Learn";
        btn.clickable.clicked += () =>
        {
            Learn(nbrOfLoop);
        };
        root.Add(btn);

        btn = new Button();
        btn.name = "CancelAutoLearn";
        btn.text = "Cancel Auto Learn";
        btn.clickable.clicked += () =>
        {
            LearnFinished.RemoveAllListeners();
        };
        root.Add(btn);

        TextField text = new TextField();
        text.name = "Generation";
        text.label = "Generation: ";
        text.value = generation.ToString();
        root.Add(text);
    }

    [MenuItem("AngelUwU/Network Tester")]
    private static void InitWindow()
    {
        NetworkTester wnd = GetWindow<NetworkTester>();
        wnd.titleContent = new GUIContent("Network Tester");
    }
}