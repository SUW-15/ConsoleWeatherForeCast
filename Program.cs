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

        static Random rnd2 = new Random();
        static Random rnd1 = new Random();
        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();
        private static int round=0;

        public static int RandomNumber(int min, int max)
        {
            lock (syncLock)
            {
                return random.Next(max, min);
            }
        }


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
                await GetAllTemperatures();



            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }






        //static int result = 0;
        private async static void progress(object source, ElapsedEventArgs e)
        {
            Console.Clear();

            foreach (Temperature temp in ListOfTemperatures)
            {


                int value1 = random.Next(-15, 25);
                int value2 = random.Next(-20, 30);
                int result = (value1 + value2);
                round = 1;
                temp.CityTemperature = +result-- / 2;
                aTimer = new Timer(1000);
                round = 2;
                temp.CityTemperature = +result-- / 3;
                aTimer = new Timer(3000);
                round = 3;
                temp.CityTemperature = +result-- / 2;
                aTimer = new Timer(2000);
                round++;
                temp.CityTemperature = +result-- / round++;
                await UpdateTemperatureAsync(temp);


            }


            await GetAllTemperatures();

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



        static async Task GetAllTemperatures()
        {
            HttpResponseMessage res = await client.GetAsync("api/TemperaturesApi");
            res.EnsureSuccessStatusCode();

            var temp = res.Content.ReadAsAsync<IEnumerable<Temperature>>().Result;

            foreach (var t in temp)
            {
                Console.WriteLine("{0} {1}", t.CityName, t.CityTemperature);
                ListOfTemperatures.Add(t);
            }
            Console.ReadLine();
        }
    }
}
