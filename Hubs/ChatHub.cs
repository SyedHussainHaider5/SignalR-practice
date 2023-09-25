using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;



namespace SignalRChat.Hubs
{
    public class ChatHub : Hub
    {
        private static bool jsonFileCreated = false;
        public override async Task OnConnectedAsync()
        {
            try
            {
                string filePath = "./calls.json";
                if (!jsonFileCreated)
                {
                    var initialData = new
                    {
                        offerCandidates = new List<object>(),
                        answerCandidates = new List<object>(),
                        answer = new JObject(),
                        offer = new JObject()
                    };
                    string initialJson = JsonConvert.SerializeObject(initialData);
                    File.WriteAllText(filePath, initialJson);
                    jsonFileCreated = true;
                    Console.WriteLine("JSON file created with initial structure.");
                }
                else
                {
                    Console.WriteLine("JSON file already exists.");
                }
                await base.OnConnectedAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task GetData()
        {
            string jsonData;
            jsonData = File.ReadAllText("./calls.json");

            await Clients.Caller.SendAsync("ReceiveData", jsonData);
        }

        //Offer
        public async Task SetOfferCandidates(string offerCandidates)
        {
            try
            {
                string filePath = "./calls.json";
                var json = File.ReadAllText(filePath);
                var jsonObj = JObject.Parse(json);
                var offerCandidatesArray = jsonObj["offerCandidates"] as JArray;
                JToken offerCandidatesToken = JToken.Parse(offerCandidates);
                foreach (var item in offerCandidatesToken)
                {
                    offerCandidatesArray.Add(item);
                }

                jsonObj["offerCandidates"] = offerCandidatesArray;
                string newJsonResult = jsonObj.ToString(Formatting.Indented);
                File.WriteAllText(filePath, newJsonResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                throw;
            }
        }
        public async Task SetOffer(string offer)
        {
            try
            {
                Console.WriteLine("I'm here in SetOffer Server side!: " + offer);
                string filePath = "./calls.json";
                var json = File.ReadAllText(filePath);
                var jsonObj = JObject.Parse(json);
                var offerCandidatesArray = jsonObj["offer"] as JObject;
                JToken offerCandidatesToken = JToken.Parse(offer);
                // offerCandidatesArray.Add(offerCandidatesToken);

                jsonObj["offer"] = offerCandidatesToken;
                string newJsonResult = jsonObj.ToString(Formatting.Indented);
                File.WriteAllText(filePath, newJsonResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                throw;
            }
        }

        //Answer
        public async Task SetAnswerCandidates(string offerCandidates)
        {
            try
            {
                string filePath = "./calls.json";
                var json = File.ReadAllText(filePath);
                var jsonObj = JObject.Parse(json);
                var offerCandidatesArray = jsonObj["offerCandidates"] as JArray;
                JToken offerCandidatesToken = JToken.Parse(offerCandidates);
                foreach (var item in offerCandidatesToken)
                {
                    offerCandidatesArray.Add(item);
                }

                jsonObj["offerCandidates"] = offerCandidatesArray;
                string newJsonResult = jsonObj.ToString(Formatting.Indented);
                File.WriteAllText(filePath, newJsonResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                throw;
            }
        }
        public async Task SetAnswer(string offer)
        {
            try
            {
                Console.WriteLine("I'm here in SetOffer Server side!: " + offer);
                string filePath = "./calls.json";
                var json = File.ReadAllText(filePath);
                var jsonObj = JObject.Parse(json);
                var offerCandidatesArray = jsonObj["offer"] as JObject;
                JToken offerCandidatesToken = JToken.Parse(offer);
                // offerCandidatesArray.Add(offerCandidatesToken);

                jsonObj["offer"] = offerCandidatesToken;
                string newJsonResult = jsonObj.ToString(Formatting.Indented);
                File.WriteAllText(filePath, newJsonResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                throw;
            }
        }








    }
}