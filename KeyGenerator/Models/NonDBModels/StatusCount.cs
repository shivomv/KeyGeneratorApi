namespace KeyGenerator.Models.NonDBModels
{
    public class StatusCount
    {
        public int UserCount { get; internal set; }
        public int GroupCount { get; internal set; }
        public int AllPapersCount { get; internal set; }
        public int KeyGenerated { get; set; }
        public int MasterUploaded { get; set; }
        public int Pendingkeys { get; set; }
    }
}
