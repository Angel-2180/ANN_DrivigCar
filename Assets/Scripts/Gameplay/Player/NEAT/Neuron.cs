using System;

public class Neuron
{
    private float _output;
    private float[] _inputs;

    private int[] _outputIDs;
    private float[] _weights;

    public Neuron()
    {
        _inputs = new float[0];
        _outputIDs = new int[0];
        _weights = new float[0];
    }

    public int[] OutputIDs
    {
        get
        {
            return _outputIDs;
        }
    }

    public float[] Weights
    {
        get
        {
            return _weights;
        }
    }

    public float Output
    {
        get
        {
            return _output;
        }
    }

    public void AddOutputConnection(int id, float weight)
    {
        int[] newOutputIDs = new int[_outputIDs.Length + 1];
        for (int i = 0; i < _outputIDs.Length; i++)
        {
            newOutputIDs[i] = _outputIDs[i];
        }
        newOutputIDs[_outputIDs.Length] = id;
        _outputIDs = newOutputIDs;

        float[] newWeights = new float[_weights.Length + 1];
        for (int i = 0; i < _weights.Length; i++)
        {
            newWeights[i] = _weights[i];
        }
        newWeights[_weights.Length] = weight;
        _weights = newWeights;
    }

    public void AddInputConnection()
    {
        float[] newInputs = new float[_inputs.Length + 1];
        for (int i = 0; i < _inputs.Length; i++)
        {
            newInputs[i] = float.NaN;
        }
        this._inputs = newInputs;
    }

    private float ActivationFunction(float x) //sigmoid
    {
        return 1 / (1 + MathF.Exp(-4.9f * x));
    }

    public float CalculateOutput()
    {
        float sum = 0;
        for (int i = 0; i < _inputs.Length; i++)
        {
            sum += _inputs[i];
        }
        _output = ActivationFunction(sum);
        return _output;
    }

    public bool IsReady()
    {
        for (int i = 0; i < _inputs.Length; i++)
        {
            if (float.IsNaN(_inputs[i]))
            {
                return false;
            }
        }
        return true;
    }

    public void FeedInput(float value)
    {
        bool found = false;
        for (int i = 0; i < _inputs.Length; i++)
        {
            if (float.IsNaN(_inputs[i]))
            {
                _inputs[i] = value;
                found = true;
                break;
            }
        }
        if (!found)
        {
            throw new Exception("No input Slot. Input Array: " + _inputs.ToString());
        }
    }

    public void Reset()
    {
        for (int i = 0; i < _inputs.Length; i++)
        {
            _inputs[i] = float.NaN;
        }
        _output = 0;
    }
}