using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data.Model
{
    public class CalendarView
    {
        public long Id { get; set; }
        public double MondayC { get; set; }
        public double TuesdayC { get; set; }
        public double WednesdayC { get; set; }
        public double ThursdayC { get; set; }
        public double FridayC { get; set; }
        public double SaturdayC { get; set; }
        public double SundayC { get; set; }
        public double Monday { get; set; }
        public double Tuesday { get; set; }
        public double Wednesday { get; set; }
        public double Thursday { get; set; }
        public double Friday { get; set; }
        public double Saturday { get; set; }
        public double Sunday { get; set; }

        [Required]

        public string MondayName { get; set; }
        [Required]

        public string TuesdayName { get; set; }
        [Required]

        public string WednesdayName { get; set; }
        [Required]

        public string ThursdayName { get; set; }
        [Required]

        public string FridayName { get; set; }
        [Required]

        public string SaturdayName { get; set; }
        [Required]

        public string SundayName { get; set; }

        [Required]

        public string MondayType { get; set; }

        [Required]

        public string TuesdayType { get; set; }

        [Required]

        public string WednesdayType { get; set; }

        [Required]

        public string ThursdayType { get; set; }

        [Required]

        public string FridayType { get; set; }

        [Required]

        public string SaturdayType { get; set; }

        [Required]

        public string SundayType { get; set; }
    }
}
