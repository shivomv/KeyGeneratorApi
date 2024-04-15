namespace KeyGenerator.Models.NonDBModels
{
    public class KeyEssential
    {

        public int Iterations { get; set; }

        public int Copies { get; set; }

        public List<string[]> SetofSteps { get; set; }

        public int ProgID { get; set; }

        public int PaperID { get; set; }

        public string CatchNumber { get; set; }

        public int SetID { get; set; }
    }
}
