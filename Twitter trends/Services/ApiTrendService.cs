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
using System.Text.RegularExpressions;

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
        const string SimpleSearchUri = "trend/search?api_key=";
        const string GetTrendsUri = "v2/trends.json?api_key=";
        const string GetTrendByLocation = "v2/trends/locations/top.json?place_type_code=";
        const string BaseAddres = "http://api.whatthetrend.com/api/";
        const string locationAll = "v2/locations/all.json";


        public static void GetTrends(Action<IEnumerable<Trend>> Results = null, Action<Exception> OnError = null, Action Finally = null)
        {
            var request = (HttpWebRequest)WebRequest.Create(BaseAddres + GetTrendsUri + ApiKey);
         request.BeginGetResponse(a =>
         {
             var Stream = request.EndGetResponse(a).GetResponseStream();
             DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(TrendsResults));
             TrendsResults results = (TrendsResults)json.ReadObject(Stream);
             if (Results != null)
             {
                 Results(results.trends);
                 Finally();
             }
         }, request);
        }
        public static void GetTrendsFromLocation(int Location_type, Action<IEnumerable<Trend>> Results = null, Action Finally = null)
        {

            var request = (HttpWebRequest)WebRequest.Create(BaseAddres + GetTrendByLocation + Location_type);
            request.BeginGetResponse(a =>
            {
                var Stream = request.EndGetResponse(a).GetResponseStream();
                DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(TrendsResults));
                TrendsResults results = (TrendsResults)json.ReadObject(Stream);
                    if (Results!=null)
                    {
                        Results(results.trends);
                        Finally();
                    }
            }, request);
        }
        public static void GetSingleTrendsFromLocation(Locations Loc, Action<Trend> Result = null, Action Finally = null)
        {
            var request = (HttpWebRequest)WebRequest.Create(BaseAddres + GetTrendByLocation + Loc.place_type_code);
            request.BeginGetResponse(a =>
            {
                var Stream = request.EndGetResponse(a).GetResponseStream();
                DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(TrendsResults));
                TrendsResults results = (TrendsResults)json.ReadObject(Stream);
                foreach (Trend x in results.trends)
                {
                    if (x.place_name == Loc.name)
                    {
                        Result(x);
                        Finally();
                        break;
                    }
                }
            }, request);
        }
        public static void GetLocations(Action<IEnumerable<Locations>> Results = null, Action Finally = null)
        {
            var request = (HttpWebRequest)WebRequest.Create(BaseAddres + locationAll);
            request.BeginGetResponse(a =>
            {
                var responseStream = request.EndGetResponse(a).GetResponseStream();
                DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(LocationsResults));
                LocationsResults results = (LocationsResults)json.ReadObject(responseStream);
                if (Results != null)
                {
                    Results(results.locations);
                    Finally();
                }
            }, request);
        }
        public static void Search(string search, Action<IEnumerable<Twit>> Results = null, Action Finally = null)
        {
            var request = (HttpWebRequest)WebRequest.Create(SearchUri + search);
            request.BeginGetResponse(a =>
            {
                var responseStream = request.EndGetResponse(a).GetResponseStream();
                DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(TwitterResults));
                TwitterResults result = json.ReadObject(responseStream) as TwitterResults;
                if (Results != null)
                {
                    Results(result.results);
                    Finally();
                }
            }, request);
        }
        public static void SearchTrend(string query, Action<IEnumerable<Twit>> Results = null, Action Finally = null)
        {
            TwitterService twitter = new TwitterService(CustomerKey, customerSecret, Token, SecretToken);
            SearchOptions search= new SearchOptions();
            search.Q=query;
            twitter.Search(search, (result, response) =>
                {
                    List<Twit> twits = new List<Twit>();
                    foreach (TwitterStatus x in result.Statuses)
                    {
                        Twit aux = new Twit();
                        aux.text = x.Text;
                        aux.from_user = x.User.Name;
                        aux.profile_image_url = x.User.ProfileImageUrl;
                        aux.created_at = x.CreatedDate;
                        twits.Add(aux);
                    }
                    if (Results != null)
                        Results(twits);
                    Finally();
                });
        }
        public static void SimpleSearch(string search, Action<IEnumerable<String>> Results = null , Action Finally=null)
        {
            var request = (HttpWebRequest)WebRequest.Create(BaseAddres+SimpleSearchUri + ApiKey + "&q="+ search);
            request.BeginGetResponse(a =>
            {
                StreamReader responseStream = new StreamReader(request.EndGetResponse(a).GetResponseStream());
                string response = responseStream.ReadToEnd();
                var listed = Regex.Split(response, "\\n");
                if (Results != null)
                {
                    Results(listed);
                    Finally();
                }
            }, request);   
        }
    }
}
