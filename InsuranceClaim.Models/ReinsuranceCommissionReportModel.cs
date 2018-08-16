﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceClaim.Models
{
   public class ReinsuranceCommissionReportModel
   {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PolicyNumber { get; set; }
        public string TransactionDate { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }

        public decimal AutoFacultativeReinsurance { get; set; }
        public decimal FacultativeReinsurance { get; set; }
    }


    public class ListReinsurance
    {
        public List<ReinsuranceCommissionReportModel> Reinsurance { get; set; }
    }





}