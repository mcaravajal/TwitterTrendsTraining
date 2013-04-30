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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Twitter_trends.Services
{
    #region Trend
    /// <summary>
    /// Model for twitter trend
    /// </summary>
    [DataContract]
    public class Trend : INotifyPropertyChanged
    {
        private TwitterResults _TwitResults;
        public TwitterResults TwitResults {
            get
            {
                return _TwitResults;
            }
            set
            {
                _TwitResults = value;
                Twits = _TwitResults.results;
            }
        }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [DataMember]
        public string name { get; set; }
        
        [DataMember]
        public string Header { get; set; }
        /// <summary>
        /// Gets or sets the newly_trending.
        /// </summary>
        /// <value>The newly_trending.</value>
        [DataMember]
        public int trend_index { get; set; }

        [DataMember]
        public string place_name { get; set; }

        /// <summary>
        /// Gets or sets the slug.
        /// </summary>
        /// <value>The trend encoded for a search.</value>
        [DataMember]
        public string slug { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Trend"/> is locked.
        /// </summary>
        /// <value><c>true</c> if locked; otherwise, <c>false</c>.</value>
        
        [DataMember]
        public string TittleTrend { get; set; }

        public bool exist(List<Trend> obj)
        {
            foreach (Trend x in obj)
            {
                    if (x.slug==this.slug && x.TwitResults.page==this.TwitResults.page && x.name==this.name)
                    {
                        return true;
                    }
            }
            return false;
        }
        public Trend Find(List<Trend> obj)
        {
            foreach (Trend x in obj)
            {
                if (x.slug==this.slug && x.TwitResults.page==this.TwitResults.page)
                {
                    return x;
                }
            }
            return null;
        }
        #endregion
        private ObservableCollection<Twit> _Twits;
        public ObservableCollection<Twit> Twits
        {
            get { return _Twits; }
            set
            {
                if (_Twits!=value)
                {
                    _Twits = value;
                    NotifyPropertyChanged("Twits");
                }
            }
        }
        #region Property Changed
        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        #endregion
    }
    #region Results
    /// <summary>
    /// Model for trends results
    /// </summary>
    [DataContract]
    public class TrendsResults
    {
        [DataMember]
        public Trend[] trends { get; set; }
    }
    #endregion
}
