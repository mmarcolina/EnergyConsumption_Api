using Microsoft.AspNetCore.Mvc;
using CsvHelper;
using EnergyConsumption_Api.Repository;
using System.Globalization;
using EnergyConsumption_Api.Models;
using EnergyConsumption_Api.Constants;

namespace EnergyConsumption_Api.Controllers
{
    [Route("api/energy")]
    [ApiController]
    public class EnergyConsumptionController : ControllerBase
    {
        private List<SteelDataEntity> _steelDataEntity;

        public EnergyConsumptionController()
        {
            using (var reader = new StreamReader("C:\\Users\\mason\\Git\\EnergyConsumption_Api\\EnergyConsumption_Api\\Data\\Steel_industry_data.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<SteelDataMap>();
                var records = csv.GetRecords<SteelDataEntity>().ToList();
                _steelDataEntity = records;
            }
        }

        [HttpGet]
        public ActionResult<ReturnSteelDataDto> Get([FromQuery] SteelDataDto steelDataDto)
        {
            var sum = 0.0;
            var mean = 0.0;
            var median = 0.0;
            var min = 0.0;
            var max = 0.0;

            var filteredData = _steelDataEntity.Where(d => d.date >= steelDataDto.StartDate && d.date <= steelDataDto.EndDate);

            if (!string.IsNullOrEmpty(steelDataDto.WeekStatus))
            {
                filteredData = filteredData.Where(d => d.WeekStatus == steelDataDto.WeekStatus);
            }

            if (!string.IsNullOrEmpty(steelDataDto.Day_of_week))
            {
                filteredData = filteredData.Where(d => d.Day_of_week == steelDataDto.Day_of_week);
            }

            if (!string.IsNullOrEmpty(steelDataDto.Load_Type))
            {
                filteredData = filteredData.Where(d => d.Load_Type == steelDataDto.Load_Type);
            }

            foreach (var operation in steelDataDto.Operation)
            {
                switch (operation)
                {
                    case EnergyConsumptionApiConstants.SUM:
                        sum = SumQueryResult(filteredData);
                        break;
                    case EnergyConsumptionApiConstants.MEAN:
                        mean = AverageQueryResult(filteredData);
                        break;
                    case EnergyConsumptionApiConstants.MEDIAN:
                        median = MedianQueryResult(filteredData);
                        break;
                    case EnergyConsumptionApiConstants.MIN:
                        min = filteredData.Min(d => d.Usage_kWh);
                        break;
                    case EnergyConsumptionApiConstants.MAX:
                        max = filteredData.Max(d => d.Usage_kWh);
                        break;
                    default:
                        throw new Exception();
                }
            }
            var response = new ReturnSteelDataDto
            {
                Sum = sum,
                Mean = mean,
                Median = median,
                Min = min,
                Max = max
            };
            return Ok(response);
        }

        private static double SumQueryResult(IEnumerable<SteelDataEntity> queryResult)
        {
             double sum = 0.0;
            foreach(SteelDataEntity record in queryResult)
            {
                sum += record.Usage_kWh;
            }
            return sum;
        }

        private static double AverageQueryResult(IEnumerable<SteelDataEntity> queryResult)
        {
            double sum = 0.0;
            var count = 0;
            foreach (SteelDataEntity record in queryResult)
            {
                sum += record.Usage_kWh;
                count++;
            }
            return (sum/count);
        }

        private static double MedianQueryResult(IEnumerable<SteelDataEntity> queryResult)
        {
            var index = 0;
            SortedList<int, double> results = new SortedList<int, double>();
            foreach (SteelDataEntity record in queryResult)
            {
                results.Add(index, record.Usage_kWh);
                index++;
            }

            double median;
            var count = results.Count();
            if (count % 2 == 0) {
                median = (results.ElementAt(count / 2).Value + results.ElementAt((count / 2) + 1).Value) / 2;
            } else
            {
                median = results.ElementAt((count + 1) / 2).Key;
            }
            return median;
        }

    }
}
