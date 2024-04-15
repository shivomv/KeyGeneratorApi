using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KeyGenerator.Models.NonDBModels
{
    public class ProgConfigInput
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProgConfigID { get; set; }

        public int ProgID { get; set; }

        public int Sets { get; set; }

        public string SetOrder { get; set; } // a,c,b,d

        public int NumberofQuestions { get; set; }

        public int BookletSize { get; set; }

        public int NumberofJumblingSteps { get; set; }

        public int SetofStepsID { get; set; }

        public string[] setofSteps { get; set; }
    }
}
