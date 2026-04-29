using EasyCompany.ConsoleApp.Enums;
using EasyCompany.ConsoleApp.Models;
using EasyCompany.SmartEnums.NewtonsoftJson;
using Newtonsoft.Json;

namespace EasyCompany.ConsoleApp;

internal class Program
{
    static void Main(string[] args)
    {
        try
        {
            var colour = new ColorSelector();
            colour.Room = "Living Room";
            colour.Colour = Colour.Blue;

            var settings = new JsonSerializerSettings
            {
                Converters = { new SmartEnumNewtonsoftConverter<Colour, int>() }
            };

            string output = JsonConvert.SerializeObject(colour, settings);
            var deserializedColour = JsonConvert.DeserializeObject<ColorSelector>(output, settings);

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
