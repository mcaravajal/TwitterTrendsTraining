// ----------------------------------------------------------------------------------
// Microsoft Developer & Platform Evangelism
// 
// Copyright (c) Microsoft Corporation. All rights reserved.
// 
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// ----------------------------------------------------------------------------------
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
// ----------------------------------------------------------------------------------

using System;
using Microsoft.Phone.Controls;

namespace Twitter_trends
{
    /// <summary>
    /// State Manager
    /// </summary>
    public static class StateManager
    {
        /// <summary>
        /// Saves a key-value pair into the state object
        /// </summary>
        /// <param name="phoneApplicationPage">The phone application page.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public static void SaveState(this PhoneApplicationPage phoneApplicationPage, string key, object value)
        {
            if (phoneApplicationPage.State.ContainsKey(key))
            {
                phoneApplicationPage.State.Remove(key);
            }

            phoneApplicationPage.State.Add(key, value);
        }

        /// <summary>
        /// Loads value from the state object, according to the key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="phoneApplicationPage">The phone application page.</param>
        /// <param name="key">The key.</param>
        /// <returns>The loaded value</returns>
        public static T LoadState<T>(this PhoneApplicationPage phoneApplicationPage, string key)
            where T : class
        {
            if (phoneApplicationPage.State.ContainsKey(key))
            {
                return (T)phoneApplicationPage.State[key];
            }

            return default(T);
        }
    }
}
