using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using Microsoft.Phone.Net.NetworkInformation;
using System.IO;

namespace Twitter_trends
{
    public partial class TrendTools : PhoneApplicationPage
    {
        public TrendTools()
        {
            InitializeComponent();
            FilterList = new ObservableCollection<Locations>();
        }
        #region Country
        
        public static readonly DependencyProperty CountryProperty =
            DependencyProperty.Register("CountryList", typeof(ObservableCollection<Locations>), typeof(TrendTools), new PropertyMetadata((ObservableCollection<Trend>)null));
        
        public ObservableCollection<Locations> CountryList
        {
            get { return (ObservableCollection<Locations>)GetValue(CountryProperty);}
            set { SetValue(CountryProperty, value); }
        }

#endregion
        #region Filter

        public static readonly DependencyProperty FilterProperty =
            DependencyProperty.Register("FilterList", typeof(ObservableCollection<Locations>), typeof(TrendTools), new PropertyMetadata((ObservableCollection<Trend>)null));

        public ObservableCollection<Locations> FilterList
        {
            get { return (ObservableCollection<Locations>)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }

        #endregion
        #region ResultsList
        private static readonly DependencyProperty ResultProperty =
            DependencyProperty.Register("ResultList", typeof(ObservableCollection<Twit>), typeof(TrendTools), new PropertyMetadata((ObservableCollection<Twit>)null));
        public ObservableCollection<Twit> ResultList
        {
            get { return (ObservableCollection<Twit>)GetValue(ResultProperty); }
            set { SetValue(ResultProperty,value); }
        }
        #endregion
        #region Timeouts
        int Timeout = 0;
        int TimeoutSearch = 0;
        #endregion
        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            this.SaveState("TrendTools", CountryList);
        }
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            var state= this.LoadState<ObservableCollection<Locations>>("TrendTools");
            if (state == null)
            {
                IsLoading.Visibility = Visibility.Visible;
                ApiTrendService.GetLocations(
                    delegate(IEnumerable<Locations> results)
                    {
                        Dispatcher.BeginInvoke(() =>
                            {
                                CountryList = new ObservableCollection<Locations>();
                                foreach (Locations aux in results)
                                {
                                    if (aux.place_type_code == 12)
                                        CountryList.Add(aux);
                                }
                            });
                        
                    },
                    delegate(Exception ex)
                    {
                        MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK);
                        NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                    },
                    delegate()
                    {
                        Dispatcher.BeginInvoke(() =>
                            {
                                if (CountryList == null)
                                {
                                    if (Timeout >= 5)
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
                                {
                                    Timeout = 0;
                                    IsLoading.Visibility = Visibility.Collapsed;
                                }
                            });
                    });
            }
        }
        private void LocationName_Tap(object sender, GestureEventArgs e)
        {
            Trend TrendSelected = null;
            Locations taped = ((TextBlock)sender).DataContext as Locations;
            ApiTrendService.GetSingleTrendsFromLocation(taped,
                (result) =>
                {
                    TrendSelected = result;
                    TrendSelected.place_name = taped.name;
                    TrendSelected.slug = null;
                    ObservableCollection<Trend> Trends = new ObservableCollection<Trend>();
                    Trends.Add(TrendSelected);
                    TwitPage.GlobalTrends = Trends;
                    TwitPage.GlobalSelectedTrend = TrendSelected;
                    Dispatcher.BeginInvoke(() => { this.NavigationService.Navigate(new Uri("/Views/TwitPage.xaml", UriKind.Relative)); });
                }, () =>
                {
                    if (TrendSelected == null)
                    {
                        if (Timeout >= 5)
                        {
                            Timeout = 0;
                            if (!NetworkInterface.GetIsNetworkAvailable())
                            {
                                MessageBox.Show("No network connection available please connect and try again", "ERROR", MessageBoxButton.OK);
                                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                            }
                            else
                            {
                                MessageBox.Show("An unexpected error occurred", "ERROR", MessageBoxButton.OK);
                                NavigationService.GoBack();
                            }
                        }
                        else
                        {
                            Timeout++;
                            LocationName_Tap(sender, e);
                        }
                    }
                }
            );
        }
        
        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CountryList != null && Search.Text.Length>=2)
            {
                IsLoading.Visibility = Visibility.Visible;
                FilterListAction(Search.Text);
                Country.ItemsSource = FilterList;
                IsLoading.Visibility = Visibility.Collapsed;
            }
            else if (Search.Text == string.Empty)
            {
                Country.ItemsSource = CountryList;
            }
        }

        private void Search_Tap(object sender, GestureEventArgs e)
        {
            if (Search.Text=="Insert location")
            {
                Search.Text = string.Empty;
            }
        }
        private void FilterListAction(string query)
        {
            if (CountryList !=null)
            {
                FilterList.Clear();
                foreach (Locations x in CountryList)
                {
                    if (x.name.ToLower().Contains(query.ToLower()))
                    {
                        FilterList.Add(x);
                    }
                }
            }
        }

        private void Result_Tap(object sender, GestureEventArgs e)
        {

        }

        private void SearchTrends_Tap(object sender, GestureEventArgs e)
        {
            SearchTrends.Focus();
            if (SearchTrends.Text == "Search Trend")
	        {
                SearchTrends.Text = string.Empty;
	        } 
        }

        private void SearchTrends_TextInput(object sender, TextCompositionEventArgs e)
        {
            if (SearchTrends.Text != string.Empty)
            {
                string query = HttpUtility.UrlEncode("#" + SearchTrends.Text);
                IsLoadingSearch.Visibility = Visibility.Visible;
                if (ResultList == null)
                    ResultList = new ObservableCollection<Twit>();
                else
                    ResultList.Clear();
                ApiTrendService.Search(query, delegate(IEnumerable<Twit> results)
                {
                    foreach (Twit x in results)
                    {
                        ResultList.Add(x);
                    }
                },
                delegate(Exception ex)
                {
                    MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK);
                },
                delegate()
                {
                    if (ResultList == null)
                    {
                        if (TimeoutSearch >= 5)
                        {
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
                        TimeoutSearch++;
                        SearchTrends_TextInput(null, null);
                    }
                    else
                    {
                        TimeoutSearch = 0;
                        IsLoadingSearch.Visibility = Visibility.Collapsed;
                    }
                });
            }
            else
            {
                StatusSearch.Text = "Insert Trend";
                StatusSearch.Visibility = Visibility.Visible;
            }
        }

        private void Search_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Search.Text==string.Empty)
            {
                Search.Text = "Insert Locations";
            }
        }

        private void SearchTrends_LostFocus(object sender, RoutedEventArgs e)
        {
            if (SearchTrends.Text==string.Empty)
            {
                StatusSearch.Text = "Insert Trend";
                StatusSearch.Visibility = Visibility.Visible;
            }
        }
    }
}