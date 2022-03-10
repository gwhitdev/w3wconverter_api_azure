using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.Models.Objects
{
    public class InputFromBody
    {
        [Required]
        public List<string> Uids { get; set; }
        [Required]
        public List<string> PostCodes { get; set; }
    }
}
