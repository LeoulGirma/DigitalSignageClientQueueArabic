using DigitalSignageClient.Data.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data
{
    public class ClientDSDbContext : DbContext
    {
        public ClientDSDbContext(DbContextOptions<ClientDSDbContext> options) : base(options)
        {

        }

        public DbSet<CurrencyView> CurrencyViews { get; set; }
        public DbSet<ScheduleView> ScheduleViews { get; set; }
        public DbSet<VideoView> VideoViews { get; set; }
        public DbSet<CalendarView> CalendarViews { get; set; }
        public DbSet<NavStyleView> NavStyleViews { get; set; }
        public DbSet<BodyStyleView> BodyStyleViews { get; set; }
        public DbSet<RSSStyleView> RSSStyleViews { get; set; }
        public DbSet<RSSNewsView> RSSNewsViews { get; set; }
        public DbSet<LicenseStatus> LicenseStatuses { get; set; }
        public DbSet<Counter> Counters { get; set; }
        public DbSet<CounterService> CounterServices { get; set; }
        public DbSet<QueueDisplay> QueueDisplays { get; set; }
        public DbSet<Queue> Queues { get; set; }
        public DbSet<ServiceGroup> ServiceGroups { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<Employee> Employees { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionBuilder)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }

        public void DetachAllEntities()
        {

            ChangeTracker.AutoDetectChangesEnabled = false;
            var changedEntriesCopy = this.ChangeTracker.Entries().ToList();
            foreach (var item in changedEntriesCopy)
            {
                this.Entry(item.Entity).State = EntityState.Detached;
            }
        }
    }
}
