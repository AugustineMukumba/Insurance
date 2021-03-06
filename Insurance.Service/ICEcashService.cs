﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using System.Configuration;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;
using InsuranceClaim.Models;
using Newtonsoft.Json;
using System.Web;
using Insurance.Domain;

namespace Insurance.Service
{
    public class ICEcashService
    {
      // public static string PSK = "127782435202916376850511";
      //  public static string SandboxIceCashApi = "http://api-test.icecash.com/request/20523588";

        public static string PSK = "565205790573235453203546";
        public static string LiveIceCashApi = "https://api.icecash.co.zw/request/20350763";
        private static string GetSHA512(string text)
        {
            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] hashValue;
            byte[] message = UE.GetBytes(text);
            SHA512Managed hashString = new SHA512Managed();
            string encodedData = Convert.ToBase64String(message);
            string hex = "";
            hashValue = hashString.ComputeHash(UE.GetBytes(encodedData));
            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }
            return hex;
        }

        public static string SHA512(string input)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(input);
            using (var hash = System.Security.Cryptography.SHA512.Create())
            {
                var hashedInputBytes = hash.ComputeHash(bytes);

                // Convert to text
                // StringBuilder Capacity is 128, because 512 bits / 8 bits in byte * 2 symbols for byte 
                var hashedInputStringBuilder = new System.Text.StringBuilder(128);
                foreach (var b in hashedInputBytes)
                    hashedInputStringBuilder.Append(b.ToString("X2"));
                return hashedInputStringBuilder.ToString();
            }
        }

        public ICEcashTokenResponse getToken()
        {

            ICEcashTokenResponse json = null;
            try
            {


                //string json = "%7B%20%20%20%22PartnerReference%22%3A%20%228eca64cb-ccf8-4304-a43f-a6eaef441918%22%2C%0A%20%20%20%20%22Date%22%3A%20%22201801080615165001%22%2C%0A%20%20%20%20%22Version%22%3A%20%222.0%22%2C%0A%20%20%20%20%22Request%22%3A%20%7B%0A%20%20%20%20%20%20%20%20%22Function%22%3A%20%22PartnerToken%22%7D%7D";
                //string PSK = "127782435202916376850511";
                string _json = "";//"{'PartnerReference':'" + Convert.ToString(Guid.NewGuid()) + "','Date':'" + DateTime.Now.ToString("yyyyMMddhhmmss") + "','Version':'2.0','Request':{'Function':'PartnerToken'}}";
                Arguments objArg = new Arguments();
                objArg.PartnerReference = Guid.NewGuid().ToString();
                objArg.Date = DateTime.Now.ToString("yyyyMMddhhmmss");
                objArg.Version = "2.0";
                objArg.Request = new FunctionObject { Function = "PartnerToken" };

                _json = Newtonsoft.Json.JsonConvert.SerializeObject(objArg);

                //string  = json.Reverse()
                string reversejsonString = new string(_json.Reverse().ToArray());
                string reversepartneridString = new string(PSK.Reverse().ToArray());

                string concatinatedString = reversejsonString + reversepartneridString;

                byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(concatinatedString);

                string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);

                string GetSHA512encrypted = SHA512(returnValue);

                string MAC = "";

                for (int i = 0; i < 16; i++)
                {
                    MAC += GetSHA512encrypted.Substring((i * 8), 1);
                }

                MAC = MAC.ToUpper();


                ICERootObject objroot = new ICERootObject();
                objroot.Arguments = objArg;
                objroot.MAC = MAC;
                objroot.Mode = "SH";

                var data = Newtonsoft.Json.JsonConvert.SerializeObject(objroot);


                JObject jsonobject = JObject.Parse(data);

                //  var client = new RestClient(SandboxIceCashApi);
                var client = new RestClient(LiveIceCashApi);
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/x-www-form-urlencoded");
                request.AddParameter("application/x-www-form-urlencoded", jsonobject, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);

                json = JsonConvert.DeserializeObject<ICEcashTokenResponse>(response.Content);

                HttpContext.Current.Session["ICEcashToken"] = json;

            }
            catch (Exception ex)
            {
                json = new ICEcashTokenResponse() { Date = "", PartnerReference = "", Version = "", Response = new TokenReposone() { Result = "0", Message = "A Connection Error Occured ! Please add manually", ExpireDate = "", Function = "", PartnerToken = "" } };
            }

            return json;
        }

        public ResultRootObject checkVehicleExists(List<RiskDetailModel> listofvehicles, string PartnerToken, string PartnerReference)
        {
            //string PSK = "127782435202916376850511";
            string _json = "";

            List<VehicleObject> obj = new List<VehicleObject>();

            var CustomerInfo = (CustomerModel)HttpContext.Current.Session["CustomerDataModal"];

            foreach (var item in listofvehicles)
            {
                obj.Add(new VehicleObject { VRN = item.RegistrationNo, DurationMonths = (item.PaymentTermId == 1 ? 12 : item.PaymentTermId), VehicleValue = 0, YearManufacture = 0, InsuranceType = 0, VehicleType = 0, TaxClass = 0, Make = "", Model = "", EntityType = "", Town = CustomerInfo.City, Address1 = CustomerInfo.AddressLine1, Address2 = CustomerInfo.AddressLine2, CompanyName = "", FirstName = CustomerInfo.FirstName, LastName = CustomerInfo.LastName, IDNumber = CustomerInfo.NationalIdentificationNumber, MSISDN = CustomerInfo.CountryCode + CustomerInfo.PhoneNumber });
            }

            QuoteArguments objArg = new QuoteArguments();
            objArg.PartnerReference = Guid.NewGuid().ToString();
            objArg.Date = DateTime.Now.ToString("yyyyMMddhhmmss");
            objArg.Version = "2.0";
            objArg.PartnerToken = PartnerToken;
            objArg.Request = new QuoteFunctionObject { Function = "TPIQuote", Vehicles = obj };

            _json = Newtonsoft.Json.JsonConvert.SerializeObject(objArg);

            //string  = json.Reverse()
            string reversejsonString = new string(_json.Reverse().ToArray());
            string reversepartneridString = new string(PSK.Reverse().ToArray());

            string concatinatedString = reversejsonString + reversepartneridString;

            byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(concatinatedString);

            string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);

            string GetSHA512encrypted = SHA512(returnValue);

            string MAC = "";

            for (int i = 0; i < 16; i++)
            {
                MAC += GetSHA512encrypted.Substring((i * 8), 1);
            }

            MAC = MAC.ToUpper();

            ICEQuoteRequest objroot = new ICEQuoteRequest();
            objroot.Arguments = objArg;
            objroot.MAC = MAC;
            objroot.Mode = "SH";

            var data = Newtonsoft.Json.JsonConvert.SerializeObject(objroot);

            JObject jsonobject = JObject.Parse(data);

            //  var client = new RestClient("http://api-test.icecash.com/request/20523588");
            var client = new RestClient(LiveIceCashApi);
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", jsonobject, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            ResultRootObject json = JsonConvert.DeserializeObject<ResultRootObject>(response.Content);

            return json;
        }

        public ResultRootObject RequestQuote(string PartnerToken, string RegistrationNo, string suminsured, string make, string model, int PaymentTermId, int VehicleYear, int CoverTypeId, int VehicleUsage, string PartnerReference)
        {
            //string PSK = "127782435202916376850511";
            string _json = "";

            var CustomerInfo = (CustomerModel)HttpContext.Current.Session["CustomerDataModal"];

            List<VehicleObject> obj = new List<VehicleObject>();

            //foreach (var item in listofvehicles)
            //{

            obj.Add(new VehicleObject { VRN = RegistrationNo, DurationMonths = (PaymentTermId == 1 ? 12 : PaymentTermId), VehicleValue = Convert.ToInt32(suminsured), YearManufacture = Convert.ToInt32(VehicleYear), InsuranceType =  Convert.ToInt32(CoverTypeId), VehicleType = Convert.ToInt32(VehicleUsage), TaxClass = 1, Make = make, Model = model, EntityType = "", Town = CustomerInfo.City, Address1 = CustomerInfo.AddressLine1, Address2 = CustomerInfo.AddressLine2, CompanyName = "", FirstName = CustomerInfo.FirstName, LastName = CustomerInfo.LastName, IDNumber = CustomerInfo.NationalIdentificationNumber, MSISDN = CustomerInfo.CountryCode + CustomerInfo.PhoneNumber });
        
            //}

            QuoteArguments objArg = new QuoteArguments();
            objArg.PartnerReference = Guid.NewGuid().ToString(); ;
            objArg.Date = DateTime.Now.ToString("yyyyMMddhhmmss");
            objArg.Version = "2.0";
            objArg.PartnerToken = PartnerToken;
            objArg.Request = new QuoteFunctionObject { Function = "TPIQuote", Vehicles = obj };

            _json = Newtonsoft.Json.JsonConvert.SerializeObject(objArg);

            //string  = json.Reverse()
            string reversejsonString = new string(_json.Reverse().ToArray());
            string reversepartneridString = new string(PSK.Reverse().ToArray());

            string concatinatedString = reversejsonString + reversepartneridString;

            byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(concatinatedString);

            string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);

            string GetSHA512encrypted = SHA512(returnValue);

            string MAC = "";

            for (int i = 0; i < 16; i++)
            {
                MAC += GetSHA512encrypted.Substring((i * 8), 1);
            }

            MAC = MAC.ToUpper();

            ICEQuoteRequest objroot = new ICEQuoteRequest();
            objroot.Arguments = objArg;
            objroot.MAC = MAC;
            objroot.Mode = "SH";

            var data = Newtonsoft.Json.JsonConvert.SerializeObject(objroot);

            JObject jsonobject = JObject.Parse(data);

           // var client = new RestClient("http://api-test.icecash.com/request/20523588");
            var client = new RestClient(LiveIceCashApi);
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", jsonobject, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            ResultRootObject json = JsonConvert.DeserializeObject<ResultRootObject>(response.Content);




            if (json.Response.Quotes != null && json.Response.Quotes.Count > 0)
            {
                if (json.Response.Quotes[0].Policy != null)
                {
                    var Setting = InsuranceContext.Settings.All();
                    var DiscountOnRenewalSettings = Setting.Where(x => x.keyname == "Discount On Renewal").FirstOrDefault();
                    var premium = Convert.ToDecimal(json.Response.Quotes[0].Policy.CoverAmount);
                    switch (PaymentTermId)
                    {
                        case 1:
                            var AnnualRiskPremium = premium;
                            if (DiscountOnRenewalSettings.ValueType == Convert.ToInt32(eSettingValueType.percentage))
                            {
                                json.LoyaltyDiscount = ((AnnualRiskPremium * Convert.ToDecimal(DiscountOnRenewalSettings.value)) / 100);
                            }
                            if (DiscountOnRenewalSettings.ValueType == Convert.ToInt32(eSettingValueType.amount))
                            {
                                json.LoyaltyDiscount = Convert.ToDecimal(DiscountOnRenewalSettings.value);
                            }
                            break;
                        case 3:
                            var QuaterlyRiskPremium = premium;
                            if (DiscountOnRenewalSettings.ValueType == Convert.ToInt32(eSettingValueType.percentage))
                            {
                                json.LoyaltyDiscount = ((QuaterlyRiskPremium * Convert.ToDecimal(DiscountOnRenewalSettings.value)) / 100);
                            }
                            if (DiscountOnRenewalSettings.ValueType == Convert.ToInt32(eSettingValueType.amount))
                            {
                                json.LoyaltyDiscount = Convert.ToDecimal(DiscountOnRenewalSettings.value);
                            }
                            break;
                        case 4:
                            var TermlyRiskPremium = premium;
                            if (DiscountOnRenewalSettings.ValueType == Convert.ToInt32(eSettingValueType.percentage))
                            {
                                json.LoyaltyDiscount = ((TermlyRiskPremium * Convert.ToDecimal(DiscountOnRenewalSettings.value)) / 100);
                            }
                            if (DiscountOnRenewalSettings.ValueType == Convert.ToInt32(eSettingValueType.amount))
                            {
                                json.LoyaltyDiscount = Convert.ToDecimal(DiscountOnRenewalSettings.value);
                            }
                            break;
                    }
                }
            }


            return json;
        }


        public static ResultRootObject TPIQuoteUpdate(Customer customer, VehicleDetail vehicleDetail, string PartnerToken, int? paymentMethod)
        {
            //string PSK = "127782435202916376850511";
            string _json = "";

            List<VehicleObject> obj = new List<VehicleObject>();

            //   var CustomerInfo = (CustomerModel)HttpContext.Current.Session["CustomerDataModal"];

            var CustomerInfo = customer;

            var item = vehicleDetail;

            if(paymentMethod==2 || paymentMethod == 3) // it's represent to visa
            {
                paymentMethod =1;
            }

            // obj.Add(new VehicleObject { VRN = item.RegistrationNo, DurationMonths = (item.PaymentTermId == 1 ? 12 : item.PaymentTermId), VehicleValue = 0, YearManufacture = 0, InsuranceType = 0, VehicleType = 0, TaxClass = 0, Make = "", Model = "", EntityType = "", Town = CustomerInfo.City, Address1 = CustomerInfo.AddressLine1, Address2 = CustomerInfo.AddressLine2, CompanyName = "", FirstName = CustomerInfo.FirstName, LastName = CustomerInfo.LastName, IDNumber = CustomerInfo.NationalIdentificationNumber, MSISDN = "01" + CustomerInfo.PhoneNumber });

            List<QuoteDetial> qut = new List<QuoteDetial>();

            qut.Add(new QuoteDetial { InsuranceID = item.InsuranceId, Status = "1" });

            var quotesDetial = new RequestTPIQuoteUpdate { Function = "TPIQuoteUpdate", PaymentMethod = Convert.ToString(paymentMethod), Identifier = "1", MSISDN = "01" + CustomerInfo.PhoneNumber, Quotes = qut };



            QuoteArgumentsTPIQuote objArg = new QuoteArgumentsTPIQuote();
            objArg.PartnerReference = Guid.NewGuid().ToString();
            objArg.Date = DateTime.Now.ToString("yyyyMMddhhmmss");
            objArg.Version = "2.0";
            objArg.PartnerToken = PartnerToken;
            objArg.Request = quotesDetial;



            _json = Newtonsoft.Json.JsonConvert.SerializeObject(objArg);

            //string  = json.Reverse()
            string reversejsonString = new string(_json.Reverse().ToArray());
            string reversepartneridString = new string(PSK.Reverse().ToArray());

            string concatinatedString = reversejsonString + reversepartneridString;

            byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(concatinatedString);

            string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);

            string GetSHA512encrypted = SHA512(returnValue);

            string MAC = "";

            for (int i = 0; i < 16; i++)
            {
                MAC += GetSHA512encrypted.Substring((i * 8), 1);
            }

            MAC = MAC.ToUpper();

            ICEQuoteRequestTPIQuote objroot = new ICEQuoteRequestTPIQuote();
            objroot.Arguments = objArg;
            objroot.MAC = MAC;
            objroot.Mode = "SH";

            var data = Newtonsoft.Json.JsonConvert.SerializeObject(objroot);

            JObject jsonobject = JObject.Parse(data);

            // var client = new RestClient("http://api-test.icecash.com/request/20523588");
            var client = new RestClient(LiveIceCashApi);
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", jsonobject, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            ResultRootObject json = JsonConvert.DeserializeObject<ResultRootObject>(response.Content);

            return json;
        }


        public static ResultRootObject TPIPolicy(VehicleDetail vehicleDetail, string PartnerToken)
        {
            //string PSK = "127782435202916376850511";
            string _json = "";

            List<VehicleObject> obj = new List<VehicleObject>();

            //   var CustomerInfo = (CustomerModel)HttpContext.Current.Session["CustomerDataModal"];


            var item = vehicleDetail;

           
            // obj.Add(new VehicleObject { VRN = item.RegistrationNo, DurationMonths = (item.PaymentTermId == 1 ? 12 : item.PaymentTermId), VehicleValue = 0, YearManufacture = 0, InsuranceType = 0, VehicleType = 0, TaxClass = 0, Make = "", Model = "", EntityType = "", Town = CustomerInfo.City, Address1 = CustomerInfo.AddressLine1, Address2 = CustomerInfo.AddressLine2, CompanyName = "", FirstName = CustomerInfo.FirstName, LastName = CustomerInfo.LastName, IDNumber = CustomerInfo.NationalIdentificationNumber, MSISDN = "01" + CustomerInfo.PhoneNumber });

            //List<QuoteDetial> qut = new List<QuoteDetial>();

            TPIPolicyDetial qut = new TPIPolicyDetial  { InsuranceID = item.InsuranceId, Function= "TPIPolicy" };

            // var quotesDetial = new RequestTPIQuoteUpdate { Function = "TPIQuoteUpdate", PaymentMethod = Convert.ToString(paymentMethod), Identifier = "1", MSISDN = "01" + CustomerInfo.PhoneNumber, Quotes = qut };



            QuoteArgumentsTPIPolicy objArg = new QuoteArgumentsTPIPolicy();
            objArg.PartnerReference = Guid.NewGuid().ToString();
            objArg.Date = DateTime.Now.ToString("yyyyMMddhhmmss");
            objArg.Version = "2.0";
            objArg.PartnerToken = PartnerToken;
            objArg.Request = qut;

            _json = Newtonsoft.Json.JsonConvert.SerializeObject(objArg);

            //string  = json.Reverse()
            string reversejsonString = new string(_json.Reverse().ToArray());
            string reversepartneridString = new string(PSK.Reverse().ToArray());

            string concatinatedString = reversejsonString + reversepartneridString;

            byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(concatinatedString);

            string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);

            string GetSHA512encrypted = SHA512(returnValue);

            string MAC = "";

            for (int i = 0; i < 16; i++)
            {
                MAC += GetSHA512encrypted.Substring((i * 8), 1);
            }

            MAC = MAC.ToUpper();

            ICEQuoteRequestTPIPolicy objroot = new ICEQuoteRequestTPIPolicy();
            objroot.Arguments = objArg;
            objroot.MAC = MAC;
            objroot.Mode = "SH";

            var data = Newtonsoft.Json.JsonConvert.SerializeObject(objroot);

            JObject jsonobject = JObject.Parse(data);

            //  var client = new RestClient("http://api-test.icecash.com/request/20523588");
            var client = new RestClient(LiveIceCashApi);
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", jsonobject, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            ResultRootObject json = JsonConvert.DeserializeObject<ResultRootObject>(response.Content);

            return json;
        }



    }

    public class Arguments
    {
        public string PartnerReference { get; set; }
        public string Date { get; set; }
        public string Version { get; set; }
        public FunctionObject Request { get; set; }
    }
    public class FunctionObject
    {
        public string Function { get; set; }
    }
    public class ICERootObject
    {
        public Arguments Arguments { get; set; }
        public string MAC { get; set; }
        public string Mode { get; set; }
    }
    public class VehicleObject
    {
        public string VRN { get; set; }
        public string IDNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MSISDN { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Town { get; set; }
        public string EntityType { get; set; }
        public string CompanyName { get; set; }
        public int DurationMonths { get; set; }
        public int VehicleValue { get; set; }
        public int InsuranceType { get; set; }
        public int VehicleType { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int TaxClass { get; set; }
        public int YearManufacture { get; set; }
    }


    public class VehicleObjectWithNullable
    {
        public string VRN { get; set; }
        public string IDNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MSISDN { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Town { get; set; }
        public string EntityType { get; set; }
        public string CompanyName { get; set; }
        public string DurationMonths { get; set; }
        public string VehicleValue { get; set; }
        public string InsuranceType { get; set; }
        public string VehicleType { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string TaxClass { get; set; }
        public string YearManufacture { get; set; }
    }

    public class QuoteArguments
    {
        public string PartnerReference { get; set; }
        public string Date { get; set; }
        public string Version { get; set; }
        public string PartnerToken { get; set; }
        public QuoteFunctionObject Request { get; set; }
    }
    public class QuoteFunctionObject
    {
        public string Function { get; set; }
        public List<VehicleObject> Vehicles { get; set; }
    }
    public class ICEQuoteRequest
    {
        public QuoteArguments Arguments { get; set; }
        public string MAC { get; set; }
        public string Mode { get; set; }
    }
    public class TokenReposone
    {
        public string Function { get; set; }
        public string Result { get; set; }
        public string Message { get; set; }
        public string PartnerToken { get; set; }
        public string ExpireDate { get; set; }
    }
    public class ICEcashTokenResponse
    {
        public string PartnerReference { get; set; }
        public string Date { get; set; }
        public string Version { get; set; }
        public TokenReposone Response { get; set; }

        public Quote Quotes { get; set; }
    }
    public class Quote
    {
        public string VRN { get; set; }
        public string InsuranceID { get; set; }
        public int Result { get; set; }
        public string Message { get; set; }
    }
    public class QuoteResponse
    {
        public int Result { get; set; }
        public string Message { get; set; }
        public List<Quote> Quotes { get; set; }
    }
    public class ICEcashQuoteResponse
    {
        public string PartnerReference { get; set; }
        public string Date { get; set; }
        public string Version { get; set; }
        public QuoteResponse Response { get; set; }
    }
    public class ResultPolicy
    {
        public string InsuranceType { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string DurationMonths { get; set; }
        public string Amount { get; set; }
        public string StampDuty { get; set; }
        public string GovernmentLevy { get; set; }
        public string CoverAmount { get; set; }
        public string PremiumAmount { get; set; }
    }
    public class ResultClient
    {
        public string IDNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MSISDN { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Town { get; set; }
        public string EntityType { get; set; }
        public string CompanyName { get; set; }
    }
    public class ResultVehicle
    {
        public string Make { get; set; }
        public string Model { get; set; }
        public string TaxClass { get; set; }
        public string YearManufacture { get; set; }
        public int VehicleType { get; set; }
        public string VehicleValue { get; set; }
    }
    public class ResultQuote
    {
        public string VRN { get; set; }
        public string InsuranceID { get; set; }
        public int Result { get; set; }
        public string Message { get; set; }
        public ResultPolicy Policy { get; set; }
        public ResultClient Client { get; set; }
        public ResultVehicle Vehicle { get; set; }
    }
    public class ResultResponse
    {
        public int Result { get; set; }
        public string Message { get; set; }
        public List<ResultQuote> Quotes { get; set; }
    }
    public class ResultRootObject
    {
        public decimal LoyaltyDiscount { get; set; }
        public string PartnerReference { get; set; }
        public string Date { get; set; }
        public string Version { get; set; }
        public ResultResponse Response { get; set; }
    }


    public class RequestTPIQuoteUpdate
    {
        public string Function { get; set; }
        public string PaymentMethod { get; set; }
        public string Identifier { get; set; }
        public string MSISDN { get; set; }
        public List<QuoteDetial> Quotes { get; set; }
    }
    public class QuoteDetial
    {
        public string InsuranceID { get; set; }

        public string Status { get; set; }

    }



    public class QuoteArgumentsTPIQuote
    {
        public string PartnerReference { get; set; }
        public string Date { get; set; }
        public string Version { get; set; }
        public string PartnerToken { get; set; }
        public RequestTPIQuoteUpdate Request { get; set; }
    }


    public class ICEQuoteRequestTPIQuote
    {
        public QuoteArgumentsTPIQuote Arguments { get; set; }
        public string MAC { get; set; }
        public string Mode { get; set; }
    }


    public class TPIPolicyDetial
    {
        public string InsuranceID { get; set; }
        public string Function { get; set; }
    }


    public class QuoteArgumentsTPIPolicy
    {
        public string PartnerReference { get; set; }
        public string Date { get; set; }
        public string Version { get; set; }
        public string PartnerToken { get; set; }
        public TPIPolicyDetial Request { get; set; }
    }


    public class ICEQuoteRequestTPIPolicy
    {
        public QuoteArgumentsTPIPolicy Arguments { get; set; }
        public string MAC { get; set; }
        public string Mode { get; set; }
    }


}
