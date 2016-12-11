using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Timers;
using WeatherForcastApi.Models;

namespace UpdateDBForeCast
{
    public class Program
    {
        public static Timer aTimer;
        static HttpClient client = new HttpClient();
        static List<Temperature> ListOfTemperatures = new List<Temperature>();

        static void Main(string[] args)
        {

            RunAsync().Wait();

        }

        static async Task RunAsync()
        {
            client.BaseAddress = new Uri("http://localhost:65144/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {


                aTimer = new Timer(5000);
                aTimer.Elapsed += new ElapsedEventHandler(progress);
                aTimer.AutoReset = true;
                aTimer.Enabled = true;
                GetAllTemperatures();



            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }



        private async static  void progress(object source, ElapsedEventArgs e)
        {
            //ListOfTemperatures = new List<Temperature>();
            Console.Clear();


            foreach (var temp in ListOfTemperatures)
            {
                Random rnd = new Random();
                var value = rnd.Next(-20, 10);
                temp.CityTemperature = value;
                await UpdateTemperatureAsync(temp);

            }
            GetAllTemperatures();

        }


        //Update temperature
        static async Task<Temperature> UpdateTemperatureAsync(Temperature temp)
        {
            HttpResponseMessage response = await client.PutAsJsonAsync($"api/TemperaturesApi/{temp.Id}", temp);
            response.EnsureSuccessStatusCode();

            //Deserialize the updated temperature from the response body.
            temp = await response.Content.ReadAsAsync<Temperature>();
            return temp;
        }



        static async void GetAllTemperatures()
        {
            HttpResponseMessage res = await client.GetAsync("api/TemperaturesApi");
            res.EnsureSuccessStatusCode();

            var temp =  res.Content.ReadAsAsync<IEnumerable<Temperature>>().Result;

            foreach (var t in temp)
            {
                Console.WriteLine("{0} {1}", t.CityName, t.CityTemperature);
                ListOfTemperatures.Add(t);
            }
            Console.ReadLine();
        }
    }
}
