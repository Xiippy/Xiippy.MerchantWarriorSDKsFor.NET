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
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using MerchantWarriorIntegrations.APIModels;
using MerchantWarriorIntegrations.Common;
using MerchantWarriorIntegrations.Exceptions;
using MerchantWarriorIntegrations.Serialization;
using MerchantWarriorIntegrations.Utils;
using Newtonsoft.Json;

namespace MerchantWarriorIntegrations.APIClient
{
    public class MerchantWarriorAPIClient
    {
        private const string HeaderMWv2Key = "MW-API-VERSION";
        private const string HeaderMWv2Value = "2.0";
        private const string ApplicationJson = "application/json";
        private string BaseUrl4Sandbox;
        private string BaseUrl4Production;
        private static readonly HttpClient client = new HttpClient();
        bool IsTest;


        public MerchantWarriorAPIClient(bool _IsTest, string _BaseUrl4Sandbox = Constants.BaseUrl4Sandbox, string _BaseUrl4Production = Constants.BaseUrl4Production)
        {
            IsTest = _IsTest;
            BaseUrl4Sandbox = _BaseUrl4Sandbox;
            BaseUrl4Production = _BaseUrl4Production;

        }


        public async Task<addMerchantRes> addMerchantAsync(MerchantWarriorPartnerConfigSet merchantWarriorPartnerConfigSet, addMerchantReq req, CancellationToken cancellationToken = default)
        {


            req.hash = HashBiz.CalculatePartnerTypeHash(merchantWarriorPartnerConfigSet.PartnerAPIPassphrase, merchantWarriorPartnerConfigSet.PartnerUUID, req.merchantCompanyLegalName, req.merchantCompanyRegNumber);
            req.apiKey = merchantWarriorPartnerConfigSet.PartnerAPIKey;
            req.partnerUUID = merchantWarriorPartnerConfigSet.PartnerUUID;

            string BaseUrl = IsTest ? BaseUrl4Sandbox : BaseUrl4Production;


            var resInStr = JsonConvert.SerializeObject(req, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            var httpContent = new StringContent(resInStr, Encoding.UTF8, ApplicationJson);

            // deal with headers
            client.DefaultRequestHeaders.Remove("Accept");
            client.DefaultRequestHeaders.Remove(HeaderMWv2Key);
            client.DefaultRequestHeaders.Add(HeaderMWv2Key, HeaderMWv2Value);
            // this is critical!
            httpContent.Headers.ContentType = new MediaTypeHeaderValue(ApplicationJson);
            httpContent.Headers.Add(HeaderMWv2Key, HeaderMWv2Value);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ApplicationJson));


            var response = await client.PostAsync($"{BaseUrl}{Constants.addMerchantPath}", httpContent, cancellationToken);
            var responseString = await response.Content.ReadAsStringAsync();
            bool isxml = !string.IsNullOrEmpty(responseString) && responseString.StartsWith("<");
            var returnedObj = isxml ? XMLConvert.DeserializeObject<addMerchantRes>(responseString) : JsonConvert.DeserializeObject<addMerchantRes>(responseString);

            if (returnedObj != null)
            {
                if (returnedObj.responseCode < 0)
                {
                    throw new MWValidationErrorException(returnedObj.responseMessage);
                }
                else if (returnedObj.responseCode > 0)
                {
                    throw new OperationWasDeclinedOrDelayedByTheProviderOrServiceException(returnedObj.responseMessage);
                }

            }

            return returnedObj;
        }

        public async Task<checkMerchantRes> checkMerchantAsync(MerchantWarriorPartnerConfigSet merchantWarriorPartnerConfigSet, checkMerchantReq req, CancellationToken cancellationToken = default)
        {
            req.hash = HashBiz.CalculatePartnerQueryHash(merchantWarriorPartnerConfigSet.PartnerAPIPassphrase, merchantWarriorPartnerConfigSet.PartnerUUID, req.requestReference);
            req.apiKey = merchantWarriorPartnerConfigSet.PartnerAPIKey;
            req.partnerUUID = merchantWarriorPartnerConfigSet.PartnerUUID;

            var httpContent = new StringContent(JsonConvert.SerializeObject(req, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            }), Encoding.UTF8, ApplicationJson);

