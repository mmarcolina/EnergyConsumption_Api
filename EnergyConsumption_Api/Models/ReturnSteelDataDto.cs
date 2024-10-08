﻿using System.ComponentModel.DataAnnotations;

namespace EnergyConsumption_Api.Models
{
    public class ReturnSteelDataDto
    {
        public double? Sum { get; set; }
        public double? Mean { get; set; }
        public double? Median { get; set; }
        public double? Min { get; set; }
        public double? Max { get; set; }
        public string? Messages { get; set; }
    }
}
