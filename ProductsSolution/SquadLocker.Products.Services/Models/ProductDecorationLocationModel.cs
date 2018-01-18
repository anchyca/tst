using SquadLocker.Products.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadLocker.Products.Services.Models
{
    public class ProductDecorationLocationModel
    {
        public int ProductId { get; set; }
        public DecorationLocationEnum DecorationLocation { get; set; }
    }
}
