namespace Murder.Editor
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            Environment.SetEnvironmentVariable("FNA_GRAPHICS_ENABLE_HIGHDPI", "1");

            using (var editor = new Architect())
            {
                editor.Run();
            }
        }
    }
}