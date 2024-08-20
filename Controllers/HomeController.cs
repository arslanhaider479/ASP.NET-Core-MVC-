using CustomIdentity.Data;
using CustomIdentity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomIdentity.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly AppDbContext context;
    private readonly IWebHostEnvironment environment;

    public HomeController(AppDbContext context, IWebHostEnvironment environment)
    {
        this.context = context;
        this.environment = environment;
    }
    public IActionResult Index()
    {
        var products = context.Products.ToList();
        return View(products);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(ProductDto productdto)
    {
        if (productdto.ImageFile == null)
        {
            ModelState.AddModelError("ImageFile","The Image File is Required");  
        }

        if (!ModelState.IsValid)
        {
            return View(productdto);
        }

        //  Save the Image file into product folder
        string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
        newFileName += Path.GetExtension(productdto.ImageFile!.FileName);

        string imageFullPath = environment.WebRootPath + "/products/" + newFileName;
        using(var stream = System.IO.File.Create(imageFullPath))
        {
            productdto.ImageFile.CopyTo(stream);
        }

        // save new product into the database
        Product product = new Product()
        {
            Name = productdto.Name,
            Brand = productdto.Brand,
            Category = productdto.Category,
            Price = productdto.Price,
            Description = productdto.Description,
            ImageFileName = newFileName,
            CreatedAt = DateTime.Now,
        };

        context.Products.Add(product);
        context.SaveChanges();

        return RedirectToAction("Index","Home");
    }

    public IActionResult Edit(int id)
    {
        var product = context.Products.Find(id);
        if(product == null)
        {
            return RedirectToAction("Index", "Home");
        }

        var productDto = new ProductDto()
        {
            Name = product.Name,
            Brand = product.Brand,
            Category = product.Category,
            Price = product.Price,
            Description = product.Description,
            
        };

        ViewData["ProductId"] = product.Id;
        ViewData["ImageFileName"] = product.ImageFileName;
        ViewData["CreatedAt"] = product.CreatedAt.ToString("MM/dd/yyyy");

        return View(productDto);
    }

    [HttpPost]
    public IActionResult Edit(int id,ProductDto productdto)
    {
        var product = context.Products.Find(id);

        if (product == null)
        {
            return RedirectToAction("Index","Home");
        }

        if (!ModelState.IsValid)
        {
            ViewData["ProductId"] = product.Id;
            ViewData["ImageFileName"] = product.ImageFileName;
            ViewData["CreatedAt"] = product.CreatedAt.ToString("MM/dd/yyyy");

            return View(productdto);
        }

        // update the imageFile if have new imageFile
        string newFileName = product.ImageFileName;

        if (productdto.ImageFile != null)
        {
            newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(productdto.ImageFile.FileName);

            string imageFullPath = environment.WebRootPath + "/products/" + newFileName;
            using (var stream = System.IO.File.Create(imageFullPath))
            {
                productdto.ImageFile.CopyTo(stream);
            }

            // delete the old image 
            string oldImageFullPath = environment.WebRootPath + "/products/" + product.ImageFileName;
            System.IO.File.Delete(oldImageFullPath);
        }

        // update product in db
        product.Name = productdto.Name;
        product.Brand = productdto.Brand;
        product.Category = productdto.Category;
        product.Price = productdto.Price;
        product.Description = productdto.Description;
        product.ImageFileName = newFileName;

        context.SaveChanges();

        return RedirectToAction("Index","Home");
    }

    public IActionResult Delete(int id)
    {
        var product = context.Products.Find(id);
        if (product == null)
        {
            return RedirectToAction("Index","Home");
        }

        string imageFullPath = environment.WebRootPath + "/products/" + product.ImageFileName;
        System.IO.File.Delete(imageFullPath);

        context.Products.Remove(product);
        context.SaveChanges(true);

        return RedirectToAction("Index", "Home");
    }
}