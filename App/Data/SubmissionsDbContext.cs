using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using App.Models;

namespace App.Data
{
    public class SubmissionsDbContext : DbContext
    {
        public SubmissionsDbContext(DbContextOptions<SubmissionsDbContext> contextOptions)
            : base(contextOptions)
        {
            
        }

        public DbSet<SubmissionDBO> SubmissionDBOs { get; set; }
    }
}