using System;

public class RandomTextGenerator
{
    private static readonly string[] Texts = new string[] {
        "Contratame", "Hire me", "Contratado", "Texto 4"
    };

    public static string GenerateRandomText()
    {
        var random = new Random();
        return Texts[random.Next(Texts.Length)];
    }
}
