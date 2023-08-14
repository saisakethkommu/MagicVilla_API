using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Interfaces;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;

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

        public VillaAPIController(ILogging logging)
        {
            _logging = logging;
        }

        [HttpGet]
        public ActionResult<IEnumerable<VillaDTO>> GetVillas()
        {
            return Ok(VillaStore.villaList);
        }

        [HttpGet("{Id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)] // this document the response on swagger
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        /*        [ProducesResponseType(200, Type = typeof(VillaDTO))]*/
        public ActionResult<VillaDTO?> GetVilla(int Id) // By defining hte type of response swagger give an example of sample response. ex: ActionResult<VillaDTO> gives sample VillaDTO response
        {
            if (Id == 0)
            {
                _logging.Log("Villa not found", "error");
                return BadRequest();
            }
            var response = VillaStore.villaList.FirstOrDefault(v => v.Id == Id);
            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)] // this document the response on swagger
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<VillaDTO> CreateVilla([FromBody] VillaDTO villa)
        {
            /*            if (!ModelState.IsValid) {
                            return BadRequest(ModelState); not needed if ApiController Annotation is used on the controlled class
                        }*/
            if (VillaStore.villaList.FirstOrDefault(v => v.Name.ToLower() == villa.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError", "Villa already exists"); // first arg is a key valye unique always and second arg is a message. return the model state when doing this
                return BadRequest(ModelState);
            }
            if (villa == null)
            {
                return BadRequest();
            }
            if (villa.Id <= 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            villa.Id = VillaStore.villaList.OrderByDescending(v => v.Id).FirstOrDefault().Id + 1;
            VillaStore.villaList.Add(villa);

            /*            return Ok(villa);*/
            return CreatedAtRoute("GetVilla", new { id = villa.Id }, villa); // this takes 3 args explicit name of the route you want to return, query param for that end point returned if it has one, and the return obj itself
        }

        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteVilla(int villaId)
        { // when if you want to return and object or document return type is ActionResult<T> else use IActionResult
            if (villaId <= 0)
            {
                return BadRequest();
            }
            var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == villaId);
            if (villa == null)
            {
                _logging.Log("Villa could not be found", "error");
                return NotFound();
            }
            VillaStore.villaList.Remove(villa);
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
        public IActionResult UpdateVillaDto(int villaId, [FromBody] VillaDTO villa)
        {
            if (villaId <= 0 || villa.Id != villaId)
            {
                return BadRequest();
            }
            var villaDetails = VillaStore.villaList.FirstOrDefault(x => x.Id == villaId);
            if (villaDetails != null)
            {
                villaDetails.Id = villa.Id;
                villaDetails.Name = villa.Name;
                villaDetails.Occupancy = villa.Occupancy;
                villaDetails.Sqft = villa.Sqft;
                return Ok(true);
            }
            return NoContent();
        }

        [HttpPatch("{id:int}", Name = "PartialUpdateVilla")]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult PartialVillaUpdate(int villaId, JsonPatchDocument<VillaDTO> patchVilla)
        {
            if (villaId <= 0 || patchVilla == null)
            {
                return BadRequest();
            }
            var villa = VillaStore.villaList.FirstOrDefault(x => x.Id == villaId);
            if (villa != null)
            {
                patchVilla.ApplyTo(villa, ModelState); // patchVilla allows you to update only specific fields when sending request to this end point now
                if (ModelState.IsValid) {
                    return NoContent();
                }
                return BadRequest();
            }
            return BadRequest();
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
