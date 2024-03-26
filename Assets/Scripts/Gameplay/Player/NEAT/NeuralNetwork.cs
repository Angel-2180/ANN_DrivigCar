using System.Collections.Generic;

public class NeuralNetwork
{
    private Dictionary<int, Neuron> _neurons;
    private List<int> _inputIDs;
    private List<int> _outputIDs;

    private List<Neuron> _unprocessed;

    public NeuralNetwork(Genome genome)
    {
        _inputIDs = new List<int>();
        _outputIDs = new List<int>();
        _neurons = new Dictionary<int, Neuron>();
        _unprocessed = new List<Neuron>();

        foreach (int nodeID in genome.GetNodeGenes().Keys)
        {
            NodeGenes node = genome.GetNodeGenes()[nodeID];
            Neuron neuron = new Neuron();

            if (node.GetNodeType() == NodeType.INPUT)
            {
                neuron.AddInputConnection();
                _inputIDs.Add(nodeID);
            }
            else if (node.GetNodeType() == NodeType.OUTPUT)
            {
                _outputIDs.Add(nodeID);
            }
            _neurons.Add(nodeID, neuron);
        }

        foreach (int connectionID in genome.GetConnectionGenes().Keys)
        {
            ConnectionGenes connection = genome.GetConnectionGenes()[connectionID];
            if (!connection.IsExpressed())
            {
                continue;
            }

            Neuron fromNeuron = _neurons[connection.GetInNode()];
            fromNeuron.AddOutputConnection(connection.GetOutNode(), connection.GetWeight());
            Neuron toNeuron = _neurons[connection.GetOutNode()];
            toNeuron.AddInputConnection();
        }
    }

    public float[] Calculate(float[] inputParam)
    {
        if (inputParam.Length != _inputIDs.Count)
        {
            throw new System.Exception("Input size does not match the number of input neurons");
        }

        foreach (int key in _neurons.Keys)
        {
            _neurons[key].Reset();
        }

        _unprocessed.Clear();
        _unprocessed.AddRange(_neurons.Values);

        for (int i = 0; i < inputParam.Length; i++) // loop through input neurons
        {
            Neuron inputNeuron = _neurons[_inputIDs[i]];
            inputNeuron.FeedInput(inputParam[i]); // input neurons only have one input, so we know they are ready
            inputNeuron.CalculateOutput();

            for (int j = 0; j < inputNeuron.OutputIDs.Length; j++) // loop through output neurons
            {
                Neuron outputNeuron = _neurons[inputNeuron.OutputIDs[j]];
                outputNeuron.FeedInput(inputNeuron.Output * inputNeuron.Weights[j]); // add the input directly to the next neuron, using the correct weight for the connection
            }
            _unprocessed.Remove(inputNeuron);
        }

        int loops = 0;
        while (_unprocessed.Count > 0)
        {
            loops++;
            if (loops > 1000)
            {
                throw new System.Exception("Infinite loop detected");
            }

            for (int i = _unprocessed.Count; i > 0; i--)
            {
                Neuron neuron = _unprocessed[i - 1];
                if (neuron.IsReady())
                {
                    neuron.CalculateOutput();

                    for (int j = 0; j < neuron.OutputIDs.Length; j++)
                    {
                        int outputID = neuron.OutputIDs[j];
                        float value = neuron.Output * neuron.Weights[j];
                        _neurons[outputID].FeedInput(value);
                    }
                    _unprocessed.Remove(neuron);
                }
            }
        }

        float[] output = new float[_outputIDs.Count];
        for (int i = 0; i < _outputIDs.Count; i++)
        {
            output[i] = _neurons[_outputIDs[i]].Output;
        }

        return output;
    }
}