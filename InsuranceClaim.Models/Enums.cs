﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceClaim.Models
{
    public enum eCoverType
    {

        Comprehensive = 4,
        ThirdParty = 1,
        FullThirdParty = 2


        //Comprehensive = 1,
        //ThirdParty = 2,
        //FullThirdParty = 3
    }
    public enum eExcessType
    {
        Percentage = 1,
        FixedAmount = 2
    }
    public enum ePaymentTerm
    {
        Annual = 1,
        //Monthly = 2,
      //  Quarterly = 3,
        Termly = 4
    }

    public enum eSettingValueType
    {
        percentage = 1,
        amount = 2
    }
    public enum eStatus
    {
        Quote = 1,
        InForce = 2,
        Lapsed = 3,
        NTU = 4,
        Renewed = 5
    }

    public enum ePolicyRenewReminderType
    {
        Email = 1,
        SMS = 2
    }
    public enum ePayeeBankDetails
    {
        Bank = 1,
        MobileMoney = 2,
        Cash = 3
    }
}
