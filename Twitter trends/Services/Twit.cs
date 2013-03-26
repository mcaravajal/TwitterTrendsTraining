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
using System.Runtime.Serialization;
using System.Net;

namespace Twitter_trends
{
    /// <summary>
    /// Model for twit
    /// </summary>
    [DataContract]
    public class Twit
    {
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        [DataMember]
        public string text { get; set; }

        /// <summary>
        /// Gets the decoded text.
        /// </summary>
        /// <value>The decoded text.</value>
        public string DecodedText 
        {
            get
            {
                return HttpUtility.HtmlDecode(text);
            }
        }

        /// <summary>
        /// Gets or sets the from_user.
        /// </summary>
        /// <value>The from_user.</value>
        [DataMember]
        public string from_user { get; set; }

        /// <summary>
        /// Gets or sets the profile_image_url.
        /// </summary>
        /// <value>The profile_image_url.</value>
        [DataMember]
        public string profile_image_url { get; set; }

        /// <summary>
        /// Gets or sets the created_at.
        /// </summary>
        /// <value>The created_at.</value>
        [DataMember]
        public DateTime created_at { get; set; }
    }

    /// <summary>
    /// Model for twitter results
    /// </summary>
    [DataContract]
    public class TwitterResults
    {
        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        /// <value>The results.</value>
        [DataMember]
        public Twit[] results { get; set; }
    }
}

