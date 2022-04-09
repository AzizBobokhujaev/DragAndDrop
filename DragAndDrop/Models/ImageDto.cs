using Microsoft.AspNetCore.Http;

namespace DragAndDrop.Models
{
    public class ImageDto
    {
        public IFormFile Images { get; set; }
        public bool IsMain { get; set; }
        
    }
}