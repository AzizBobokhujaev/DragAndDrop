using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DragAndDrop.ApplicationContext;
using DragAndDrop.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragAndDrop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly DragContext _context;

        public ImageController(DragContext context)
        {
            _context = context;
        }

        [HttpGet("GetImages")]
        public async Task<List<Image>> GetImages()
        {
            return await _context.Images.OrderBy(x=>x.OrderId).ToListAsync();
        }

        [HttpPost("Create")]
        public async Task<string> CreateImage([FromForm] ImageDto file)
        {
            var allImages = _context.Images.ToList();
            var path = $"wwwroot/Images/";
            var dbPath = $"Images";
            var fullPath = Path.GetFullPath(path);
            if (!Directory.Exists(fullPath))
            { 
                Directory.CreateDirectory(fullPath);
            }

            var fileExtension = Path.GetExtension(file.Images.FileName);
            var fileName = $"{Guid.NewGuid().ToString()}{fileExtension}";
            var finalFileName = Path.Combine(fullPath, fileName);
            var imagePath = Path.Combine(dbPath, fileName);
            await using (var fileStream = System.IO.File.Create(finalFileName))
            {
                await file.Images.CopyToAsync(fileStream);
            }
                
            if (file.IsMain)
            {
                foreach (var img in allImages)
                {
                    img.IsMain = false;
                }
            }
            var image = new Image()
            {
                Path = imagePath, 
                OrderId = allImages.Count+1, 
                IsMain = file.IsMain
            };
            if (allImages.Count==0) 
            { 
                image.IsMain = true;
            }
            _context.Images.Add(image);
            await _context.SaveChangesAsync();
            return "created";
        }

        [HttpPut("Edit")]
        public async Task Edit(int id,bool isMain)
        {
            var image = await _context.Images.FirstOrDefaultAsync(x => x.Id == id);
            if (image==null)
            {
                return;
            }
            
            if (isMain || _context.Images.Count()==1)
            {
                foreach (var image1 in _context.Images.ToList())
                {
                    image1.IsMain = false;
                }

                image.IsMain = true;
            }
            else if (isMain == false && image.IsMain)
            {
                image.IsMain = true;
            }
            else
            {
                image.IsMain = false;
            }



            
            
            
            _context.Images.Update(image);
            await _context.SaveChangesAsync();

        }

        #region Order
        [HttpPut("Order")]
        public async Task<List<Image>> Order(int orderId1, int orderId2)
        {
            var images = _context.Images.ToList();
            var img1 = await _context.Images.FirstOrDefaultAsync(x => x.OrderId == orderId1);
            var img2 = await _context.Images.FirstOrDefaultAsync(x => x.OrderId == orderId2);
            List<Image> result = null;
            if (img2==null)
            { 
                foreach (var t in images)
                {
                    t.OrderId -= 1;
                    img1.OrderId = images.Count;
                }
                
                result = images.OrderBy(x => x.OrderId).ToList();
                for (var i = 0; i < result.Count; i++)
                {
                    result[i].OrderId = i + 1;
                }
                _context.Images.UpdateRange(result);
                await _context.SaveChangesAsync();
                return result;
            }
            var imgOrderId1 = img1.OrderId;
            var imgOrderId2 = img2.OrderId;
            
            if (imgOrderId1<imgOrderId2)
            {
                for (var i = 0; i < imgOrderId2-1; i++)
                {
                    img1.OrderId = imgOrderId2-1;
                    images[i].OrderId -= 1;
                }
                result = images.OrderBy(x => x.OrderId).ToList();
            }
            else
            {
                foreach (var t in images)
                {
                    t.OrderId += 1;           
                    img1.OrderId = imgOrderId2;
                }
                result = images.OrderBy(x => x.OrderId).ToList();
            }
            for (var i = 0; i < result.Count; i++)
            {
                result[i].OrderId = i + 1;
            }
            _context.Images.UpdateRange(result);
            await _context.SaveChangesAsync();
            return result;
        }
        #endregion

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        [HttpGet("GetImageById")]
        public async Task<Image> GetImageById(int imageId)
        {
            return await _context.Images.FirstOrDefaultAsync(x => x.Id == imageId);
        }

        [HttpDelete("Delete")]
        public async Task<bool> Delete()
        {
            var images = await GetImages();
            _context.Images.RemoveRange(images);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}