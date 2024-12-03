using System;
using System.Collections.Generic;

namespace courseProjAPI.Models;

public partial class BrosShopOrderComposition
{
    public int BrosShopProductId { get; set; }

    public int BrosShopOrderId { get; set; }

    public sbyte BrosShopQuantity { get; set; }

    public virtual BrosShopOrder BrosShopOrder { get; set; } = null!;

    public virtual BrosShopProduct BrosShopProduct { get; set; } = null!;
}
