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

        [HttpGet("{name}", Name = "GetByName")]
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

        [HttpPost(Name = "Create")]
        public IActionResult Create([FromBody]CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new { celestialObject.Id }, celestialObject);
        }

        [HttpPut("{id}", Name = "Update")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            var query = from co in _context.CelestialObjects
                        where co.Id == id
                        select co;

            var modelObject = query.SingleOrDefault();

            if (modelObject == null)
            {
                return NotFound();
            }
            else
            {
                modelObject.Name = celestialObject.Name;
                modelObject.OrbitalPeriod = celestialObject.OrbitalPeriod;
                modelObject.OrbitedObjectId = celestialObject.OrbitedObjectId;

                _context.Update(modelObject);
                _context.SaveChanges();

                return NoContent();
            }
        }

        [HttpPatch("{id}/{name}", Name = "RenameObject")]
        public IActionResult RenameObject(int id, string name)
        {
            var query = from co in _context.CelestialObjects
                        where co.Id == id
                        select co;

            var modelObject = query.SingleOrDefault();

            if (modelObject == null)
            {
                return NotFound();
            }
            else
            {
                modelObject.Name = name;

                _context.Update(modelObject);
                _context.SaveChanges();

                return NoContent();
            }
        }

        [HttpDelete("{id}", Name ="Delete")]
        public IActionResult Delete(int id)
        {
            var query = from co in _context.CelestialObjects
                        where co.Id == id || co.OrbitedObjectId == id
                        select co;

            var objectsToDelete = query.ToList();

            if (objectsToDelete.Any())
            {
                _context.RemoveRange(objectsToDelete);
                _context.SaveChanges();

                return NoContent();
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
