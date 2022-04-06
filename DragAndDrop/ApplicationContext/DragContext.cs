using DragAndDrop.Models;
using Microsoft.EntityFrameworkCore;

namespace DragAndDrop.ApplicationContext
{
    public class DragContext:DbContext
    {
        public DragContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Image> Images { get; set; }
    }
}