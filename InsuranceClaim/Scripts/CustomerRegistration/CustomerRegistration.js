﻿$(document).ready(function () {
})

function GoToProductDetail(json) {
    debugger

    if (json.IsError == true) {
       window.location.href = '/CustomerRegistration/ProductDetail';
    }
    else {
        var errorMessage = json.error;
        if (errorMessage != null && errorMessage != '') {
            toastr.error(errorMessage)
            if (errorMessage == "Sucessfully update")
            {
                window.location.href = '/Account/QuotationList';
            }
            
        }
    }
}

function GoToRiskDetail(json) {   
    debugger;
    if (json.Status == true) {
        window.location.href = '/CustomerRegistration/RiskDetail/' + json.Id;
    }
    else {
        toastr.error(json.Message);
    }
}

function GoToSummaryDetail(json) {
    if (json == true) {
        window.location.href = '/CustomerRegistration/SummaryDetail';
    }
   
}

function GoToPaymentDetail(json) {
    window.location.href = '/CustomerRegistration/PaymentDetail';
}