            // deal with headers
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Remove(HeaderMWv2Key);
            client.DefaultRequestHeaders.Add(HeaderMWv2Key, HeaderMWv2Value);
            // this is critical!
            httpContent.Headers.ContentType = new MediaTypeHeaderValue(ApplicationJson);
            httpContent.Headers.Add(HeaderMWv2Key, HeaderMWv2Value);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ApplicationJson));

            string BaseUrl = IsTest ? BaseUrl4Sandbox : BaseUrl4Production;

            var response = await client.PostAsync($"{BaseUrl}{Constants.checkMerchantPath}", httpContent, cancellationToken);
            var responseString = await response.Content.ReadAsStringAsync();
            bool isxml = !string.IsNullOrEmpty(responseString) && responseString.StartsWith("<");
            var returnedObj = isxml ? XMLConvert.DeserializeObject<checkMerchantRes>(responseString) : JsonConvert.DeserializeObject<checkMerchantRes>(responseString);
            if (returnedObj != null)
            {
                if (returnedObj.responseCode < 0)
                {
                    throw new MWValidationErrorException(returnedObj.responseMessage);
                }
                else if (returnedObj.responseCode > 0)
                {
                    throw new OperationWasDeclinedOrDelayedByTheProviderOrServiceException(returnedObj.responseMessage);
                }

            }
            return returnedObj;
        }




        public async Task<processAuthRes> processAuthAsync(MerchantWarriorMerchantConfigSet merchantWarriorMerchantConfigSet, processAuthReq req, CancellationToken cancellationToken = default)
        {


            req.hash = HashBiz.CalculateTransactionTypeHash(merchantWarriorMerchantConfigSet.MerchantAPIPassphrase, merchantWarriorMerchantConfigSet.MerchantUUID, req.transactionAmount, req.transactionCurrency);
            req.merchantUUID = merchantWarriorMerchantConfigSet.MerchantUUID;
            req.apiKey = merchantWarriorMerchantConfigSet.MerchantAPIKey;


            string reqInStr = JsonConvert.SerializeObject(req, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Converters = new List<JsonConverter> { new FloatCurrencyJsonConverter() }
            });

            var httpContent = new StringContent(reqInStr, Encoding.UTF8, ApplicationJson);

            // deal with headers
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Remove(HeaderMWv2Key);
            client.DefaultRequestHeaders.Add(HeaderMWv2Key, HeaderMWv2Value);
            // this is critical!
            httpContent.Headers.ContentType = new MediaTypeHeaderValue(ApplicationJson);
            httpContent.Headers.Add(HeaderMWv2Key, HeaderMWv2Value);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ApplicationJson));

            string BaseUrl = IsTest ? BaseUrl4Sandbox : BaseUrl4Production;

            var response = await client.PostAsync($"{BaseUrl}{Constants.processAuthPath}", httpContent, cancellationToken);
            var responseString = await response.Content.ReadAsStringAsync();
            bool isxml = !string.IsNullOrEmpty(responseString) && responseString.StartsWith("<");

            var returnedObj = isxml ? XMLConvert.DeserializeObject<processAuthRes>(responseString) : JsonConvert.DeserializeObject<processAuthRes>(responseString);
            if (returnedObj != null)
            {
                if (returnedObj.responseCode < 0)
                {
                    throw new MWValidationErrorException(returnedObj.responseMessage);
                }
                else if (returnedObj.responseCode > 0)
                {
                    throw new OperationWasDeclinedOrDelayedByTheProviderOrServiceException(returnedObj.responseMessage);
                }

            }
            return returnedObj;
        }



        public async Task<processCardRes> processCardAsync(MerchantWarriorMerchantConfigSet merchantWarriorMerchantConfigSet, processCardReq req, CancellationToken cancellationToken = default)
        {
            req.hash = HashBiz.CalculateTransactionTypeHash(merchantWarriorMerchantConfigSet.MerchantAPIPassphrase, merchantWarriorMerchantConfigSet.MerchantUUID, req.transactionAmount, req.transactionCurrency);
            req.merchantUUID = merchantWarriorMerchantConfigSet.MerchantUUID;
            req.apiKey = merchantWarriorMerchantConfigSet.MerchantAPIKey;

            string reqInStr = JsonConvert.SerializeObject(req, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Converters = new List<JsonConverter> { new FloatCurrencyJsonConverter() }
            });
            var httpContent = new StringContent(reqInStr, Encoding.UTF8, ApplicationJson);

            // deal with headers
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Remove(HeaderMWv2Key);
            client.DefaultRequestHeaders.Add(HeaderMWv2Key, HeaderMWv2Value);
            // this is critical!
            httpContent.Headers.ContentType = new MediaTypeHeaderValue(ApplicationJson);
            httpContent.Headers.Add(HeaderMWv2Key, HeaderMWv2Value);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ApplicationJson));

            string BaseUrl = IsTest ? BaseUrl4Sandbox : BaseUrl4Production;

            var response = await client.PostAsync($"{BaseUrl}{Constants.processCardPath}", httpContent, cancellationToken);
            var responseString = await response.Content.ReadAsStringAsync();
            bool isxml = !string.IsNullOrEmpty(responseString) && responseString.StartsWith("<");

            var returnedObj = isxml ? XMLConvert.DeserializeObject<processCardRes>(responseString) : JsonConvert.DeserializeObject<processCardRes>(responseString);
            if (returnedObj != null)
            {
                if (returnedObj.responseCode < 0)
                {
                    throw new MWValidationErrorException(returnedObj.responseMessage);
                }
                else if (returnedObj.responseCode > 0)
                {
                    throw new OperationWasDeclinedOrDelayedByTheProviderOrServiceException(returnedObj.responseMessage);
                }

            }
            return returnedObj;
        }



        public async Task<processCardMWPayframeRes> processCardMWPayframeAsync(MerchantWarriorMerchantConfigSet merchantWarriorMerchantConfigSet, processCardMWPayframeReq req, CancellationToken cancellationToken = default)
        {
            req.hash = HashBiz.CalculateTransactionTypeHash(merchantWarriorMerchantConfigSet.MerchantAPIPassphrase, merchantWarriorMerchantConfigSet.MerchantUUID, req.transactionAmount, req.transactionCurrency);
            req.merchantUUID = merchantWarriorMerchantConfigSet.MerchantUUID;
            req.apiKey = merchantWarriorMerchantConfigSet.MerchantAPIKey;


            string reqInStr = JsonConvert.SerializeObject(req, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Converters = new List<JsonConverter> { new FloatCurrencyJsonConverter() }
            });

            var httpContent = new StringContent(reqInStr, Encoding.UTF8, ApplicationJson); ;

            // deal with headers
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Remove(HeaderMWv2Key);
            client.DefaultRequestHeaders.Add(HeaderMWv2Key, HeaderMWv2Value);
            // this is critical!
            httpContent.Headers.ContentType = new MediaTypeHeaderValue(ApplicationJson);
            httpContent.Headers.Add(HeaderMWv2Key, HeaderMWv2Value);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ApplicationJson));

            string BaseUrl = IsTest ? BaseUrl4Sandbox : BaseUrl4Production;

            var response = await client.PostAsync($"{BaseUrl}{Constants.processCardMWPayframePath}", httpContent, cancellationToken);
            var responseString = await response.Content.ReadAsStringAsync();
            bool isxml = !string.IsNullOrEmpty(responseString) && responseString.StartsWith("<");
            processCardMWPayframeRes returnedObj = isxml ? XMLConvert.DeserializeObject<processCardMWPayframeRes>(responseString) : JsonConvert.DeserializeObject<processCardMWPayframeRes>(responseString);
            if (returnedObj != null)
            {
                if (returnedObj.responseCode < 0)
                {
                    throw new MWValidationErrorException(returnedObj.responseMessage);
                }
                else if (returnedObj.responseCode > 0)
                {
                    throw new OperationWasDeclinedOrDelayedByTheProviderOrServiceException(returnedObj.responseMessage);
                }

            }
            return returnedObj;
        }



        public async Task<processAuthMWPayframeRes> processAuthMWPayframeAsync(MerchantWarriorMerchantConfigSet merchantWarriorMerchantConfigSet, processAuthMWPayframeReq req,  CancellationToken cancellationToken = default)
        {
            req.hash = HashBiz.CalculateTransactionTypeHash(merchantWarriorMerchantConfigSet.MerchantAPIPassphrase, merchantWarriorMerchantConfigSet.MerchantUUID, req.transactionAmount, req.transactionCurrency);
            req.merchantUUID = merchantWarriorMerchantConfigSet.MerchantUUID;
            req.apiKey = merchantWarriorMerchantConfigSet.MerchantAPIKey;

            string reqInStr = JsonConvert.SerializeObject(req, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Converters = new List<JsonConverter> { new FloatCurrencyJsonConverter() }
            });

            var httpContent = new StringContent(reqInStr, Encoding.UTF8, ApplicationJson); ;

            // deal with headers
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Remove(HeaderMWv2Key);
            client.DefaultRequestHeaders.Add(HeaderMWv2Key, HeaderMWv2Value);
            // this is critical!
            httpContent.Headers.ContentType = new MediaTypeHeaderValue(ApplicationJson);
            httpContent.Headers.Add(HeaderMWv2Key, HeaderMWv2Value);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ApplicationJson));

            string BaseUrl = IsTest ? BaseUrl4Sandbox : BaseUrl4Production;

            var response = await client.PostAsync($"{BaseUrl}{Constants.processAuthMWPayframePath}", httpContent, cancellationToken);
            var responseString = await response.Content.ReadAsStringAsync();
            bool isxml = !string.IsNullOrEmpty(responseString) && responseString.StartsWith("<");
            processAuthMWPayframeRes returnedObj = isxml ? XMLConvert.DeserializeObject<processAuthMWPayframeRes>(responseString) : JsonConvert.DeserializeObject<processAuthMWPayframeRes>(responseString);
            if (returnedObj != null)
            {
                if (returnedObj.responseCode < 0)
                {
                    throw new MWValidationErrorException(returnedObj.responseMessage);
                }
                else if (returnedObj.responseCode > 0)
                {
                    throw new OperationWasDeclinedOrDelayedByTheProviderOrServiceException(returnedObj.responseMessage);
                }

            }
            return returnedObj;
        }


        public async Task<processCaptureRes> processCaptureAsync(MerchantWarriorMerchantConfigSet merchantWarriorMerchantConfigSet, processCaptureReq req,  CancellationToken cancellationToken = default)
        {
            req.hash = HashBiz.CalculateTransactionTypeHash(merchantWarriorMerchantConfigSet.MerchantAPIPassphrase, merchantWarriorMerchantConfigSet.MerchantUUID, req.transactionAmount, req.transactionCurrency);

            req.merchantUUID = merchantWarriorMerchantConfigSet.MerchantUUID;
            req.apiKey = merchantWarriorMerchantConfigSet.MerchantAPIKey;

            string reqInStr = JsonConvert.SerializeObject(req, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Converters = new List<JsonConverter> { new FloatCurrencyJsonConverter() }
            });

            var httpContent = new StringContent(reqInStr, Encoding.UTF8, ApplicationJson); ;

            // deal with headers
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Remove(HeaderMWv2Key);
            client.DefaultRequestHeaders.Add(HeaderMWv2Key, HeaderMWv2Value);
            // this is critical!
            httpContent.Headers.ContentType = new MediaTypeHeaderValue(ApplicationJson);
            httpContent.Headers.Add(HeaderMWv2Key, HeaderMWv2Value);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ApplicationJson));

            string BaseUrl = IsTest ? BaseUrl4Sandbox : BaseUrl4Production;

            var response = await client.PostAsync($"{BaseUrl}{Constants.processCapturePath}", httpContent, cancellationToken);
            var responseString = await response.Content.ReadAsStringAsync();
            bool isxml = !string.IsNullOrEmpty(responseString) && responseString.StartsWith("<");
            processCaptureRes returnedObj = isxml ? XMLConvert.DeserializeObject<processCaptureRes>(responseString) : JsonConvert.DeserializeObject<processCaptureRes>(responseString);
            if (returnedObj != null)
            {
                if (returnedObj.responseCode < 0)
                {
                    throw new MWValidationErrorException(returnedObj.responseMessage);
                }
                else if (returnedObj.responseCode > 0)
                {
                    throw new OperationWasDeclinedOrDelayedByTheProviderOrServiceException(returnedObj.responseMessage);
                }

            }
            return returnedObj;
        }
    }
}
