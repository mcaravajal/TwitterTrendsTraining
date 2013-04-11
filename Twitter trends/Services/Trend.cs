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
    #region TrendDescription
    /// <summary>
    /// Model for twitter trend description
    /// </summary>
    [DataContract]
    public class TrendDescription
    {
        /// <summary>
        /// Gets or sets the created_at.
        /// </summary>
        /// <value>The created_at.</value>
        [DataMember]
        public DateTime created_at { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        [DataMember]
        public string text { get; set; }

        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        /// <value>The score.</value>
        [DataMember]
        public int score { get; set; }
    }
    #endregion
    #region Trend
    /// <summary>
    /// Model for twitter trend
    /// </summary>
    [DataContract]
    public class Trend : INotifyPropertyChanged
    {
        private ObservableCollection<Twit> _twits;
        
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [DataMember]
        public TrendDescription description { get; set; }

        /// <summary>
        /// Gets or sets the first_trended_at.
        /// </summary>
        /// <value>The first_trended_at.</value>
        [DataMember]
        public DateTime first_trended_at { get; set; }
        /// <summary>
        /// Gets or sets the last_trended_at.
        /// </summary>
        /// <value>The last_trended_at.</value>
        [DataMember]
        public DateTime last_trended_at { get; set; }
        
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
        public int newly_trending { get; set; }

        /// <summary>
        /// Gets or sets the query.
        /// </summary>
        /// <value>The query.</value>
        [DataMember]
        public string query { get; set; }

        /// <summary>
        /// Gets or sets the trend_index.
        /// </summary>
        /// <value>The trend_index.</value>
        [DataMember]
        public int trend_index { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        [DataMember]
        public string url { get; set; }

        /// <summary>
        /// Gets or sets the category_id.
        /// </summary>
        /// <value>The category_id.</value>
        [DataMember]
        public int category_id { get; set; }

        /// <summary>
        /// Gets or sets the category_name.
        /// </summary>
        /// <value>The category_name.</value>
        [DataMember]
        public string category_name { get; set; }
        /// <summary>
        /// Gets or sets the place_name.
        /// </summary>
        /// <value>The place_name.</value>
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
        public bool locked { get; set; }
        
        [DataMember]
        public string TittleTrend { get; set; }
        /*/// <summary>
        /// Gets the indexed name of the trend.
        /// </summary>
        /// <value>The indexed name of the trend.</value>
        public string IndexedTrendName
        {
            get
            {
                return string.Format("#{0} {1}", trend_index, name);
            }
        }*/
        
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
        /*public override bool Equals(object obj)
        {
            Trend other = obj as Trend;

            if (other == null) return false;

            // two trends are equal if their names are equal
            return name.Equals(other.name);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return name.GetHashCode();
        }*/

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
        /// <summary>
        /// Gets or sets the api_version.
        /// </summary>
        /// <value>The api_version.</value>
        [DataMember]
        public string api_version { get; set; }

        /// <summary>
        /// Gets or sets the as_of.
        /// </summary>
        /// <value>The as_of.</value>
        [DataMember]
        public string as_of { get; set; }

        /// <summary>
        /// Gets or sets the trends.
        /// </summary>
        /// <value>The trends.</value>
        [DataMember]
        public Trend[] trends { get; set; }
    }
    #endregion
}
