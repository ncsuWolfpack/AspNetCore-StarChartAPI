using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var query = from co in _context.CelestialObjects
                        where co.Id == id
                        select co;

            var celestialObject = query.SingleOrDefault();
            if (celestialObject == null)
            {
                return NotFound();
            }
            else
            {
                celestialObject.Satellites = GetSatellites(id);

                return Ok(celestialObject);
            }
        }

        [HttpGet("{name}", Name ="GetByName")]
        public IActionResult GetByName(string name)
        {
            var query = from co in _context.CelestialObjects
                        where co.Name.ToUpper() == name.ToUpper()
                        select co;

            var celestialObjects = query.ToList();

            if (celestialObjects.Any())
            {
                foreach (var co in celestialObjects)
                {
                    co.Satellites = GetSatellites(co.Id);
                }

                return Ok(celestialObjects);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet(Name = "GetAll")]
        public IActionResult GetAll()
        {
            var query = from co in _context.CelestialObjects
                        select co;

            var celestialObjects = query.ToList();

            if (celestialObjects.Any())
            {
                foreach (var co in celestialObjects)
                {
                    co.Satellites = GetSatellites(co.Id);
                }

                return Ok(celestialObjects);
            }
            else
            {
                return NotFound();
            }
        }

        private List<CelestialObject> GetSatellites(int celestialObjectId)
        {
            var satQuery = from s in _context.CelestialObjects
                           where s.OrbitedObjectId == celestialObjectId
                           select s;

            return satQuery.ToList();
        }

    }
}
