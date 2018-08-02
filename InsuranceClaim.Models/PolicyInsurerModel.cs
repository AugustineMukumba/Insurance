﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceClaim.Models
{
  public class PolicyInsurerModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please Enter Insurer Name.")]
        public string InsurerName { get; set; }
        [Required(ErrorMessage = "Please Enter Insurer Code.")]
        public string InsurerCode { get; set; }
        [Required(ErrorMessage = "Please Enter Insurer Address.")]
        public string InsurerAddress { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }

    }
}
