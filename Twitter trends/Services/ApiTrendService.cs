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
using System.IO.IsolatedStorage;
using System.Collections.ObjectModel;

namespace Twitter_trends
{
    public static class ApiTrendService
    {
        public const string CustomerKey = "jemspqjwrsvuII3CRRfg";
        public const string customerSecret = "FiNZDWSEgdYpT5bIubVT7EzUg8KoOmJ3wseewdyO9Yk";
        const string Token = "158125533-xr2HQyH8UoyckOVSJ8vbk0tFFN7bQEbYTOvKClPF";
        const string TokenSecret = "69c8uGnrjDG8OafO3pCC93MpXtqjrzaViVaH970DgoM";
        const string ApiKey = "af6b4e5630d3be1bad95813cf7058cfc6c52d82a";
        const string SearchUri = "http://search.twitter.com/search.json";
        const string SimpleSearchUri = "trend/search?api_key=";
        const string GetTrendsUri = "v2/trends.json?api_key=";
        const string GetTrendByLocation = "v2/trends/locations/top.json?place_type_code=";
        const string BaseAddres = "http://api.whatthetrend.com/api/";
        const string locationAll = "v2/locations/all.json";
        const string TopsyApi = "http://otter.topsy.com/";
        const string TopsyKey = "EQXRTO2PMDZ5UZJHJQLAAAAAAB35CHNZS5IQAAAAAAAFQGYA";
        const string TopsySearch = "search.json?apikey=" + TopsyKey + "&q=";


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
                if (Results != null)
                {
                    Results(results.trends);
                    if (Finally!=null)
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
        public static void Search(string search, Action<TwitterResults> Results = null, Action Finally = null)
        {
            HttpWebRequest request;
            if (search.StartsWith("?"))
            {
                request = (HttpWebRequest)WebRequest.Create(SearchUri + search);
            }
            else
            {
                request = (HttpWebRequest)WebRequest.Create(SearchUri +"?q="+ search + "&rpp=100");
            }
             
            request.BeginGetResponse(a =>
            {
                var responseStream = request.EndGetResponse(a).GetResponseStream();
                DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(TwitterResults));
                TwitterResults Result = new TwitterResults();
                Result.results = new ObservableCollection<Twit>();
                Result= json.ReadObject(responseStream) as TwitterResults;
                if (Result.results.Count <= 0)
                {
                    SearchOlderResults(search, Results, Finally);
                    return;
                }
                if (Results != null)
                {
                    Result.next_page = Result.next_page==null ? null : "twitt" +Result.next_page;
                    Results(Result);
                    Finally();
                }
            }, request);
        }
        public static void SimpleSearch(string search, Action<IEnumerable<String>> Results = null, Action Finally = null)
        {
            var request = (HttpWebRequest)WebRequest.Create(BaseAddres + SimpleSearchUri + ApiKey + "&q=" + search);
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
        public static void SearchOlderResults(string search, Action<TwitterResults> Results = null, Action Finally = null)
        {
            var request = (HttpWebRequest)WebRequest.Create(TopsyApi + TopsySearch + search + "&perpage=100");
            request.BeginGetResponse(a =>
            {
                var responseStream = request.EndGetResponse(a).GetResponseStream();
                DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(TopsyTwit));
                TopsyTwit result = json.ReadObject(responseStream) as TopsyTwit;
                if (Results != null)
                {
                    TwitterResults twitResults = new TwitterResults();
                    List<Twit> twit = new List<Twit>();
                    foreach (TopsyContent x in result.response.list)
                    {
                        Twit aux = new Twit();
                        aux.from_user = x.trackback_author_name;
                        aux.profile_image_url = x.topsy_author_img;
                        aux.text = x.content;
                        twit.Add(aux);
                    }
                    twitResults.results = new ObservableCollection<Twit>(twit);
                    twitResults.page = result.response.page;
                    twitResults.next_page = "topsy" + search + "&page=" + (result.response.page + 1);
                    if (twitResults.next_page != null && twitResults.next_page.Contains("&page=" + result.response.page))
                    {
                        twitResults.next_page=twitResults.next_page.Replace("&page=" + result.response.page.ToString(), string.Empty);
                    }
                    Results(twitResults);
                    Finally();
                }
            }, request);
        }
    }
}
