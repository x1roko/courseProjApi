﻿using System;
using System.Collections.Generic;

namespace courseProjAPI.Models;

public partial class BrosShopProductAttribute
{
    public int BrosShopAttributesId { get; set; }

    public int BrosShopProductId { get; set; }

    public int BrosShopCount { get; set; }

    public string? BrosShopColor { get; set; }

    public string? BrosShopSize { get; set; }

    public virtual BrosShopProduct BrosShopProduct { get; set; } = null!;
}
