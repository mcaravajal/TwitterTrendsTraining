using Microsoft.Phone.Controls;
using System.Collections.ObjectModel;
using Twitter_trends.Services;
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
using Microsoft.Phone.Net.NetworkInformation;

namespace Twitter_trends
{
    public partial class TwitPage : PhoneApplicationPage
    {
        #region Trends
        public static readonly DependencyProperty TrendsProperty =
            DependencyProperty.Register("Trends", typeof(ObservableCollection<Trend>), typeof(TwitPage),
            new PropertyMetadata((ObservableCollection<Trend>)null));
        public ObservableCollection<Trend> Trends
        {
            get { return this.GetValue(TrendsProperty) as ObservableCollection<Trend>; }
            set { SetValue(TrendsProperty, value); }
        }
        #endregion
        #region LocationSelected
        public static Locations SelectedLocation;
        #endregion
        #region SelectedTrend
        public static readonly DependencyProperty SelectedTrendProperty =
            DependencyProperty.Register("Twits",typeof(Trend),typeof(TwitPage), new PropertyMetadata((Trend)null));
        public Trend SelectedTrend
        {
            get { return this.GetValue(SelectedTrendProperty) as Trend; }
            set { this.SetValue(SelectedTrendProperty, value); }
        }
        #endregion
        #region TimeOut
        int TimeOut=0;
#endregion
        public TwitPage()
        {
            InitializeComponent();
        }
        protected override void  OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            var status = this.LoadState<ObservableCollection<Trend>>("TwitPage");
            if (status == null)
            {
                IsLoading.Visibility = Visibility.Visible;
                ApiTrendService.GetTrendsFromLocation(SelectedLocation.place_type_code,
                    delegate(IEnumerable<Trend> results)
                    {
                        Trends = new ObservableCollection<Trend>();
                        foreach (Trend x in results)
                        {
                            Trends.Add(x);
                        }
                    },
                    delegate(Exception ex)
                    {

                    },
                    delegate()
                    {
                        if (Trends == null)
                        {
                            if (TimeOut >= 5)
                            {
                                TimeOut = 0;
                                //TODO if there is network, what should I do? Change the message or show an error message?
                                if (NetworkInterface.GetIsNetworkAvailable())
                                {
                                    MessageBox.Show("No network connection available please connect and try again", "ERROR", MessageBoxButton.OK);
                                    NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                                }
                            }
                            TimeOut++;
                            OnNavigatedFrom(null);
                        }
                        else
                        {
                            IsLoading.Visibility = Visibility.Collapsed;
                        }
                    });
            }
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IsLoading.Visibility = Visibility.Visible;
            SelectedTrend = e.AddedItems[0] as Trend;
            ApiTrendService.Search(SelectedTrend.slug,
                delegate(IEnumerable<Twit> results)
                {
                    SelectedTrend.Twits = new ObservableCollection<Twit>();
                    foreach (Twit x in results)
                    {
                        SelectedTrend.Twits.Add(x);
                    }
                },
                delegate(Exception ex)
                {

                },
                delegate()
                {
                    if (Trends != null)
                    {
                        if (SelectedTrend == null)
                        {
                            OnNavigatedTo(null);
                        }
                        else if (SelectedTrend.Twits == null)
                        {
                            if (TimeOut >= 5)
                            {
                                TimeOut = 0;
                                if (NetworkInterface.GetIsNetworkAvailable())
                                {
                                    MessageBox.Show("No network connection available please connect and try again", "ERROR", MessageBoxButton.OK);
                                    NavigationService.GoBack();
                                }
                                else
                                {
                                    MessageBox.Show("Unexpected error occurred", "ERROR", MessageBoxButton.OK);
                                    NavigationService.GoBack();
                                }
                            }
                            else
                            {
                                TimeOut++;
                                Pivot_SelectionChanged(sender, e);
                            }
                        }
                        else
                        {
                            IsLoading.Visibility = Visibility.Collapsed;
                        }
                    }
                    else
                    {
                        OnNavigatedTo(null);
                    }
                });
        }
        protected override void  OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            this.SaveState("TwitPage", Trends);
        }
        
    }
}