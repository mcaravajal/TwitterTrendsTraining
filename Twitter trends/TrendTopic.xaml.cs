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

namespace Twitter_trends
{
    public partial class TrendTopic : PhoneApplicationPage
    {
        int Timeout = 0;
        List<Trend> trends;
        bool IsTrendsLoading;
        string TrendingTopic = "TrendingTopic";
        public TrendTopic()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            trends = this.LoadState<List<Trend>>(TrendingTopic);
            if (trends == null)
            {
                trends = new List<Trend>();
                IsTrendsLoading = true;
                ApiTrendService.GetTrends(delegate
                    (IEnumerable<Trend> results)
                {
                    foreach (Trend x in results)
                    {
                        trends.Add(x);
                    }
                    IsTrendsLoading = false;
                },
                delegate(Exception ex)
                {
                    IsTrendsLoading=false;
                });
            }
        }
        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            this.SaveState(TrendingTopic,trends);
        }
        private void PivotControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (trends == null)
            {
                OnNavigatedTo(null);
            }
        }
    }
}