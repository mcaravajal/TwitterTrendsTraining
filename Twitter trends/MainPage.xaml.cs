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

namespace Twitter_trends
{
    public partial class MainPage : PhoneApplicationPage
    {
        
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
            this.NavigationService.Navigate(new Uri("/Views/TwitPage.xaml", UriKind.Relative));
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            /*var settings = IsolatedStorageSettings.ApplicationSettings;
            OAuthAccessToken access= new OAuthAccessToken();
            try
            {
                settings.TryGetValue<OAuthAccessToken>("Token", out access);
            }
            catch (Exception)
            {
                this.GoToPage(ApplicationPages.Auth);
            }*/
        }
    }
}