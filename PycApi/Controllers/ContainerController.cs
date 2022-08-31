using Microsoft.AspNetCore.Mvc;
using PycApi.Context;
using PycApi.Model;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PycApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContainerController : ControllerBase
    {
        //CRUD işlemleri için oluşturulan clasın yapıcı metot içine implemantasyonu
        private readonly IMapperSession<Container> containerSession;
        private readonly IMapperSession<Vehicle> vehicleSession;

        public ContainerController(IMapperSession<Container> containerSession, IMapperSession<Vehicle> vehicleSession)
        {
            this.containerSession = containerSession;
            this.vehicleSession = vehicleSession;
        }
        //bütün konteynırları listeleyen metot
        [HttpGet("GetContainerList")]
        public List<Container> GetContainerList()
        {
            List<Container> result = containerSession.entity.ToList();
            return result;
        }

        //araca ait konteynırları lisleyen metot
        [HttpGet("GetContainerByVehicleId/{id}")]
        public IActionResult GetContainerByVehicleId(int id)
        {

            var result = containerSession.entity.Where(x => x.VehicleId == id).ToList();
            return Ok(result);
        }
        //yeni biir konteynır oluşturan metot
        [HttpPost("CreateContainer")]
        public void CreateContainer([FromBody] Container container)
        {
            try
            {
                containerSession.BeginTransaction();
                containerSession.Save(container);
                containerSession.Commit();
            }
            catch (Exception ex)
            {
                containerSession.Rollback();
                Log.Error(ex, "Container Insert Error");
            }
            finally
            {
                containerSession.CloseTransaction();
            }
        }
        //konteynırı güncelleyen metot
        [HttpPut("UpdateContainer")]
        public ActionResult<Container> UpdateContainer([FromBody] Container request)
        {
            Container container = containerSession.entity.Where(x => x.Id == request.Id).FirstOrDefault();
            if (container == null)
            {
                return NotFound();
            }

            try
            {
                containerSession.BeginTransaction();
                container.VehicleId = container.VehicleId;
                container.Id = request.Id;
                container.ContainerName = request.ContainerName;
                container.Latitude = request.Latitude;
                container.Longitude = request.Longitude;

                containerSession.Update(container);

                containerSession.Commit();
            }
            catch (Exception ex)
            {
                containerSession.Rollback();
                Log.Error(ex, "Container Update Error");
            }
            finally
            {
                containerSession.CloseTransaction();
            }
            return Ok();
        }
        //konteynırı silen metot
        [HttpDelete("DeleteContainer/{id}")]
        public ActionResult<Container> DeleteContainer(int id)
        {
            Container container = containerSession.entity.Where(x => x.Id == id).FirstOrDefault();

            if (container == null)
            {
                return NotFound();
            }

            try
            {
                containerSession.BeginTransaction();
                containerSession.Delete(container);
                containerSession.Commit();
            }
            catch (Exception ex)
            {
                containerSession.Rollback();
                Log.Error(ex, "Container Delete Error");
            }
            finally
            {
                containerSession.CloseTransaction();
            }

            return Ok();
        }
        //konteynırları kümeleyerek geri döndüren metot
        [HttpGet("GetContainerInGroup")]
        public IActionResult GetContainerInGroup(int id, int count)
        {
          List < Container > containerlist = containerSession.entity.Where(x => x.VehicleId == id).ToList();
            var results = containerlist.Select((x, i) => new { Key = i % count, Value = x })
                .GroupBy(x => x.Key, x => x.Value).ToList();          
            return Ok(results);
        }
        
    }
}
