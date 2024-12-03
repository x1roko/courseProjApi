using System;
using System.Collections.Generic;

namespace courseProjAPI.Models;

public partial class BrosShopImage
{
    public int BrosShopImagesId { get; set; }

    public int BrosShopProductId { get; set; }

    public string BrosShopImageTitle { get; set; } = null!;

    public virtual BrosShopProduct BrosShopProduct { get; set; } = null!;
}
