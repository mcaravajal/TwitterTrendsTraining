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
using System.Security.Cryptography;
using TweetSharp;

namespace Twitter_trends
{
    public static class ApiTrendService
    {
        const string CustomerKey = "jemspqjwrsvuII3CRRfg";
        const string customerSecret = "FiNZDWSEgdYpT5bIubVT7EzUg8KoOmJ3wseewdyO9Yk";
        const string Token = "158125533-xr2HQyH8UoyckOVSJ8vbk0tFFN7bQEbYTOvKClPF";
        const string SecretToken = "69c8uGnrjDG8OafO3pCC93MpXtqjrzaViVaH970DgoM";
        const string ApiKey = "af6b4e5630d3be1bad95813cf7058cfc6c52d82a";
        const string SearchUri = "http://search.twitter.com/search.json?q=";
        const string GetTrendsUri = "v2/trends.json?api_key=";
        const string GetTrendByLocation = "v2/trends/locations/top.json?place_type_code=";
        const string BaseAddres = "http://api.whatthetrend.com/api/";
        const string locationAll = "v2/locations/all.json";
        

        public static void GetTrends(Action<IEnumerable<Trend>> Results = null, Action<Exception> OnError = null, Action Finally = null)
        {
            WebClient Client = new WebClient();
            Client.OpenReadCompleted += delegate(object sender, OpenReadCompletedEventArgs e)
            {
                try
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
                        if (Results != null)
                        {
                            Results(results.trends);
                        }
                    }
                }
                finally
                {
                    Finally();
                }
            };
            Client.OpenReadAsync(new Uri(BaseAddres + GetTrendsUri + ApiKey));
        }
        public static void GetTrendsFromLocation(int Location_type, Action<IEnumerable<Trend>> Results = null, Action<Exception> OnError = null, Action Finally = null)
        {
            WebClient Client = new WebClient();
            Client.OpenReadCompleted += delegate(object sender, OpenReadCompletedEventArgs e)
            {
                try
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
                        if (Results != null)
                        {
                            Results(results.trends);
                        }
                    }
                }
                finally
                {
                    Finally();
                    
                }
            };
            Client.OpenReadAsync(new Uri(BaseAddres + GetTrendByLocation+Location_type));
        }
        public static void GetSingleTrendsFromLocation(Locations Loc, Action<Trend> Result=null,Action Finally=null)
        {
            try
            {
            WebClient Client = new WebClient();
            Client.OpenReadCompleted += delegate(object sender, OpenReadCompletedEventArgs e)
            {

                Stream stream = e.Result;
                DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(TrendsResults));
                TrendsResults results = (TrendsResults)json.ReadObject(stream);
                foreach (Trend x in results.trends)
                {
                    if (x.place_name == Loc.name)
                    {
                        Result(x);
                        break;
                    }
                }
            };
                
            Client.OpenReadAsync(new Uri(BaseAddres + GetTrendByLocation + Loc.place_type_code));
                }
            finally
                {
                    if (Finally!=null)
                    {
                        Finally();
                    }
            }
        }
        public static void GetLocations(Action<IEnumerable<Locations>> Results = null, Action<Exception> Error = null, Action Finally = null)
        {
            WebClient Client = new WebClient();
            Client.OpenReadCompleted += delegate(object sender, OpenReadCompletedEventArgs e)
            {
                try
                {
                    if (e.Error != null)
                    {
                        if (Error != null)
                        {
                            Error(e.Error);
                        }
                    }
                    else
                    {
                        Stream stream = e.Result;
                        DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(LocationsResults));
                        LocationsResults results = (LocationsResults)json.ReadObject(stream);
                        if (Results != null)
                        {
                            Results(results.locations);
                        }
                    }
                }
                finally
                {
                    if (Finally!=null)
                        Finally();
                }
            };
            Client.OpenReadAsync(new Uri(BaseAddres + locationAll));
        }
        public static void Search(string search, Action<IEnumerable<Twit>> Results = null, Action<Exception> Error = null, Action Finally = null)
        {
            WebClient client = new WebClient();
            client.OpenReadCompleted += delegate(object sender, OpenReadCompletedEventArgs e)
            {
                try
                {
                    if (e.Error != null)
                    {
                        if (Error != null)
                        {
                            Error(e.Error);
                        }
                    }
                    else
                    {
                        Stream stream = e.Result;
                        DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(TwitterResults));
                        TwitterResults result = json.ReadObject(stream) as TwitterResults;
                        if (Results != null)
                        {
                            Results(result.results);
                        }
                    }
                }
                finally
                {
                    if (Finally != null)
                        Finally();
                }
            };
            client.OpenReadAsync(new Uri(string.Format(SearchUri + search)));
        }
    }
}
