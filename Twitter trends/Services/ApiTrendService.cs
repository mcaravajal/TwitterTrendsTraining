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
using System.IO;
using System.Runtime.Serialization.Json;
using System.Collections.Generic;
using Twitter_trends.Services;

namespace Twitter_trends
{
    public static class ApiTrendService
    {
        const string ApiKey = "af6b4e5630d3be1bad95813cf7058cfc6c52d82a";
        const string SearchUri = "trend/search?api_key=";
        const string GetTrendsUri = "v2/trends.json?api_key=";
        const string BaseAddres = "http://api.whatthetrend.com/api/";

        public static void GetTrends(Action<IEnumerable<Trend>> Results=null,Action<Exception> OnError=null)
        {
            WebClient Client = new WebClient();
            Client.OpenReadCompleted += delegate(object sender, OpenReadCompletedEventArgs e)
            {
                if (e.Error != null)
                {
                    if (OnError != null)
                    {
                        OnError(e.Error);
                    }
                }
                else
                {
                    Stream stream = e.Result;
                    DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(TrendsResults));
                    TrendsResults results = (TrendsResults)json.ReadObject(stream);
                    if (Results!=null)
                    {
                        Results(results.trends);
                    }
                }
            };
            Client.OpenReadAsync(new Uri(BaseAddres + GetTrendsUri + ApiKey));
        }
    }
}
