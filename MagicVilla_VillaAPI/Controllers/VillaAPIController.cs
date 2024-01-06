using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Interfaces;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Text.RegularExpressions;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/VillaAPI")]
    [ApiController] // this allows us to add few built in basic api validations like annotations and stuff
    public class VillaAPIController : ControllerBase
    {
        //   private readonly ILogger<VillaAPIController> _logger; // because of dependency injection we need not instantiate the class disposing and access is all proided by .net

        /*        public VillaAPIController(ILogger<VillaAPIController> logger)
                {
                    _logger = logger;
                }*/

        private readonly ILogging _logging;
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public VillaAPIController(ILogging logging, ApplicationDbContext db, IMapper mapper)
        {
            _logging = logging;
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
        {
            IEnumerable<Villa> villaList = await _db.Villas.ToListAsync(); // getting from the database as villa model and returning the villaDTO. basically a reverse map
            return Ok(_mapper.Map<List<VillaDTO>>(villaList));
        }

        [HttpGet("{Id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)] // this document the response on swagger
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        /*        [ProducesResponseType(200, Type = typeof(VillaDTO))]*/
        public async Task<ActionResult<VillaDTO?>> GetVilla(int Id) // By defining hte type of response swagger give an example of sample response. ex: ActionResult<VillaDTO> gives sample VillaDTO response
        {
            if (Id <= 0)
            {
                _logging.Log("Villa not found", "error");
                return BadRequest();
            }
            var response = await _db.Villas.FirstOrDefaultAsync(v => v.Id == Id);
            if (response == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<VillaDTO>(response));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)] // this document the response on swagger
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VillaDTO>> CreateVilla([FromBody] VillaCreateDTO villaCreateDTO)
        { 
            /*            if (!ModelState.IsValid) {
                            return BadRequest(ModelState); not needed if ApiController Annotation is used on the controlled class
                        }*/
            if (villaCreateDTO == null)
            {
                return BadRequest();
            }

            if (await _db.Villas.FirstOrDefaultAsync(v => v.Name.ToLower() == villaCreateDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError", "Villa already exists"); // first arg is a key valye unique always and second arg is a message. return the model state when doing this
                return BadRequest(ModelState);
            }
            /*            if (villa.Id <= 0)
                        {
                            return StatusCode(StatusCodes.Status500InternalServerError);
                        }*/ //not needed as Entity framework create id as automatic field
            /*            villa.Id = _db.Villas.OrderByDescending(v => v.Id).FirstOrDefault().Id + 1;*/ // this is no longer needed as we defiend the id column as an identity column
            Villa villaModel = _mapper.Map<Villa>(villaCreateDTO);     // mapping to villa model as we are creating the database which is of type villa model  
            //            VillaStore.villaList.Add(villa);
            // on our model
/*            Villa villaModel = new()
            {
                Name = villaCreateDTO.Name,
                Sqft = (int)villaCreateDTO.Sqft,
                Occupancy = villaCreateDTO.Occupancy,
                ImageUrl = villaCreateDTO.ImageUrl,
                Amenity = villaCreateDTO.Amenity,
                Details = villaCreateDTO.Details,
                Rate = villaCreateDTO.Rate,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            }; not needed after using auto mapper*/

            await _db.Villas.AddAsync(villaModel);  // this will just keep track of all the changes we want to make into database. save chanages is needed to complete the operation 
            await _db.SaveChangesAsync(); // this will gather and push the change on database this is needed to complete the addition to databse instead of making 3 calls we call this to make on call and save the changes

            /*            return Ok(villa);*/
            return CreatedAtRoute("GetVilla", new { id = (int)villaModel.Id }, villaModel); // this takes 3 args explicit name of the route you want to return, query param for that end point returned if it has one, and the return obj itself
        }

        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteVilla(int villaId)
        { // when if you want to return and object or document return type is ActionResult<T> else use IActionResult
            if (villaId <= 0)
            {
                return BadRequest();
            }
            var villa = await _db.Villas.FirstOrDefaultAsync(v => v.Id == villaId);
            if (villa == null)
            {
                _logging.Log("Villa could not be found", "error");
                return NotFound();
            }
            _db.Villas.Remove(villa);
            _db.SaveChanges();
            /*            if (VillaStore.villaList.FirstOrDefault(v => v.Id == villaId) == null)
                        {
                            ModelState.AddModelError("NotFoundCustomError", "No villa Id was found");
                            return NotFound(ModelState);
                        }
                        var villa = VillaStore.villaList.Find(v => v.Id == villaId);
                        if (villa == null)
                        {
                            return NotFound();
                        }
                        VillaStore.villaList.Remove(villa);*/
            _logging.Log("Villa deleted", "");
            return Ok(true);
        }

        [HttpPut("id:int", Name = "UpdateVilla")]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateVillaDto(int villaId, [FromBody] VillaUpdateDTO villaUpdateDTO)
        {
            if (villaId <= 0 || villaUpdateDTO.Id != villaId)
            {
                return BadRequest();
            }
            /*            var villaDetails = _db.Villas.FirstOrDefault(x => x.Id == villaId);*/ // with entity framework we need not retrieve the details from database we can define the mapping and entity framework will notice that it needs to update the recirds with the specific ID value
            /*            var villaDetails = _db.Villas.FirstOrDefault(x => x.Id == villaId);
                        if (villaDetails != null)
                        {
                            villaDetails.Id = (int)villaUpdateDTO.Id;
                            villaDetails.Name = villaUpdateDTO.Name;
                            villaDetails.Occupancy = villa.Occupancy;
                            villaDetails.Sqft = (int)villa.Sqft;
                            villaDetails.Amenity = villa.Amenity;
                            villaDetails.Rate  = villa.Rate;
                            villaDetails.ImageUrl = villa.ImageUrl;
                            villaDetails.Details = villa.Details;
                            _db.SaveChanges();
                            return Ok(true);
                        }
                        return NoContent();*/
            Villa villaModel = _mapper.Map<Villa>(villaUpdateDTO); // mapping the input villaUpdateDTO to Villa
            
/*            Villa villaModel = new()
            {
                Id = villaUpdateDTO.Id ?? 0,
                Name = villaUpdateDTO.Name,
                Sqft = (int)villaUpdateDTO.Sqft, // assuming Sqft in DTO is a double; if not, you can remove the cast
                Occupancy = villaUpdateDTO.Occupancy,
                ImageUrl = villaUpdateDTO.ImageUrl,
                Amenity = villaUpdateDTO.Amenity,
                Details = villaUpdateDTO.Details,
                Rate = villaUpdateDTO.Rate,
                CreatedDate = DateTime.Now, // Assuming you want to set current date
                UpdatedDate = DateTime.Now  // Assuming you want to set current date
            }; not needed after using auto mapper*/
            _db.Update(villaModel);
            await _db.SaveChangesAsync();
            return Ok(villaModel);
        }

        [HttpPatch("{id:int}", Name = "PartialUpdateVilla")]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PartialVillaUpdate(int villaId, JsonPatchDocument<VillaUpdateDTO> patchVilla)
        {
            if (villaId <= 0 || patchVilla == null)
            {
                return BadRequest();
            }
/*
            if (villaModel != null)
            {
                patchVilla.ApplyTo(villaModel, ModelState); // patchVilla allows you to update only specific fields when sending request to this end point now
                if (ModelState.IsValid)
                {
                    return NoContent();
                }
                return BadRequest();
            }*/ // this is not needed when working with entity framework model we setup the dto object which we pass into the patch update and then after performing applyTo we assign it back to the villa model and call update and save

            var villa = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(x => x.Id == villaId); // we dont want to track this records as we are updating on the model iteself so dont want to track this DTO
            VillaUpdateDTO villaUpdateDTO = _mapper.Map<VillaUpdateDTO>(villa);
/*            VillaUpdateDTO villaDTO = new()
            {
                Id = villa.Id,
                Name = villa.Name,
                Sqft = (int)villa.Sqft, // assuming Sqft in DTO is a double; if not, you can remove the cast
                Occupancy = villa.Occupancy,
                ImageUrl = villa.ImageUrl,
                Amenity = villa.Amenity,
                Details = villa.Details,
                Rate = villa.Rate
            }; not needed when using auto mapper*/

            if (villa == null) { 
                return BadRequest();
            }

            patchVilla.ApplyTo(villaUpdateDTO, ModelState);
            Villa villaModel = _mapper.Map<Villa>(villaUpdateDTO);
            /*Villa villaModel = new()
            {
                Id = villaDTO.Id ?? 0,
                Name = villaDTO.Name,
                Sqft = (int)villaDTO.Sqft, // assuming Sqft in DTO is a double; if not, you can remove the cast
                Occupancy = villaDTO.Occupancy,
                ImageUrl = villaDTO.ImageUrl,
                Amenity = villaDTO.Amenity,
                Details = villaDTO.Details,
                Rate = villaDTO.Rate,
                CreatedDate = DateTime.Now, // Assuming you want to set current date
                UpdatedDate = DateTime.Now  // Assuming you want to set current date
            };*/
            _db.Update(villaModel);
            await _db.SaveChangesAsync();
            // idealally on patch we use a sproc to update certain fields 
            return Ok();
        }

        private bool IsValidTopping(string description, string culturecode)
        {
            var filterredDescription = description.Split(":");
            var wholeToppingCount = 0m;
            var leftOrRightToppingCount = 0m;
            var wholeText = culturecode.ToLower() == "en-us" ? "Whole" : "Entera";
            var rightText = culturecode.ToLower() == "en-us" ? "Whole" : "Izquierda";
            var leftText = culturecode.ToLower() == "en-us" ? "Whole" : "Derecha";

            if (filterredDescription.Length < 2)
            { 
                var indexofFirstComma = filterredDescription[1].IndexOf(",") + 1;
                var toppings = filterredDescription[1].Substring(indexofFirstComma, filterredDescription[1].Length - indexofFirstComma);

                if (toppings == filterredDescription[1])
                {
                    return false;
                }

                var toppingCount = GetToppingCount(toppings, true, culturecode);

                return toppingCount >= 2;
            }

            for (int i = 2; i < filterredDescription.Length; i++)
            {
                var toppingsToCount = filterredDescription[i].Replace(wholeText, string.Empty).Replace(", " + leftText, string.Empty).Replace(", " + rightText, string.Empty);

                if (filterredDescription[i - 1].Contains(leftText) || filterredDescription[i - 1].Contains(rightText))
                {
                    leftOrRightToppingCount += GetToppingCount(toppingsToCount, false, culturecode);
                }
                else
                {
                    wholeToppingCount = GetToppingCount(toppingsToCount, true, culturecode);
                }

            }

            return wholeToppingCount + leftOrRightToppingCount >= 2;
        }

        private decimal GetToppingCount(string toppings, bool isWholeToppings, string cultureCode)
        {
            decimal toppingCount;
            string extraCheeseText = cultureCode.ToLower() == "en-us" ? "Extra Cheese" : "Mas Queso";
            int extraToppingsCount = Regex.Matches(toppings, "Extra").Count() - Regex.Matches(toppings, extraCheeseText).Count();

            if (isWholeToppings)
            {
                toppingCount = toppings.Split(",").Length + extraToppingsCount * 1m;
            }
            else {
                toppingCount = toppings.Split(",").Length + extraToppingsCount * 0.5m;
            }
            return toppingCount;
        }

        private IList GetInvoice(DateTime startDate, DateTime endDate)
        {
            var query = (from order in orderrepo.table
                         join oi in orderitem.table on order.id equals oi.orderid
                         join p in product.table on oi.ProductId equals p.Id
                         join invoice in invoices.table on order.id equals invoice.OrderId
                         join id in invoicedetails.table on invoice.Id equals id.InvoiceId
                         join f in fundraiser.table on invoice.FundraiserId equals f.Id
                         join g in groups.table on f.GroupId equals g.Id

                         where invoice.InvoiceDate >= startDate && invoice.InvoiceDate <= endDate

                         select new {
                             InvoiceID = invoice.Id,
                             InvoiceDetailsId = id.InvoiceId,
                             TransactionDate = invocie.InvoiceDate,
                             CustomerId = g.Id,
                             AddressId = order.billingaddressid,
                             transactiondetailsid = id.Id,
                             transactiondetailfreight = invoice.Id.toString() + "NF",
                             unitprice = id.groupprice,
                             unitpricefrieght = order.ordershippingtax,
                             extendedprice = oi.quantity * id.groupprice,
                             itemquantity = id.qty,
                             itemnumber = p.sku

                         }).ToList();
                );
        }
    }
}

//Read about fronbody fromform and others

// In order for a class to behave as a controller it has to inherit from ControllerBase Class. Controller provider supports for MVC. We are creating justs an API so we can inherit from just controller
// An API generally returns some sort of data. for that we need to define models 
// to invoke an end point we need to define a route for the controller
// In prod application we use DTO 
// DTOs provide a wrapper and the entity model and what is being exposed from the API
// for example if our database model we have a field which we dont want to expose to end user we setup a DTO without that field
// typically we will have a DB but for now we create a data store

// 18. if we want to get only one villa instead of list of villas
// to specify the confusion between which endpoint should be called we have ot explicitly mention that the get request needs Id param
// If an endpoint needs a parameter it needs to be specified on the http verb along with the data type name

// 19. when we work with API we should define what would be the return type
// actionresuslt is implementtation of type IActionResult

//21
/*In ASP.NET Web API, the[FromBody] attribute is used to specify that a parameter of a Web API controller method should be bound from the request body. It is commonly used when you want to receive data in the HTTP request body, typically in JSON or XML format, and map that data to a parameter in your Web API method.

When you use the [FromBody] attribute, Web API attempts to deserialize the request body content to the specified parameter type. For example, if you have a Web API method that expects an object of type Book, and you send a JSON object representing a book in the request body, Web API will automatically parse and bind that JSON data to the Book parameter using the [FromBody] attribute.*/

// 22

// sometimes we want to send the location url where the resources is created on a posst request for that we use createdAtRoute
{
HeaderInfo:
    {
        TransactionId
        TransactionNumber
        TransactionType
        TransactionDate
        CustomerId
        CountryCode
        AddressId
        SitePurpose
    }
DetailInformation:
    [
        // Pepp Pizza Kit
        {
    TransactionDetailId: 1
            TransactionLineType: "Line"
            CurrencyCode
            UnitPrice
            ExtendedPrice
            Qty
            ItemNumber
            UnitOfMeasure
            LineNumber: 1
        },
        // Chocolate Chunk Cookie
        {
    TransactionDetailId: 2
            TransactionLineType
            CurrencyCode
            UnitPrice
            ExtendedPrice
            Qty
            ItemNumber
            UnitOfMeasure
            LineNumber: 2
        },
        // Freight
        {
    TransactionDetailId: 10NF
    TransactionLineType: Freight
    CurrencyCode: USD
    UnitPrice: 8
            ExtendedPrice: 8
            Qty: 1
            ItemNumber: NULL
            UnitOfMeasure: "EA"
            LineNumber: 3
        }
    ]
}