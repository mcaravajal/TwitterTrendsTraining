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
        #region Global Trends
        public static ObservableCollection<Trend> GlobalTrends;
        #endregion
        #region Global Selected Trend
        public static Trend GlobalSelectedTrend;
        #endregion
        #region SelectedTrend
        /// <summary>
        /// CurrentTrend Dependency Property
        /// </summary>
        public static readonly DependencyProperty CurrentTrendProperty =
            DependencyProperty.Register("CurrentTrend", typeof(Trend), typeof(TwitPage),
                new PropertyMetadata((Trend)null,
                    new PropertyChangedCallback(OnCurrentTrendChanged)));

        /// <summary>
        /// Gets or sets the CurrentTrend property. This dependency property 
        /// indicates what is the current trend.
        /// </summary>
        public Trend CurrentTrend
        {
            get { return (Trend)GetValue(CurrentTrendProperty); }
            set { SetValue(CurrentTrendProperty, value); }
        }

        /// <summary>
        /// Handles changes to the CurrentTrend property.
        /// </summary>
        private static void OnCurrentTrendChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TwitPage target = (TwitPage)d;
            Trend oldCurrentTrend = (Trend)e.OldValue;
            Trend newCurrentTrend = target.CurrentTrend;
            target.OnCurrentTrendChanged(oldCurrentTrend, newCurrentTrend);
        }
        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the CurrentTrend property.
        /// </summary>
        protected virtual void OnCurrentTrendChanged(Trend oldCurrentTrend, Trend newCurrentTrend)
        {
            if (newCurrentTrend != oldCurrentTrend)
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    FlagPivot = true;
                    PivotControl.SelectedItem = newCurrentTrend;
                });
            }
        }
        #endregion
        #region TimeOut
        int TimeOut=0;
