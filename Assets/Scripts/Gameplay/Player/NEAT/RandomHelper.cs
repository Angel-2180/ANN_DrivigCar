using UnityEngine;

public static class RandomHelper
{
    public static float RandomZeroToOne()
    {
        return UnityEngine.Random.Range(0f, 1f);
    }

    public static int RandomInt(int min, int max)
    {
        return UnityEngine.Random.Range(min, max);
    }

    public static int RandomGaussian()
    {
        return UnityEngine.Random.Range(-1, 1);
    }
}