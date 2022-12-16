// *******************************************************************************************
// Copyright © 2019 Xiippy.ai. All rights reserved. Australian patents awarded. PCT patent pending.
//
// NOTES:
//
// - No payment gateway SDK function is consumed directly. Interfaces are defined out of such interactions and then the interface is implemented for payment gateways. Design the interface with the most common members and data structures between different gateways. 
// - A proper factory or provider must instantiate an instance of the interface that is interacted with.
// - Any major change made to SDKs should begin with the c sharp SDK with the mindset to keep the high-level syntax, structures and class names the same to minimise porting efforts to other languages. Do not use language specific features that do not exist in other languages. We are not in the business of doing the same thing from scratch multiple times in different forms.
// - Pascal Case for naming conventions should be used for all languages
// - No secret or passwords or keys must exist in the code when checked in
//
// *******************************************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using MerchantWarriorIntegrations.APIModels.Base;

namespace MerchantWarriorIntegrations.APIModels
{
    // send to https://api.merchantwarrior.com/payframe/
    public class processAuthMWPayframeReq : BaseMWRequest
    {
        public override string method { get; } = "processAuth";
        public string merchantUUID { get; set; }
        public string apiKey { get; set; }
        public string transactionAmount { get; set; }
        public string transactionCurrency { get; set; }
        public string transactionProduct { get; set; }
        public string customerName { get; set; }

        public string customerCountry { get; set; }
        public string customerState { get; set; }
        public string customerCity { get; set; }
        public string customerAddress { get; set; }
        public string customerPostCode { get; set; }
        public string payframeToken { get; set; }
        public string payframeKey { get; set; }
     

        #region optional
        public string transactionReferenceID { get; set; }
        public string threeDSToken { get; set; }
        public string customerPhone { get; set; }
        public string customerEmail { get; set; }
        public string customerIP { get; set; }
        public string storeID { get; set; }



        public string addCard { get; set; } = "0";
        public string custom1 { get; set; }
        public string custom2 { get; set; }
        public string custom3 { get; set; }


        #endregion optional
    }
}
