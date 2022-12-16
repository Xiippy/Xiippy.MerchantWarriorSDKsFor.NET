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
    public class addMerchantReq: BaseMWRequest
    {
        public override string method { get; } = "addMerchant";

        public string partnerUUID { get; set; }
        public string apiKey { get; set; }
        public string merchantCompanyLegalName { get; set; }
        public string merchantCompanyTradingName { get; set; }
        public string merchantCompanyRegNumber { get; set; }
        public string merchantIndustry { get; set; }
        public string merchantWebsite { get; set; }
        public string merchantCountry { get; set; }
        public string merchantState { get; set; }
        public string merchantCity { get; set; }
        public string merchantStreet { get; set; }
        public string merchantPostcode { get; set; }
        public string merchantTimezone { get; set; }
        public string merchantContactName { get; set; }
        public string merchantContactEmail { get; set; }
        public string merchantContactPhone { get; set; }
        public string merchantContactMobile { get; set; }
        public string merchantBillingContactName { get; set; }
        public string merchantBillingContactEmail { get; set; }


        #region optional fields
        public string ddSettlementAccountName { get; set; }
        public string ddSettlementAccountBSB { get; set; }
        public string ddSettlementAccountNumber { get; set; }
        public string ccSettlementAccountName { get; set; }
        public string ccSettlementAccountBSB { get; set; }
        public string ccSettlementAccountNumber { get; set; }
        public string cardAcceptorName { get; set; }
        public string merchantCategoryCode { get; set; }
        public string fileAttachments { get; set; }
        public string status { get; set; }
        #endregion optional fields
    }
}
