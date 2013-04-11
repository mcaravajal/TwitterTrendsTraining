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

namespace Twitter_trends
{
    public partial class TrendTopic : PhoneApplicationPage
    {
        int Timeout = 0;
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
        #endregion
        string TrendingTopic = "TrendingTopic";
        public TrendTopic()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            trends = this.LoadState<ObservableCollection<Trend>>(TrendingTopic);
            if (trends == null)
            {

                trends = new ObservableCollection<Trend>();
                IsLoading.Visibility = Visibility.Visible;
                ApiTrendService.GetTrends(
                    //When it finish the loading
                    delegate(IEnumerable<Trend> results)
                    {
                        foreach (Trend x in results)
                        {
                            if (x.slug==null)
                            {
                                x.slug = HttpUtility.UrlEncode(x.name);
                                x.TittleTrend = x.name;
                                x.Header = x.trend_index.ToString();
                            }
                            trends.Add(x);
                        }
                    },
                    //If something is wrong
                    delegate(Exception ex)
                    {
                        IsLoading.Visibility = Visibility.Collapsed;
                    },
                    //Check if the results are Ok
                    delegate()
                    {
                        if (trends == null)
                        {
                            if (Timeout >= 0)
                            {
                                Timeout = 0;
                                if (NetworkInterface.GetIsNetworkAvailable())
                                {
                                    MessageBox.Show("No network connection available please connect and try again", "ERROR", MessageBoxButton.OK);
                                    NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                                }
                                else
                                {
                                    MessageBox.Show("An unexpected error occurred", "ERROR", MessageBoxButton.OK);
                                    NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                                }
                            }
                            else
                            {
                                Timeout++;
                                //Call this function again until we get the results or the timeout reach the limit
                                OnNavigatedTo(null);
                            }
                        }
                        else
                            IsLoading.Visibility = Visibility.Collapsed;
                    });
            }
        }
        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            this.SaveState(TrendingTopic,trends);
        }
        private void PivotControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void StackPanel_Tap(object sender, GestureEventArgs e)
        {
            TextBlock block = sender as TextBlock;
            Trend selected = block.DataContext as Trend;
            TwitPage.GlobalTrends = trends;
            TwitPage.GlobalSelectedTrend = selected;
            NavigationService.Navigate(new Uri("/Views/TwitPage.xaml",UriKind.Relative));
        }
    }
}