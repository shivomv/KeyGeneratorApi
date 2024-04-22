namespace KeyGenerator.Models.NonDBModels
{
    public class PaperDetails
    {
        public int PaperID { get; set; }
        public int ProgrammeID { get; set; }
        public string PaperName { get; set; }
        public string CatchNumber { get; set; }
        public string PaperCode { get; set; }
        public int CourseID { get; set; }
        public string ExamType { get; set; }
        public int SubjectID { get; set; }
        public string PaperNumber { get; set; }
        public DateTime ExamDate { get; set; }
        public int NumberofQuestion { get; set; }
        public int BookletSize { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedByID { get; set; }
        public bool MasterUploaded { get; set; }
        public bool KeyGenerated { get; set; }
        public string ProgrammeName { get; set; }
        public string CourseName { get; set; }
        public string SubjectName { get; set; }
        public string CreatedBy { get; set; }
    }

}
