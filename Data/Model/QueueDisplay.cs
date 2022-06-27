using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data.Model
{
    public class QueueDisplay
    {
        [Key,Required]
        public long Id { get; set; }
        [Required]
        public string FirstToken { get; set; }
        [Required]

        public string FirstCounter { get; set; }
        [Required]

        public string SecondToken { get; set; }
        [Required]

        public string SecondCounter { get; set; }
        [Required]

        public string ThirdToken { get; set; }
        [Required]

        public string ThirdCounter { get; set; }
        [Required]

        public string FourthToken { get; set; }
        [Required]

        public string FourthCounter { get; set; }
        [Required]

        public string FifthToken { get; set; }
        [Required]

        public string FifthCounter { get; set; }
        [Required]

        public string SixthToken { get; set; }
        [Required]

        public string SixthCounter { get; set; }
        [Required]

        public string SeventhToken { get; set; }
        [Required]

        public string SeventhCounter { get; set; }
    }
}
