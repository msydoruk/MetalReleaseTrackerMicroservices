﻿using System.ComponentModel.DataAnnotations;

namespace MetalReleaseTracker.CoreDataService.Configuration;

public enum DistributorCode
{
    [Display(Name = "Osmose Productions")]
    OsmoseProductions = 1,

    [Display(Name = "Drakkar Records")]
    Drakkar = 2,

    [Display(Name = "Dark Than Black Records")]
    DarkThanBlackRecords = 3
}