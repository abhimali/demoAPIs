using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;



namespace MyWebApp.Controllers
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public ProductType Type { get; set; }
        // Add more properties as needed
    }
    // create enums for type
    public enum ProductType
    {
        Food,
        Beverage,
        Toy,
        Other
    }

    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public List<OrderItem> Items { get; set; }
        // Add more properties as needed
    }

    public class OrderItem
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly List<Product> products;

        public ProductsController()
        {
            // Initialize the list of products
            products = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", Price = 10.99m, IsActive = true, Type = ProductType.Food  },
                new Product { Id = 2, Name = "Product 2", Price = 19.99m, IsActive = false, Type = ProductType.Food  },
                new Product { Id = 3, Name = "Product 3", Price = 29.99m, IsActive = true, Type = ProductType.Beverage },
                new Product { Id = 4, Name = "Product 4", Price = 39.99m, IsActive = true, Type = ProductType.Other },
                new Product { Id = 5, Name = "Product 5", Price = 49.99m, IsActive = true, Type = ProductType.Toy },
            };
        }

        [HttpGet]
        public IActionResult GetAllActiveProducts()
        {
            var activeProducts = products.Where(p => p.IsActive);
            return Ok(activeProducts);
        }

        [HttpGet("{id}")]
        public IActionResult GetProductById(int id)
        {
            Product product = products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound(products);
            }

            return Ok(product);
        }

    }

    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly List<Order> orders;
        private readonly List<Product> products;

        public OrdersController()
        {
            // Initialize the list of orders
            orders = new List<Order>();

            // Initialize the list of products
            products = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", Price = 10.99m, IsActive = true },
                new Product { Id = 2, Name = "Product 2", Price = 19.99m, IsActive = true },
                // Add more products as needed
            };
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(Order))]
        public IActionResult GetAllOrders()
        {
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public IActionResult GetOrderById(int id)
        {
            Order order = orders.FirstOrDefault(o => o.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        [HttpPost]
        public IActionResult CreateOrder(Order order)
        {
            // Validate order items
            if (order.Items == null || order.Items.Count == 0)
            {
                return BadRequest("Order must have at least one item.");
            }

            foreach (var orderItem in order.Items)
            {
                Product product = products.FirstOrDefault(p => p.Id == orderItem.ProductId);
                if (product == null)
                {
                    return BadRequest($"Invalid product ID: {orderItem.ProductId}");
                }

                if (!product.IsActive)
                {
                    return BadRequest($"Product {product.Name} is currently inactive.");
                }
            }

            order.Id = GenerateOrderId();
            order.OrderDate = DateTime.Now;

            // Perform additional processing, validation, and persistence logic
            // ...

            orders.Add(order);

            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
        }

        // Add more API endpoints for updating and deleting orders as needed

        private int GenerateOrderId()
        {
            Random rnd = new Random();
            return rnd.Next(1, 1000);
        }
    }
}

