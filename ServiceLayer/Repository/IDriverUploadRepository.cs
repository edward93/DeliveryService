﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities;

namespace ServiceLayer.Repository
{
    public interface IDriverUploadRepository : IEntityRepository
    {
        Task<DriverUpload> GetDriverUploadByDriverIdAsync(int id);
    }
}
