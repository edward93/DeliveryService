﻿using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Threading.Tasks;
using DAL.Context;
using DAL.Entities;

namespace ServiceLayer.Repository
{
    public class PersonRepository :EntityRepository, IPersonRepository
    {
        public PersonRepository(IDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Person> CreatePersonAsync(Person person)
        {
            try
            {
                DbContext.Persons.AddOrUpdate(person);
                await DbContext.SaveChangesAsync();
                return person;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<Person> GetPersonByUserIdAsync(string userId)
        {
            return await DbContext.Persons.FirstOrDefaultAsync(c => c.UserId == userId);
        }
    }
}