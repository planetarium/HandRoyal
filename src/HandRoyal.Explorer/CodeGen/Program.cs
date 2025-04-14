namespace HandRoyal.Explorer.CodeGen
{
    public static class Program
    {
        public static void Main()
        {
            var outputPath = Path.Combine(AppContext.BaseDirectory, "Generated");
            Directory.CreateDirectory(outputPath);

            var generator = new ActionQueryGenerator(outputPath, "HandRoyal.Explorer");
            generator.Generate();

            Console.WriteLine($"Generated files in: {outputPath}");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
