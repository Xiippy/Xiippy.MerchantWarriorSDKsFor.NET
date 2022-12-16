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

namespace MerchantWarriorIntegrations.Utils
{
    public class HashBiz
    {
        public static string CalculatePartnerQueryHash(string apiPassphrase, string partnerUUID, string requestReference)
        {
            string S1 = MD5(apiPassphrase) + partnerUUID + requestReference;
            S1 = S1.ToLower();
            return MD5(S1);
        }
        public static string CalculatePartnerTypeHash(string apiPassphrase, string partnerUUID, string merchantCompanyLegalName, string merchantCompanyRegNumber)
        {
            string S1 = MD5(apiPassphrase) + partnerUUID + merchantCompanyLegalName + merchantCompanyRegNumber;
            S1 = S1.ToLower();
            return MD5(S1);
        }
        public static string CalculateTransactionTypeHash(string apiPassphrase, string merchantUUID, string transactionAmount, string transactionCurrency)
        {
            string S1 = MD5(apiPassphrase) + merchantUUID + transactionAmount + transactionCurrency;
            S1 = S1.ToLower();
            return MD5(S1);
        }
        public static string MD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}
