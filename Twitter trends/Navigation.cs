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
using Microsoft.Phone.Controls;

namespace Twitter_trends
{
    public enum ApplicationPages
    {
        Trends,
        Twitter,
        Tools,
        Main,
        Back,
        Auth
    }
    public static class Navigation
    {
        public static void GoToPage(this PhoneApplicationPage phoneApplicationPage, ApplicationPages applicationPage)
        {
            switch (applicationPage)
            {
                case ApplicationPages.Trends:
                    phoneApplicationPage.NavigationService.Navigate(new Uri("/Views/TrendTopic.xaml", UriKind.Relative));
                    break;

                case ApplicationPages.Twitter:
                    phoneApplicationPage.NavigationService.Navigate(new Uri("/Views/TwitPage.xaml", UriKind.Relative));
                    break;

                case ApplicationPages.Tools:
                    phoneApplicationPage.NavigationService.Navigate(new Uri("/Views/TrendTools.xaml", UriKind.Relative));
                    break;
                case ApplicationPages.Main:
                    phoneApplicationPage.NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                    break;
                case ApplicationPages.Back:
                    if (phoneApplicationPage.NavigationService.CanGoBack)
                        phoneApplicationPage.NavigationService.GoBack();
                    else
                        phoneApplicationPage.GoToPage(ApplicationPages.Main);
                    break;
                case ApplicationPages.Auth:
                    phoneApplicationPage.NavigationService.Navigate(new Uri("/Views/Auth.xaml", UriKind.Relative));
                    break;

            }
        }

    }
}
