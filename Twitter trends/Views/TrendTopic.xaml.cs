using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Twitter_trends.Services;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.IsolatedStorage;

namespace Twitter_trends
{
    public partial class TrendTopic : PhoneApplicationPage
    {
        #region Timeout
        int Timeout = 0;
        #endregion
        #region Trends
        /// <summary>
        /// Trends Dependency Property
        /// </summary>
        public static readonly DependencyProperty TrendsProperty =
            DependencyProperty.Register("Trends", typeof(ObservableCollection<Trend>), typeof(TrendTopic),
                new PropertyMetadata((ObservableCollection<Trend>)null));

        /// <summary>
        /// Gets or sets the Trends property. This dependency property 
        /// indicates the current twitter trends.
        /// </summary>
        public ObservableCollection<Trend> trends
        {
            get { return (ObservableCollection<Trend>)GetValue(TrendsProperty); }
            set { SetValue(TrendsProperty, value); }
        }
        public static ObservableCollection<Trend> GlobalTrends;
        #endregion
        #region IsLoading
        public static readonly DependencyProperty IsLoadingProp =
            DependencyProperty.Register("IsLoading", typeof(bool), typeof(TrendTopic),
            new PropertyMetadata((bool)false));
        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProp); }
            set { SetValue(IsLoadingProp, value); }
        }
        #endregion
        string TrendingTopic = "TrendingTopic";
        public TrendTopic()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            trends = this.LoadState<ObservableCollection<Trend>>(TrendingTopic);
            if (trends == null && GlobalTrends == null)
            {
                trends = new ObservableCollection<Trend>();
                IsLoading = true;
                ApiTrendService.GetTrends(
                    (results) =>
                    {
                        Dispatcher.BeginInvoke(() =>
                        {
                            foreach (Trend x in results)
                            {
                                if (x.slug == null)
                                {
                                    x.slug = HttpUtility.UrlEncode(x.name);
                                    x.TittleTrend = x.name;
                                    x.Header = x.trend_index.ToString();
                                }
                                trends.Add(x);
                            }
                        });
                    },
                    (ex) =>
                    {
                        IsLoading = false;
                    },
                    () =>
                    {
                        Dispatcher.BeginInvoke(() => IsLoading = false);
                    });
            }
            else
            {
                if (GlobalTrends != null)
                    trends = GlobalTrends;

            }
        }
        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            this.SaveState(TrendingTopic,trends);
        }
        private void StackPanel_Tap(object sender, GestureEventArgs e)
        {
            TextBlock block = sender as TextBlock;
            Trend selected = block.DataContext as Trend;
            TwitPage.GlobalTrends = trends;
            TwitPage.GlobalSelectedTrend = selected;
            TwitPage.Action = 2;
            this.GoToPage(ApplicationPages.Twitter);
        }
    }
}