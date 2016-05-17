using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using HtmlAgilityPack;

namespace CSharpJSONReader
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
   
    public class Entry
    {
        /// <Description>
        /// 
        /// </Description>
        public string name;                 // stored name of participant's entry 
        private int numberOfEntries = 1;    // number of entries this person has

        public Entry() { }            // Defualt Constructor 
        public Entry(string n) { name = n; }  // Custom Constructor
        public void addEntry() { numberOfEntries++; }   // increment numberOfEntries
        public void Print() { Console.WriteLine(name + " : " + numberOfEntries ); }
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public class RaffleJSONReader
    {
        /// <Description>
        /// Reader built to parse a JSON file achieved by diving through Facebook's structure
        /// Requiring:
        ///     Parsing a JSON file in C#
        ///     Getting access to protected content (FaceBook)  // Certain generated code need to access Facebooks otherwise protected code
        /// 
        ///     HtmlAgility     // Rights to respective owners
        ///     Newtonsoft      // Rights to respective owners
        /// </Description>

        private string app_id = "1706869599564587";                     // Reference Cade to FaceBook   // Generated from FaceBook's Dev tools
        private string app_secret = "3efbd216e0ebdc024734d7baef505ac9"; // DO NOT SHARE WITH ANYONE     // Generated from FaceBook's Dev tools
        private string page_id = "attackonkitten";      // FaceBook Page we're working from
        private string accessToken;     // Combination of app_id and app_secret // (app_id + "|" + app_secret)
        private List<Entry> Entries = new List<Entry>();    // List of Entries

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public RaffleJSONReader()  // Defualt Constructor
        {
            accessToken = app_id + "|" + app_secret;    // Assign AccessToken's value
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void AddEntry(string EntryName)  // Adding Entry to our list Entries
        {
            if (Entries.Exists(p => p.name == EntryName))    // If given name already exist, increment it
            {
                Entries.Find(p => p.name == EntryName).addEntry();
            }
            else                                            // Else instantiate an instance of Entry with the given Name
            {
                Entries.Add(new Entry(EntryName));
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ParseJSON(string type)  // type which data  // Example: "comment", "response", 
        {
            JObject feeds = GrabFromWeb(type);                      // Grab all of the json file
            JArray feed = JArray.Parse(feeds["data"].ToString());   // Grab everything from the 'data' key of feeds 

            foreach (JObject a in feed.Children<JObject>())  // Loops through each post
            {
                string hj = a.ToString();
                if (a[type] != null)
                { 
                    if (a[type]["data"] != null)
                    {
                        string depth1 = a[type]["data"].ToString();
                        JArray post_Collection = JArray.Parse(depth1);
                        foreach (JObject b in post_Collection.Children<JObject>()) // loop through each element of a post
                        {
                            string depth2 = b.ToString();
                            if (b["from"] != null)  // For Messages
                            {
                                string collectedName = b["from"]["name"].ToString();
                                AddEntry(collectedName);
                            }
                            if (b["name"] != null)  // For Reactions
                            {
                                string collectedName = b["name"].ToString();
                                AddEntry(collectedName);
                            }
                        }
                    }
                }
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private JObject GrabFromWeb(string type)   // Uses AccessToken, pageID
        {
            string time = ((int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds).ToString();  // Current time
            string urlBase = "https://graph.facebook.com/v2.6/";   // base url we add to
            string node = "/" + page_id + "/feed";              // Our specified page
            string parameters = "/?fields=" + type + "&since=2016-4-1&until=" + time + "&access_token=" + accessToken;   // Build a string based on our specified parameters: type, date range
            string url = urlBase + node + parameters;                   // Put them all together

            string jsonString = new System.Net.WebClient().DownloadString(url);   // get info as a string 
            JObject jsonJObject = JObject.Parse(jsonString);    // parse it into a JObject file
            return jsonJObject; // return our JObject
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void PrintResults()  // Print Each entry // This prints the entry Name and numberOfEntries
        {
            foreach(Entry e in Entries)
            {
                e.Print();      // Call this entry's print
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    static class Test
    {
        static void Main()
        {
            RaffleJSONReader rjr = new RaffleJSONReader();  // Make an instance of our RaffleJSONReader
            
            rjr.ParseJSON("comments");      //
            rjr.ParseJSON("reactions");     //
            rjr.PrintResults();
        }
    }
    
}
