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
using System.Collections.ObjectModel;
using Twitter_trends.Services;
using Microsoft.Phone.Shell;

namespace Twitter_trends.Views
{
    public partial class TwitList : PhoneApplicationPage
    {
        #region Header
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header",typeof(string),typeof(TwitList),new PropertyMetadata(string.Empty));
        public string Header
        {
            get{ return GetValue(HeaderProperty) as string; }
            set{ SetValue(HeaderProperty,value);}
        }
        #endregion
        #region Tittle
        public static readonly DependencyProperty TittleProperty = DependencyProperty.Register("Tittle",typeof(string),typeof(TwitList),new PropertyMetadata(string.Empty));
        public string Tittle
        {
            get { return GetValue(TittleProperty) as string; }
            set { SetValue(TittleProperty, value); }
        }
        #endregion
        #region Twits
        public static readonly DependencyProperty TwitsProperty = DependencyProperty.Register("TwitsList", typeof(ObservableCollection<Twit>), typeof(TwitList), new PropertyMetadata(((ObservableCollection<Twit>)null)));
        public ObservableCollection<Twit> TwitsList
        {
            get { return GetValue(TwitsProperty) as ObservableCollection<Twit>; }
            set { SetValue(TwitsProperty, value); }
        }
        #endregion
        #region Trend
        public static Trend GlobalTrend { get; set; }
        #endregion
        #region CurrentResults
        public TwitterResults CurrentResults { get; set; }
        #endregion
        #region LoadedResults
        public List<TwitterResults> LoadedResults;
        #endregion
        #region IsLoading
        Visibility isLoading;
        #endregion
        #region LocationSelected
        static public Locations SelectedLocation;
        #endregion
        public TwitList()
        {
            InitializeComponent();
            LoadedResults = new List<TwitterResults>();
        }
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            ApplicationBar.IsVisible = false;
            Loading.Visibility = Visibility.Visible;
            if (GlobalTrend == null && SelectedLocation == null)
            {
                this.GoToPage(ApplicationPages.Back);
            }
            else
            {
                if (SelectedLocation != null)
                {
                    ApiTrendService.GetTrendsFromLocation(SelectedLocation.place_type_code, (Results) =>
                        {
                            foreach (Trend x in Results)
                            {
                                if (x.place_name == SelectedLocation.name)
                                {
                                    Dispatcher.BeginInvoke(() =>
                                        {
                                            Header = x.place_name;
                                            Tittle = x.name;
                                            ApiTrendService.Search(x.slug, (SearchResults) => Result(SearchResults), () => Finally());
                                        });
                                    break;
                                }
                            }
                        },null);
                }
                else
                {
                    Header = GlobalTrend.Header;
                    Tittle = GlobalTrend.TittleTrend;
                    ApiTrendService.Search(GlobalTrend.slug, (SearchResults) => Result(SearchResults), () => Finally());
                }
            }
        }
        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            SelectedLocation = null;
            GlobalTrend = null;
            this.SaveState("TwitList", CurrentResults);
        }
        private void Change_Page_Click(object sender, EventArgs e)
        {
            Loading.Visibility = Visibility.Visible;
            ApplicationBarIconButton button = sender as ApplicationBarIconButton;
            if (button.Text == "Next")
            {
                //If the Twit List is already loaded
                TwitterResults NewResults = LoadedResults.FirstOrDefault<TwitterResults>(x => x.page == CurrentResults.page + 1);
                if (NewResults!=null)
                {
                    Result(NewResults);
                    Finally();
                    return;
                }
                //Get the query to search
                string query = CurrentResults.next_page.Substring(5);
                //Check where the query must run, if it's topsy or twitter
                if (CurrentResults.next_page.StartsWith("topsy"))
                {
                    ApiTrendService.SearchOlderResults(query,(SearchResults) =>Result(SearchResults),() =>Finally());
                }
                else if (CurrentResults.next_page.StartsWith("twitt"))
                {
                    ApiTrendService.Search(query,(SearchResults) =>Result(SearchResults),()=>Finally());
                }
            }
            else if (button.Text == "Back")
            {
                //Get the list which must be loaded, MUST BE LOADED
                TwitterResults NewResults= LoadedResults.FirstOrDefault<TwitterResults>(x=>x.page==CurrentResults.page-1);
                Loading.Visibility = Visibility.Visible;

                if (NewResults != null)
                {
                    Result(NewResults);
                    Finally();
                }
            }
        }
        #region Result and Finally methods
        private void Result(TwitterResults SearchResults)
        {
            Dispatcher.BeginInvoke(() =>
            {
                TwitsList = SearchResults.results;
                CurrentResults = SearchResults;
                Twits.Focus();
            });
        }

        private void Finally()
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (!LoadedResults.Contains(CurrentResults))
                    LoadedResults.Add(CurrentResults);
                
                if (CurrentResults.next_page == null || CurrentResults.results.Count<95)
                    (ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = false;
                else
                    (ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = true;
                
                if (CurrentResults.page == 1)
                    (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = false;
                else
                    (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;

                Loading.Visibility = Visibility.Collapsed;
                ApplicationBar.IsVisible = true;
            });
        }
        #endregion
    }
}