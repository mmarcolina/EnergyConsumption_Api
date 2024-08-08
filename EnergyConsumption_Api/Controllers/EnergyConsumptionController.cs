using Microsoft.AspNetCore.Mvc;
using CsvHelper;
using EnergyConsumption_Api.Repository;
using System.Globalization;
using EnergyConsumption_Api.Models;
using EnergyConsumption_Api.Constants;
using System.Diagnostics.Eventing.Reader;

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
        public ActionResult<ReturnSteelDataDto> Get([FromQuery] SteelDataDto steelDataDto, string startDate = "2018-01-01", 
            string endDate = "2018-12-31")
        {
            double? sum = null;
            double? mean = null;
            double? median = null;
            double? min = null;
            double? max = null;
            string? messages = null;

            if (!string.IsNullOrEmpty(steelDataDto.StartDate.ToString()))
            {
                steelDataDto.StartDate = DateTime.Parse(startDate, CultureInfo.InvariantCulture);
            }

            if (!string.IsNullOrEmpty(steelDataDto.EndDate.ToString()))
            {
                steelDataDto.EndDate = DateTime.Parse(endDate, CultureInfo.InvariantCulture);
            }

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
            if (steelDataDto.Operation == null || steelDataDto.Operation.Length == 0)
            {
                messages = EnergyConsumptionApiConstants.NO_OPERATION_SELECTED;
            } else
            {
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
                            try
                            {
                                min = filteredData.Min(d => d.Usage_kWh);
                            } catch
                            {
                                min = EnergyConsumptionApiConstants.OUT_OF_RANGE;
                                throw new InvalidOperationException();
                            }
                            
                            break;
                        case EnergyConsumptionApiConstants.MAX:
                            try
                            {
                                max = filteredData.Max(d => d.Usage_kWh);
                            } catch
                            {
                                min = EnergyConsumptionApiConstants.OUT_OF_RANGE;
                                throw new InvalidOperationException();
                            }
                            break;
                        default:
                            break;

                    }
                }
            }

            var response = new ReturnSteelDataDto();

            if (sum == EnergyConsumptionApiConstants.OUT_OF_RANGE || mean == EnergyConsumptionApiConstants.OUT_OF_RANGE || median == EnergyConsumptionApiConstants.OUT_OF_RANGE || min == EnergyConsumptionApiConstants.OUT_OF_RANGE || max == EnergyConsumptionApiConstants.OUT_OF_RANGE)
            {
                response.Messages = EnergyConsumptionApiConstants.INVALID_RANGE;
            } else
            {
                response.Sum = sum;
                response.Mean = mean;
                response.Median = median;
                response.Min = min;
                response.Max = max;
                response.Messages = messages;
            }

            return Ok(response);
        }

        private static double SumQueryResult(IEnumerable<SteelDataEntity> queryResult)
        {
             double sum = 0.0;
            if(queryResult == null || queryResult.Count() >= 1)
            {
                foreach (SteelDataEntity record in queryResult)
                {
                    sum += record.Usage_kWh;
                }
                return sum;
            } else
            {
                return EnergyConsumptionApiConstants.OUT_OF_RANGE;
            }
            
        }

        private static double AverageQueryResult(IEnumerable<SteelDataEntity> queryResult)
        {
            double sum = 0.0;
            var count = 0;
            if (queryResult == null || queryResult.Count() >= 1)
            {
                foreach (SteelDataEntity record in queryResult)
                {
                    sum += record.Usage_kWh;
                    count++;
                }
                return (sum / count);
            } else
            {
                return EnergyConsumptionApiConstants.OUT_OF_RANGE;
            }
        }

        private static double MedianQueryResult(IEnumerable<SteelDataEntity> queryResult)
        {
            var index = 0;
            SortedList<int, double> results = new SortedList<int, double>();
            if (queryResult == null || queryResult.Count() >= 1)
            {
                foreach (SteelDataEntity record in queryResult)
                {
                    results.Add(index, record.Usage_kWh);
                    index++;
                }

                double median;
                var count = results.Count();
                if (count % 2 == 0)
                {
                    median = (results.ElementAt(count / 2).Value + results.ElementAt((count / 2) + 1).Value) / 2;
                }
                else
                {
                    median = results.ElementAt((count + 1) / 2).Key;
                }
                return median;
            } else
            {
                return EnergyConsumptionApiConstants.OUT_OF_RANGE;
            }
        }

    }
}
