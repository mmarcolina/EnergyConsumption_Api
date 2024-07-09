namespace EnergyConsumption_Api.Extensions
{
    public class Extensions
    {
        public static DateTime ConvertStringToDateTime(string input)
        {
            var dateTime = DateTime.Parse(input);
            return dateTime;
        }
    }
}
