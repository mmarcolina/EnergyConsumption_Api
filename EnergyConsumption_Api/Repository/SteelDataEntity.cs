using CsvHelper.Configuration;

namespace EnergyConsumption_Api.Repository
{
    public class SteelDataEntity
    {
        public DateTime date { get; set; }
        public double Usage_kWh { get; set; }
        public double Lagging_Current_Reactive_Power_kVarh { get; set; }
        public double Leading_Current_Reactive_Power_kVarh { get; set; }
        public double CO2_tCO2 { get; set; }
        public double Lagging_Current_Power_Factor { get; set; }
        public double Leading_Current_Power_Factor { get; set; }
        public int NSM { get; set; }
        public string? WeekStatus { get; set; }
        public string? Day_of_week { get; set; }
        public string? Load_Type { get; set; }
    }

    internal class SteelDataMap : ClassMap<SteelDataEntity>
    {
        public SteelDataMap()
        {
            Map(m => m.date).TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>()
            .TypeConverterOption.Format("dd/MM/yyyy HH:mm");
            Map(m => m.Usage_kWh);
            Map(m => m.Lagging_Current_Reactive_Power_kVarh);
            Map(m => m.Leading_Current_Reactive_Power_kVarh);
            Map(m => m.CO2_tCO2);
            Map(m => m.Lagging_Current_Power_Factor);
            Map(m => m.Leading_Current_Power_Factor);
            Map(m => m.NSM);
            Map(m => m.WeekStatus);
            Map(m => m.Day_of_week);
            Map(m => m.Load_Type);
        }
    }
}
