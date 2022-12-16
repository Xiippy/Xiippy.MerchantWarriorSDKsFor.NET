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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace MerchantWarriorIntegrations.Serialization
{
   public static class ObjectToDictionary
    {
        public static System.Collections.Specialized.NameValueCollection MapToDictionary(object source/*, string name*/)
        {
            var dictionary = new System.Collections.Specialized.NameValueCollection();
            MapToDictionaryInternal(dictionary, source/*, name*/);
            return dictionary;
        }

        private static void MapToDictionaryInternal(System.Collections.Specialized.NameValueCollection dictionary, object source)
        {
            var properties = source.GetType().GetProperties();
            foreach (var p in properties)
            {
                var key =/* name + "." +*/ p.Name;
                object value = p.GetValue(source, null);
                if (value != null)
                {


                    Type valueType = value.GetType();

                    if (valueType.IsPrimitive || valueType == typeof(String))
                    {
                        dictionary.Add(key, value.ToString());
                    }
                    else if (value is IEnumerable)
                    {
                        var i = 0;
                        foreach (object o in (IEnumerable)value)
                        {
                            MapToDictionaryInternal(dictionary, o/*, key + "[" + i + "]"*/);
                            i++;
                        }
                    }
                    else
                    {
                        MapToDictionaryInternal(dictionary, value/*, key*/);
                    }
                }
            }
        }
    }
}
