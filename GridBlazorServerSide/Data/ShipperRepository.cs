﻿using GridBlazorServerSide.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace GridBlazorServerSide.Data
{
    public class ShipperRepository : SqlRepository<Shipper>
    {
        public ShipperRepository(NorthwindDbContext context)
            : base(context)
        {
        }

        public override IQueryable<Shipper> GetAll()
        {
            return EfDbSet;
        }

        public override async Task<Shipper> GetById(object id)
        {
            return await GetAll().SingleOrDefaultAsync(c => c.ShipperID == (int)id);
        }
    }
}