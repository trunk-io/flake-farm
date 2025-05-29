// See https://aka.ms/new-console-template for more information
public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
    }

    public static bool SometimesWorks()
    {
        return DateTime.Now.Millisecond % 2 == 0;
    }
}
