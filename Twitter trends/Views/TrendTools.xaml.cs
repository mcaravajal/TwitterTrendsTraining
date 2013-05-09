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
using Twitter_trends.Views;

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
            get { return (ObservableCollection<Locations>)GetValue(CountryProperty); }
            set { SetValue(CountryProperty, value); }
        }
        public static ObservableCollection<Locations> GlobalCountryList;
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
            DependencyProperty.Register("ResultList", typeof(ObservableCollection<Trend>), typeof(TrendTools), new PropertyMetadata((ObservableCollection<Trend>)null));
        public ObservableCollection<Trend> ResultList
        {
            get { return (ObservableCollection<Trend>)GetValue(ResultProperty); }
            set { SetValue(ResultProperty, value); }
        }
        #endregion
        #region Isloading
        public static readonly DependencyProperty IsLoadingProp =
            DependencyProperty.Register("IsLoading", typeof(bool), typeof(TrendTools),
            new PropertyMetadata((bool)false));
        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProp); }
            set { SetValue(IsLoadingProp, value); }
        }
        public static readonly DependencyProperty IsLoadingSearchProp =
            DependencyProperty.Register("IsLoadingSearch", typeof(bool), typeof(TrendTools),
            new PropertyMetadata((bool)false));
        public bool IsLoadingSearch
        {
            get { return (bool)GetValue(IsLoadingSearchProp); }
            set { SetValue(IsLoadingSearchProp, value); }
        }
        #endregion
        #region Timeouts
        int TimeoutSearch = 0;
        #endregion
        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            this.SaveState("TrendTools", CountryList);
        }
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            var state = this.LoadState<ObservableCollection<Locations>>("TrendTools");
            if (state == null && GlobalCountryList == null)
            {
                IsLoading = true;
                ApiTrendService.GetLocations(
                    (results) =>
                    {
                        Dispatcher.BeginInvoke(() =>
                            {

                                CountryList = new ObservableCollection<Locations>();
                                foreach (Locations aux in results)
                                {
                                    if (aux.place_type_code == 12)
                                        CountryList.Add(aux);
                                }
                                StatusLocation.Text = "Total countries: " + CountryList.Count;
                            });
                    },
                    () =>
                    {
                        Dispatcher.BeginInvoke(() =>
                            {
                                IsLoading = false;
                            });
                    });
            }
            else
            {
                Dispatcher.BeginInvoke(() =>
                    {
                        if (GlobalCountryList != null)
                            CountryList = GlobalCountryList;
                        else
                            CountryList = state;
                        Country.ItemsSource = CountryList;
                        IsLoading=false;
                    });
            }
        }
        private void LocationName_Tap(object sender, GestureEventArgs e)
        {
            TwitList.SelectedLocation = ((TextBlock)sender).DataContext as Locations;
            this.GoToPage(ApplicationPages.TwitList);
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CountryList != null && Search.Text.Length >= 2 && Search.Text!="Insert Location")
            {
                IsLoading=true;
                FilterListAction(Search.Text);
                Country.ItemsSource = FilterList;
                IsLoading = false;
            }
            else if (Search.Text == string.Empty || SearchTrends.Text == "Insert Location")
            {
                Country.ItemsSource = CountryList;
            }
        }

        private void FilterListAction(string query)
        {
            if (CountryList != null)
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
            Trend selected= ((TextBlock)sender).DataContext as Trend;
            selected.Header = selected.name;
            selected.place_name = "Earth";
            selected.slug = HttpUtility.UrlEncode(selected.name);
            TwitList.GlobalTrend = selected;
            this.GoToPage(ApplicationPages.TwitList);
        }

        private void SearchTrends_TextInput(object sender, TextCompositionEventArgs e)
        {
                EmptyList.Visibility = Visibility.Collapsed;
                if (SearchTrends.Text != string.Empty && SearchTrends.Text!="Insert Trend")
                {
                    SearchResults.Focus();
                    string query = HttpUtility.UrlEncode("#"+SearchTrends.Text);
                    IsLoadingSearch=true;
                    Dispatcher.BeginInvoke(() =>
                    {
                    if (ResultList == null)
                        ResultList = new ObservableCollection<Trend>();
                    else
                        ResultList.Clear();
                    });
                    ApiTrendService.SimpleSearch(query,
                    (results) =>
                    {
                        Dispatcher.BeginInvoke(() =>
                        {
                            foreach (string x in results)
                            {
                                Trend aux = new Trend();
                                aux.name=x;
                                ResultList.Add(aux);
                            }
                            StatusSearch.Text = "Total finded:"+ ResultList.Count.ToString();
                        });
                    },
                    () =>
                    {
                        Dispatcher.BeginInvoke(() =>
                        {
                            if (ResultList == null)
                            {
                                if (App.CheckError(TimeoutSearch))
                                {
                                    this.GoToPage(ApplicationPages.Back);
                                }
                                else
                                {
                                    TimeoutSearch++;
                                    SearchTrends_TextInput(null, null);
                                }
                            }
                            else
                            {
                                TimeoutSearch = 0;
                                IsLoadingSearch=false;
                            }
                        });
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
            if (Search.Text == string.Empty)
            {
                Search.Text = "Insert Location";
            }
        }

        private void SearchTrends_LostFocus(object sender, RoutedEventArgs e)
        {
            if (SearchTrends.Text == string.Empty)
            {
                StatusSearch.Text = "Insert Trend";
                StatusSearch.Visibility = Visibility.Visible;
            }
        }

        private void Search_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Search.Text=="Insert Location")
            {
                Search.Text = string.Empty;
            }
        }

        private void SearchTrends_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchTrends.Text == "Insert Trend")
            {
                SearchTrends.Text = string.Empty;
            }
        }
    }
}