﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities;
using DAL.Enums;

namespace ServiceLayer.Service
{
    public interface IDriverService : IEntityService
    {
        Task<Driver> CreateDriverAsync(Driver driver);
        Task<Driver> UpdateDriverAsync(Driver driver);
        Task<Driver> GetDriverByPersonAsync(string personId);
        Task<bool> DeleteDriver(int driverId);
        Task ChangeDriverStatusAsync(int driverId, DriverStatus newStatus);
        Task ApproveDriverAsync(int driverId, int currentPersonId);
        Task RejectDriverAsync(int driverId, int currentPersonId);
    }
}
