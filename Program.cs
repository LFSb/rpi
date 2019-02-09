using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Globalization;

namespace rpi 
{
	class Program 
	{
		static void Main (string[] args) 
		{
      using(var client = new WebClient())
      {
        var content = CallInverter(client, args[0]);

        Console.WriteLine($"Wattage: {content.Item1}. Total kWh: {content.Item2}");
      }
		}

    private static Tuple<int, double> CallInverter(WebClient client, string inverter)
    {
      var data = client.OpenRead($"http://{inverter}/home.cgi"); 			
                
      using(var sr = new StreamReader(data))
      {
        var content = sr.ReadToEnd();
        var spl = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        return new Tuple<int, double>(int.Parse(spl[10]), double.Parse(spl[11], CultureInfo.InvariantCulture));
      }	
    }

    public static void TheRealDeal()
    {
      var currentFileName = Path.Combine(@"C:\UTILS\ZeverKlever", $"{DateTime.Now.ToString("yyyy-MM-dd")}.txt");

      if(!File.Exists(currentFileName))
      {
        File.Create(currentFileName);
      }

      using(var client = new WebClient())
      {
        while(DateTime.Now.Hour < 22 && DateTime.Now.Hour > 6)
        {
          using(var sw = File.AppendText(currentFileName))
          {
            var combinedWattage = 0;
            var combinedkWh = 0.0d;
            
            try
            {
              foreach(var inverter in new []{"192.168.178.23", "192.168.178.24"})
              {
                var data = client.OpenRead($"http://{inverter}/home.cgi"); 			
                
                using(var sr = new StreamReader(data))
                {
                  var content = sr.ReadToEnd();
                  var spl = content.Split(new[] { "\r\n", "\r", "\n" },    StringSplitOptions.None);
                  combinedWattage += int.Parse(spl[10]);
                  combinedkWh += double.Parse(spl[11], CultureInfo.InvariantCulture);
                  
                }	
              }
                    
              sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
              sw.WriteLine($"Current wattage: {combinedWattage}");
              sw.WriteLine($"Total kWh today: {combinedkWh}");
              sw.WriteLine(new string('-', 100));
            }
            catch(Exception ex)
            {
              Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-DD HH:mm:ss.fff")} Something went wrong. {ex.ToString()}");
            }
            finally
            {
              Thread.Sleep(60*1000);		
            }		
          }	
        }
      }
    }
	}
}