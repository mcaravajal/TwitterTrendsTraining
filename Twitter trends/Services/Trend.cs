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
        private ObservableCollection<Twit> _twits;
        
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

        /// <summary>
        /// Gets or sets the twits.
        /// </summary>
        /// <value>The twits.</value>
        public ObservableCollection<Twit> Twits
        {
            get { return _twits; }
            set
            {
                if (_twits != value)
                {
                    _twits = value;
                    NotifyPropertyChanged("Twits");
                }
            }
        }
        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        #region Property Changed

        /// <summary>
        /// Notifies the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public bool exist(List<Trend> obj)
        {
            foreach (Trend x in obj)
            {
                    if (x.slug==this.slug)
                    {
                        return true;
                    }
            }
            return false;
        }
        #endregion
    }
    #endregion
    #region Results
    /// <summary>
    /// Model for trends results
    /// </summary>
    [DataContract]
    public class TrendsResults
    {
        [DataMember]
        string Nextpage { get; set; }
        [DataMember]
        string Previouspage { get; set; }
        [DataMember]
        public Trend[] trends { get; set; }
    }
    #endregion
}