#endregion
        #region Flags
        public bool FlagPivot;
        public static int Action; //1=View only 1 location 2= View List of location 3=View list of trends
        #endregion 
        #region LoadedTwits
        public List<Trend> LoadedTrends= new List<Trend>();
        #endregion
        public TwitPage()
        {
            InitializeComponent();
        }
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (SelectedLocation != null)
            {
                if (Action == 1)
                {
                    Trend TrendSelected = null;
                    Locations taped = SelectedLocation;
                    ApiTrendService.GetSingleTrendsFromLocation(taped,
                        (result) =>
                        {
                            Dispatcher.BeginInvoke(() =>
                            {
                                TrendSelected = result;
                                TrendSelected.place_name = taped.name;
                                TrendSelected.slug = null;
                                TrendSelected.Header = TrendSelected.place_name;
                                TrendSelected.TittleTrend = TrendSelected.name;
                                Trends = new ObservableCollection<Trend>();
                                Trends.Add(TrendSelected);
                                CurrentTrend = TrendSelected;
                            });

                        }, () =>
                        {
                            Dispatcher.BeginInvoke(() =>
                            {
                                if (TrendSelected == null)
                                {
                                    if (App.CheckError(TimeOut))
                                    {
                                        TimeOut = 0;
                                        this.GoToPage(ApplicationPages.Back);
                                    }
                                    else
                                        TimeOut++;
                                }
                            });
                        });
                }
                else
                {
                    var status = this.LoadState<ObservableCollection<Trend>>("TwitPage");
                    if (status == null)
                    {
                        FlagPivot = false;
                        IsLoading.Visibility = Visibility.Visible;
                        ApiTrendService.GetTrendsFromLocation(SelectedLocation.place_type_code,
                            (results) =>
                            {
                                Dispatcher.BeginInvoke(() =>
                                {
                                    Trends = new ObservableCollection<Trend>();
                                    foreach (Trend x in results)
                                    {
                                        if (x.slug == null)
                                        {
                                            x.TittleTrend = x.trend_index.ToString();
                                            x.slug = HttpUtility.UrlEncode(x.name);
                                            x.name = null;
                                            x.Header = (x.trend_index.ToString());
                                        }
                                        else
                                        {
                                            x.TittleTrend = HttpUtility.UrlDecode(x.slug);
                                            x.Header = (x.place_name);
                                        }
                                        Trends.Add(x);
                                    }
                                });
                            },
                            () =>
                            {
                                Dispatcher.BeginInvoke(() =>
                                    {
                                        if (Trends == null)
                                        {
                                            if (App.CheckError(TimeOut))
                                            {
                                                TimeOut = 0;
                                                this.GoToPage(ApplicationPages.Back);
                                            }
                                            else
                                            {
                                                TimeOut++;
                                                OnNavigatedFrom(null);
                                            }
                                        }

                                        else
                                        {
                                            IsLoading.Visibility = Visibility.Collapsed;
                                            if (SelectedLocation.name != null)
                                            {
                                                foreach (Trend x in Trends)
                                                {
                                                    if (x.place_name == SelectedLocation.name)
                                                    {
                                                        FlagPivot = true;
                                                        PivotControl.SelectedItem = x;
                                                        break;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                FlagPivot = true;
                                                PivotControl.SelectedItem = (Trends[0]);
                                            }
                                        }
                                    });
                            });
                    }
                }
            }
            else
            {
                if (Action == 3 || Action==4)
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        Trends = new ObservableCollection<Trend>();
                        
                        if (GlobalSelectedTrend.slug == null)
                        {
                            if (GlobalSelectedTrend.trend_index == 0)
                            {
                                GlobalSelectedTrend.Header = GlobalSelectedTrend.place_name;
                                GlobalSelectedTrend.TittleTrend = GlobalSelectedTrend.name;
                                GlobalSelectedTrend.slug = HttpUtility.UrlEncode(GlobalSelectedTrend.name);
                                GlobalSelectedTrend.name = null;
                            }
                            else
                            {
                                GlobalSelectedTrend.TittleTrend = GlobalSelectedTrend.trend_index.ToString();
                                GlobalSelectedTrend.slug = HttpUtility.UrlEncode(GlobalSelectedTrend.name);
                                GlobalSelectedTrend.name = null;
                            }
                        }
                        while (GlobalSelectedTrend.slug.StartsWith("+"))
                        {
                            GlobalSelectedTrend.slug = GlobalSelectedTrend.slug.Remove(0, 1);
                        }
                        if (Action == 3)
                            Trends = GlobalTrends;
                        else
                            Trends.Add(GlobalSelectedTrend);
                        CurrentTrend = GlobalSelectedTrend;
                        FlagPivot = true;
                        IsLoading.Visibility = Visibility.Collapsed;
                    });
                }
            }
        }
        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems[0]!=null)
            {
                FlagPivot = false;
                Trend ChangedTo = e.AddedItems[0] as Trend;

                if (ChangedTo.exist(LoadedTrends))
                {
                    foreach (Trend x in LoadedTrends)
                    {
                        if (x.slug==ChangedTo.slug)
                        {
                            FlagPivot = true;
                            ChangedTo.Twits = x.Twits;
                            break;
                        }
                    }
                        IsLoading.Visibility= Visibility.Collapsed;
                }
                else
                {
                    FlagPivot = false;
                    IsLoading.Visibility = Visibility.Visible;
                    string query = ChangedTo.slug == null ? HttpUtility.UrlEncode(ChangedTo.name) : ChangedTo.slug;
                    ApiTrendService.Search(query,
                            (results) =>
                            {
                                Dispatcher.BeginInvoke(() =>
                                    {
                                        ChangedTo.Twits = new ObservableCollection<Twit>();
                                        foreach (Twit x in results)
                                        {
                                            ChangedTo.Twits.Add(x);
                                        }
                                        IsLoading.Visibility = Visibility.Collapsed;
                                        if (!ChangedTo.exist(LoadedTrends))
                                        {
                                            LoadedTrends.Add(ChangedTo);
                                        }
                                    });
                            },
                            () =>
                            {
                                Dispatcher.BeginInvoke(() =>
                                {
                                    if (Trends != null)
                                    {
                                        if (ChangedTo == null)
                                        {
                                            OnNavigatedTo(null);
                                        }
                                        else if (ChangedTo.Twits == null)
                                        {
                                            if (App.CheckError(TimeOut))
                                            {
                                                TimeOut = 0;
                                                this.GoToPage(ApplicationPages.Back);
                                            }
                                            else
                                            {
                                                TimeOut++;
                                                Pivot_SelectionChanged(sender, e);
                                            }
                                        }
                                        else
                                        {
                                            FlagPivot = true;
                                        }
                                    }

                                    else
                                    {
                                        OnNavigatedTo(null);
                                    }
                                });
                            });
                }
            }
        }
        protected override void  OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            SelectedLocation = null;
            GlobalSelectedTrend = null;
            GlobalTrends = null;
            this.SaveState("TwitPage", Trends);
        }
    }
} 