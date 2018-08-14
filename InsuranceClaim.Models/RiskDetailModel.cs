﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceClaim.Models
{
    public class RiskDetailModel
    {
        public bool IncludeRadioLicenseCost { get; set; }
        public int Id { get; set; }
        public int PolicyId { get; set; }
        //[Required(ErrorMessage = "Please Enter No Of Cars Covered")]
        //[DefaultValue(1)]
        public int? NoOfCarsCovered { get; set; }
        [Required(ErrorMessage = "Please Enter Registration No")]
        public string RegistrationNo { get; set; }
        public int? CustomerId { get; set; }
        public string MakeId { get; set; }
        public string ModelId { get; set; }
        [Required(ErrorMessage = "Please Enter Cubic Capacity")]
        public decimal? CubicCapacity { get; set; }
        [Required(ErrorMessage = "Please Enter Vehicle Year")]
        public int? VehicleYear { get; set; }
        [Required(ErrorMessage = "Please Enter Engine Number")]
        public string EngineNumber { get; set; }
        [Required(ErrorMessage = "Please Enter Chassis Number")]
        public string ChasisNumber { get; set; }
        public string VehicleColor { get; set; }
        public int? VehicleUsage { get; set; }
        public int? CoverTypeId { get; set; }
        [Required(ErrorMessage = "Please Enter Cover Start Date")]
        public DateTime? CoverStartDate { get; set; }
        [Required(ErrorMessage = "Please Enter Cover End Date")]

        public DateTime? CoverEndDate { get; set; }
       

        public decimal? SumInsured { get; set; }
        [Required(ErrorMessage = "Please Enter Basic Premium")]
        public decimal? Premium { get; set; }
        public int? AgentCommissionId { get; set; }
        public decimal? Rate { get; set; }
        public decimal? StampDuty { get; set; }
        public decimal? ZTSCLevy { get; set; }
        public decimal? RadioLicenseCost { get; set; }
        public string OptionalCovers { get; set; }
        public int ExcessType { get; set; }
        public decimal Excess { get; set; }
        public string CoverNoteNo { get; set; }
        public bool IsLicenseDiskNeeded { get; set; }
        public Boolean Addthirdparty { get; set; }
        public decimal? AddThirdPartyAmount { get; set; }

        public Boolean PassengerAccidentCover { get; set; }
        public Boolean ExcessBuyBack { get; set; }
        public Boolean RoadsideAssistance { get; set; }
        public Boolean MedicalExpenses { get; set; }
        public int? NumberofPersons { get; set; }
        public bool chkAddVehicles { get; set; }
        public bool isUpdate { get; set; }
        public int vehicleindex { get; set; }
        public decimal? PassengerAccidentCoverAmount { get; set; }
        public decimal? ExcessBuyBackAmount { get; set; }
        public decimal? RoadsideAssistanceAmount { get; set; }
        public decimal? MedicalExpensesAmount { get; set; }
        public decimal? PassengerAccidentCoverAmountPerPerson { get; set; }
        public decimal? ExcessBuyBackPercentage { get; set; }
        public decimal? RoadsideAssistancePercentage { get; set; }
        public decimal? MedicalExpensesPercentage { get; set; }
        public decimal? ExcessAmount { get; set; }
        public DateTime RenewalDate { get; set; }
        public DateTime TransactionDate { get; set; }
        [Required(ErrorMessage = "Please Select Payment Term.")]
        public int PaymentTermId { get; set; }
        [Required(ErrorMessage = "Please Select A Product.")]
        public int ProductId { get; set; }
        public string InsuranceId { get; set; }
        public decimal? AnnualRiskPremium { get; set; }
        public decimal? TermlyRiskPremium { get; set; }
        public decimal? QuaterlyRiskPremium { get; set; }
        public decimal? Discount { get; set; }
        public decimal? BalanceAmount { get; set; }

    }
}
