﻿using CsvHelper.Configuration;

namespace EnergyConsumption_Api.Models
{
    public record SteelDataDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? WeekStatus { get; set; }
        public string? Day_of_week { get; set; }
        public string? Load_Type { get; set; }
        public string[]? Operation { get; set; }
    }
}