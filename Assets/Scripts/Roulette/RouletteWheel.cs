using UnityEngine;

public static class RouletteWheel
{
    public static T Select<T>(params (T item, float weight)[] options)
    {
        float total = 0;
        foreach (var o in options) total += o.weight;
        float random = Random.Range(0, total);
        float current = 0;
        foreach (var o in options)
        {
            current += o.weight;
            if (random <= current) return o.item;
        }
        return options[0].item;
    }
}