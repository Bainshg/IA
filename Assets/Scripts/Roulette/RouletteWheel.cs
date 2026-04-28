using UnityEngine;

public static class RouletteWheel
{
    public static int Select(float[] weights)
    {
        float total = 0;
        foreach (float w in weights) total += w; //obtenemos el total
        float random = Random.Range(0, total); 
        float current = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            current += weights[i]; //le vamos sumando los weights a current
            if (random <= current) return i; //cuando current supera a random, elegimos ese weight
        }
        
        // si no retornó en el for, hubo un error raro pero retornamos 0 para catching.
        return 0;
    }
}