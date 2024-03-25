using UnityEngine;

public static class RandomHelper
{
    public static float RandomZeroToOne()
    {
        return UnityEngine.Random.Range(0f, 1f);
    }
}