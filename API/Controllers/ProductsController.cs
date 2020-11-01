using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IGenericRespository<Product> _productRepo;
        private readonly IGenericRespository<ProductBrand> _productBrandRepo;
        private readonly IGenericRespository<ProductType> _productTypeRepo;
        private readonly IMapper _mapper;

        public ProductsController(
            IGenericRespository<Product> productRepo,
            IGenericRespository<ProductBrand> productBrandRepo,
            IGenericRespository<ProductType> productTypeRepo,
            IMapper mapper
        )
        {
            _productRepo = productRepo;
            _productBrandRepo = productBrandRepo;
            _productTypeRepo = productTypeRepo;
            _mapper = mapper;
        }

        [HttpGet]
        // Task wraps method in async call, bit like a Promise in node
        public async Task<ActionResult<List<ProductToReturnDto>>> GetProducts()
        {
            var spec = new ProductsWithTypesAndBrandsSpecification();

            var products = await _productRepo.ListAsync(spec);

            return products.Select(product => new ProductToReturnDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                PictureUrl = product.PictureUrl,
                Price = product.Price,
                ProductBrand = product.ProductBrand.Name,
                ProductType = product.ProductType.Name
            }).ToList();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
        {
            var spec = new ProductsWithTypesAndBrandsSpecification(id);

            Product product = await _productRepo.GetEntityWithSpec(spec);

            return Ok(_mapper.Map<Product, ProductToReturnDto>(product));

            // return Ok(
            //     new ProductToReturnDto
            //     {
            //         Id = product.Id,
            //         Name = product.Name,
            //         Description = product.Description,
            //         PictureUrl = product.PictureUrl,
            //         Price = product.Price,
            //         ProductBrand = product.ProductBrand.Name,
            //         ProductType = product.ProductType.Name
            //     }
            // );
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
        {
            return Ok(await _productBrandRepo.ListAllAsync());
        }

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetProductTypes()
        {
            return Ok(await _productTypeRepo.ListAllAsync());
        }
    }
}