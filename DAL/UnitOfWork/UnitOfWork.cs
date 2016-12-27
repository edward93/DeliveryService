using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Context;
using DAL.Entities;

namespace DAL.UnitOfWork
{
    public class UnitOfWork : IDisposable
    {
        private IDbContext context = new DbContext();
        private GenericRepository<DriverUpload> _driverUploadRepository;

        public GenericRepository<DriverUpload> DepartmentRepository
        {
            get
            {

                if (this._driverUploadRepository == null)
                {
                    this._driverUploadRepository = new GenericRepository<DriverUpload>(context);
                }
                return _driverUploadRepository;
            }
        }

        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
