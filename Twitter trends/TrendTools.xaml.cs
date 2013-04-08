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

namespace Twitter_trends
{
    public partial class TrendTools : PhoneApplicationPage
    {
        public TrendTools()
        {
            InitializeComponent();
        }
#region Country
        
        public static readonly DependencyProperty CountryProperty =
            DependencyProperty.Register("CountryList", typeof(ObservableCollection<Locations>), typeof(TrendTools), new PropertyMetadata((ObservableCollection<Trend>)null));
        
        public ObservableCollection<Locations> CountryList
        {
            get {return (ObservableCollection<Locations>)GetValue(CountryProperty);}
            set { SetValue(CountryProperty, value); }
        }

#endregion
        int Timeout = 0;
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
                        CountryList = new ObservableCollection<Locations>();
                        foreach (Locations aux in results)
                        {
                            if (aux.place_type_name!="Town")
                                CountryList.Add(aux);
                        }
                    },
                    delegate(Exception ex)
                    {
                        MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK);
                        NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                    },
                    delegate()
                    {
                        if (CountryList == null)
                        {
                            if (Timeout >= 5)
                            {
                                Timeout = 0;
                                //TODO if there is network, what should I do? Change the message or show an error message?
                                if (NetworkInterface.GetIsNetworkAvailable())
                                {
                                    MessageBox.Show("No network connection available please connect and try again", "ERROR", MessageBoxButton.OK);
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
                            IsLoading.Visibility= Visibility.Collapsed;
                        }
                    });

            }
        }
        private void LocationName_Tap(object sender, GestureEventArgs e)
        {
            Locations taped= ((TextBlock)sender).DataContext as Locations;
            TwitPage.SelectedLocation= taped;
            this.NavigationService.Navigate(new Uri("/TwitPage.xaml", UriKind.Relative));
        }
    }
}