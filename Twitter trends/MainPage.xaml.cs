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
using TweetSharp;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Tasks;
using System.Collections.ObjectModel;
using Microsoft.Phone.Shell;

namespace Twitter_trends
{
    public partial class MainPage : PhoneApplicationPage
    {
        #region Flags
        bool Locations=false;
        bool TrendsByLocations = false;
        bool FlagTrendTopic = false;

        public static readonly DependencyProperty IsLoadingProp =
            DependencyProperty.Register("IsLoading", typeof(bool), typeof(MainPage),
            new PropertyMetadata((bool)false));
        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProp); }
            set { SetValue(IsLoadingProp, value); }
        }
        #endregion
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Views/TrendTopic.xaml", UriKind.Relative));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Views/TrendTools.xaml", UriKind.Relative));
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Locations Loc = new Locations();
            Loc.place_type_code = 12;
            TwitPage.SelectedLocation = Loc;
            TwitPage.Action = 1;
            this.NavigationService.Navigate(new Uri("/Views/TwitPage.xaml", UriKind.Relative));
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if(TrendTools.GlobalCountryList==null)
                LoadLocationsAsync();
            if(TwitPage.GlobalTrends==null)
                LoadTrendsByLocationAsync();
            if (TrendTopic.GlobalTrends==null)
                LoadTrendTopic();
        }
        private void StatusChanged()
        {
            Dispatcher.BeginInvoke(() =>
                {
                    if (!Locations && !TrendsByLocations && !FlagTrendTopic)
                        IsLoading = false;
                    else
                        IsLoading = true;
                });
        }
        private void LoadLocationsAsync()
        {
            if (TrendTools.GlobalCountryList == null)
            {
                Locations = true;
                StatusChanged();
                ObservableCollection<Locations> CountryList = new ObservableCollection<Locations>();
                ApiTrendService.GetLocations(
                        (results) =>
                        {
                            Dispatcher.BeginInvoke(() =>
                            {
                                foreach (Locations aux in results)
                                {
                                    if (aux.place_type_code == 12)
                                        CountryList.Add(aux);
                                }
                            });

                        },
                        () =>
                        {
                            Dispatcher.BeginInvoke(() =>
                            {

                                if (CountryList != null && CountryList.Count > 0 && TrendTools.GlobalCountryList != CountryList)
                                {
                                    TrendTools.GlobalCountryList = CountryList;
                                    Locations = false;
                                    StatusChanged();
                                }
                            });
                        });
            }
        }
        private void LoadTrendsByLocationAsync()
        {
            TrendsByLocations = true;
            StatusChanged();
            ObservableCollection<Trend> Trends = new ObservableCollection<Trend>();
            Trend CurrentTrend = new Trend();
            Locations taped = new Locations();
            taped.place_type_code = 12;
            ApiTrendService.GetTrendsFromLocation(12,
                            (results) =>
                            {
                                    foreach (Trend x in results)
                                    {
                                        if (x.slug == null)
                                        {
                                            x.TittleTrend = x.trend_index.ToString();
                                            x.slug = HttpUtility.UrlEncode(x.name);
                                            x.Header = (x.trend_index.ToString());
                                        }
                                        else
                                        {
                                            x.TittleTrend = HttpUtility.UrlDecode(x.slug);
                                            x.Header = (x.place_name);
                                        }
                                        Trends.Add(x);
                                    }
                            },
                            () =>
                            {
                                if (Trends!=null)
                                    TwitPage.GlobalTrends = Trends;
                                TrendsByLocations = false;
                                StatusChanged();
                            });
        }
        private void LoadTrendTopic()
        {
            ObservableCollection<Trend> trends = new ObservableCollection<Trend>();
            FlagTrendTopic = true;
            StatusChanged();
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
                    FlagTrendTopic = false;
                    StatusChanged();
                },
                () =>
                {
                    FlagTrendTopic = false;
                    TrendTopic.GlobalTrends = trends;
                    StatusChanged();
                });
        }
    }
}