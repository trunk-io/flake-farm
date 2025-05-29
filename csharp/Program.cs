using System;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        FlakyFunction();
    }

    public static void FlakyFunction()
    {
        Random random = new Random();
        int randomNumber = random.Next(1, 5); // Generates a number between 1 and 4
        if (randomNumber == 1)
        {
            throw new Exception("This is a flaky exception!");
        }
        Console.WriteLine("FlakyFunction executed successfully.");
    }
}
