using System;

public class RandomTextGenerator
{
    private static readonly string[] Texts = new string[] {
        "Texto 1", "Texto 2", "Texto 3", "Texto 4"
    };

    public static string GenerateRandomText()
    {
        var random = new Random();
        return Texts[random.Next(Texts.Length)];
    }
}
