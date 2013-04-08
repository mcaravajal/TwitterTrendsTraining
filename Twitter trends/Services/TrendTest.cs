using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Twitter_trends.Services
{
    /// <summary>
    /// Model for twitter trend description
    /// </summary>
    [DataContract]
    public class TrendDescriptionTest
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

    [DataContract]
    public class TrendTest : INotifyPropertyChanged
    {
        private ObservableCollection<Twit> _twits;
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [DataMember]
        public TrendDescriptionTest description { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [DataMember]
        public string name { get; set; }

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
        /// Gets or sets the last_trended_at.
        /// </summary>
        /// <value>The last_trended_at.</value>
        [DataMember]
        public DateTime last_trended_at { get; set; }

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
        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return name.GetHashCode();
        }
        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            Trend other = obj as Trend;

            if (other == null) return false;

            // two trends are equal if their names are equal
            return name.Equals(other.name);
        }

    }
    [DataContract]
    public class TrendTestResult
    {
        [DataMember]
        public TrendTest[] trends { get; set; }
    }
}
