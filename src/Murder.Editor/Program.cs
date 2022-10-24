namespace Murder.Editor
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var editor = new Architect())
            {
                editor.Run();
            }
        }
        
    }
}
