using SampleAPI.Models;
using SampleAPI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SampleAPI.Controllers
{
    [Authorize]
    public class ProductsController : ApiController
    {
        /* Get Method in Web API Controller */

        // GET api/product
        public IHttpActionResult GetAllProduct()
        {
            IList<ProductViewModel> products;

            using (var db = new SampleContext())
            {
                products = db.Products
                    .Select(p => new ProductViewModel()
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Price = p.Price,
                        Quantity = p.quantity,
                        CategoryId = p.CategoryId
                    }).ToList();
            }

            return Ok(products);
        }

        // GET api/product/1
        public IHttpActionResult GetProdcutById(int id)
        {
            ProductViewModel product;

            using (var db = new SampleContext())
            {
                product = db.Products
                    .Where(p => p.Id == id)
                    .Select(p => new ProductViewModel()
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Price = p.Price,
                        Quantity = p.quantity,
                        CategoryId = p.CategoryId
                    }).FirstOrDefault();
            }

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        /* Example: Post Method in Web API Controller */
        // POST api/values
        public IHttpActionResult Post(ProductViewModel product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data.");
            }

            using (var db = new SampleContext())
            {
                db.Products.Add(new Product()
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    quantity = product.Quantity,
                    CategoryId = product.CategoryId
                });

                db.SaveChanges();
            }

            return Ok();
        }

        /* Example: Put Method in Web API Controller */
        // PUT api/values/2
        public IHttpActionResult Put(int id, ProductViewModel product)
        {
            if (id == 0 || !ModelState.IsValid)
            {
                return BadRequest("Not a valid model");
            }

            using (var db = new SampleContext())
            {
                var existingProduct = db.Products
                    .FirstOrDefault(p => p.Id == id);

                if (existingProduct != null)
                {
                    existingProduct.Name = product.Name;
                    existingProduct.Price = product.Price;
                    existingProduct.quantity = product.Quantity;
                    existingProduct.CategoryId = product.CategoryId;
                    db.SaveChanges();
                }
                else
                {
                    return NotFound();
                }
            }

            return Ok();
        }

        /* Example: Delete Method in Web API Controller */

        // DELETE api/values/2
        public IHttpActionResult Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Not a valid product id");
            }

            using (var db = new SampleContext())
            {
                var product = db.Products
                    .FirstOrDefault(p => p.Id == id);

                db.Entry(product).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
            }

            return Ok();
        }

    }
}
