using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serialization.Json;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace SlackApiCommunication
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("SLACK komunikacija");
            Console.WriteLine("__________________");
            Console.WriteLine("[1] Test-Connection");
            Console.WriteLine("[2] Send-Message");
            Console.WriteLine("[3] Get-Message");
            Console.Write(Environment.NewLine);
            string odabir = "";
            Program program = new Program();
            do
            {
                Console.Write("Unesite komandu: ");
                odabir = Console.ReadLine();
                //Polje komandi
                String[] odabirPolje = odabir.Split(' ');




                switch (odabirPolje[0].ToString())
                {
                    //Test-Connection -Token "xoxp-768273224514-781055315909-1148760846918-cebf41639c7d834f5bbe3094051cc49d"
                    case "Test-Connection":
                        {
                            String token = odabirPolje[2].Replace("\"", String.Empty);
                            //Slanje zahtjeva za provjeru konekcije i prosljeđivanje tokena koji je dohvaćen pomoću alata POSTMAN
                            string odgovorPosluzitelja = program.SlanjeZahtjeva("auth.test", token);
                            //Deserializacija odgovora
                            OdgovorKonekcija odgovorKonekcija = JsonConvert.DeserializeObject<OdgovorKonekcija>(odgovorPosluzitelja);
                            if (odgovorKonekcija.ok == true)
                            {
                                Console.WriteLine("Konekcija je ispravna.\nURL: " + odgovorKonekcija.url + ",\nTeam: " + odgovorKonekcija.team + ",\nUser: " + odgovorKonekcija.user + "");
                            }
                            else
                            {
                                Console.WriteLine("Konekcija je neispravna. Provjerite token.");
                            }
                            break;
                        }
                    //Send-Message -Token "xoxp-768273224514-781055315909-1148760846918-cebf41639c7d834f5bbe3094051cc49d" -Channel "CPGH2UN58" -Text "Hello World!"
                    case "Send-Message":
                        {
                            //Regex provjera komande te dohvaćanje tokena i teksta
                            string regEx = "Send-Message -Token (\".+\") -Channel (\".+\") -Text (\".+\")";
                            Match[] matches = Regex.Matches(odabir, @regEx)
                       .Cast<Match>()
                       .ToArray();
                            String token = "";
                            String channel = "";
                            String tekst = "";
                            //Dohvaćanje potrebnih atributa iz komandi
                            if (matches.Length != 0)
                            {
                                token = matches[0].Groups[1].ToString().Replace("\"", String.Empty);
                                channel = matches[0].Groups[2].ToString().Replace("\"", String.Empty);
                                tekst = matches[0].Groups[3].ToString().Replace("\"", String.Empty);
                            }
                            else
                            {
                                Console.WriteLine("Neispravna komanda zahtjeva.");
                                break;
                            }

                            //Slanje zahtjeva za slanje poruka na željeni kanal
                            string odgovorPosluzitelja = program.SlanjeZahtjeva("chat.postMessage", token, channel, tekst);

                            OdgovorSlanje odgovorSlanje = JsonConvert.DeserializeObject<OdgovorSlanje>(odgovorPosluzitelja);
                            if (odgovorSlanje.ok == true)
                            {
                                Console.WriteLine("Poruka je poslana na kanal: " + odgovorSlanje.channel + ".\nType: " + odgovorSlanje.message.type + ",\nText: " + odgovorSlanje.message.text + "");
                            }
                            else
                            {
                                Console.WriteLine("Slanje nije uspjelo. Provjerite komandu zahtjeva.");
                            }

                            break;
                        }
                    //Get-Message -Token "xoxp-768273224514-781055315909-1148760846918 cebf41639c7d834f5bbe3094051cc49d" -Channel "CNSDRDL2X" -Count "4" -Unreads "true" -Oldest "1591005658" -Latest "1591005658"
                    case "Get-Message":
                        {
                            string regEx = "Get-Message -Token (?'token'\".+\") -Channel (?'channel'\"[^\"]*\")(?: -Count (?'count'\"[0-9]*\"))?(?: -Unreads (?'unreads'\"true\"|\"false\"))?(?: -Oldest (?'oldest'\"[0-9]*\"))?(?: -Latest (?'latest'\"[0-9]*\"))?";

                            Match[] matches = Regex.Matches(odabir, @regEx)
                       .Cast<Match>()
                       .ToArray();

                            String token = "";
                            String channel = "";
                            String count = "";
                            String unreads = "";
                            //EPOCH vrijeme
                            String oldest = "";
                            //EPOCH vrijeme
                            String latest = "";
                            if (matches.Length != 0)
                            {
                                token = matches[0].Groups[1].Value.Replace("\"", String.Empty);
                                channel = matches[0].Groups[2].Value.Replace("\"", String.Empty);
                                count = matches[0].Groups[3].Value.Replace("\"", String.Empty);
                                unreads = matches[0].Groups[4].Value.Replace("\"", String.Empty);
                                oldest = matches[0].Groups[5].Value.Replace("\"", String.Empty);
                                latest = matches[0].Groups[6].Value.Replace("\"", String.Empty);

                            }
                            else
                            {
                                Console.WriteLine("Neispravna komanda zahtjeva.");
                                break;
                            }

                            //Dohvaćanje potrebnih atributa iz komandi


                            //Slanje zahtjeva za slanje poruka na željeni kanal
                            string odgovorPosluzitelja = program.SlanjeZahtjeva("channels.history", token, channel, count, unreads, oldest, latest);

                            DohvacanjePoruka.OdgovorDohvacanjePoruka odgovorSlanje = JsonConvert.DeserializeObject<DohvacanjePoruka.OdgovorDohvacanjePoruka>(odgovorPosluzitelja);
                            if (odgovorSlanje.ok == true)
                            {
                                //Ispis dohvaćenih poruka
                                foreach (var poruka in odgovorSlanje.messages)
                                {
                                    Console.WriteLine("Korisnik [" + poruka.user + "]: " + poruka.text);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Dohvaćanje nije uspjelo. Provjerite komandu zahtjeva.");
                            }

                            break;
                        }
                    default:
                        {
                            Console.WriteLine("Nepostojeća komanda: " + odabir + "");
                            break;
                        }
                }
            } while (odabir[0].ToString() != "9");
            Console.ReadLine();
        }
        public String SlanjeZahtjeva(string putanjaKomande, string token)
        {
            String urlMain = "https://slack.com/api/";
            var client = new RestClient(urlMain + putanjaKomande);
            var request = new RestRequest(Method.POST);
            request.AddHeader("Accept", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");
            request.AddParameter("token", token);
            var response = client.Post(request);
            var content = response.Content;
            return content;
        }
        public String SlanjeZahtjeva(string putanjaKomande, string token, string channel, string text)
        {
            String urlMain = "https://slack.com/api/";
            var client = new RestClient(urlMain + putanjaKomande);
            var request = new RestRequest(Method.POST);
            request.AddHeader("Accept", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");
            request.AddParameter("token", token);
            request.AddParameter("channel", channel);
            request.AddParameter("text", text);
            request.AddParameter("as_user", true);
            var response = client.Post(request);
            var content = response.Content;
            return content;
        }

        public String SlanjeZahtjeva(string putanjaKomande, string token, string channel, string count, string unreads, string oldest, string latest)
        {
            String urlMain = "https://slack.com/api/";
            var client = new RestClient(urlMain + putanjaKomande);
            var request = new RestRequest(Method.GET);
            request.AddHeader("Accept", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");
            request.AddParameter("token", token);
            request.AddParameter("channel", channel);
            request.AddParameter("count", count);
            request.AddParameter("unreads", unreads);
            request.AddParameter("oldest", oldest);
            request.AddParameter("latest", latest);
            var response = client.Get(request);
            var content = response.Content;
            return content;
        }
    }
}
