using UnityEngine;

public static class RouletteWheel
{
    public static int Select(float[] weights)
    {
        float total = 0;
        foreach (float w in weights) total += w;
        float random = Random.Range(0, total);
        float current = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            current += weights[i];
            if (random <= current) return i;
        }
        return 0;
    }
}