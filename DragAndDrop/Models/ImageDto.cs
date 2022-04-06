using Microsoft.AspNetCore.Http;

namespace DragAndDrop.Models
{
    public class ImageDto
    {
        public IFormFile[] Images { get; set; }
    }
}