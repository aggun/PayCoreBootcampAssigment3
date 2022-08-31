using Microsoft.AspNetCore.Mvc;
using PycApi.Context;
using PycApi.Model;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PycApi.Controllers
{
    [ApiController]
    [Route("api/nhb/[controller]")]
    public class VehicleContoller : ControllerBase
    {
        //CRUD işlemleri için oluşturulan clasın yapıcı metot içine implemantasyonu
        private readonly IMapperSession<Vehicle> vehicleSession;
        private readonly IMapperSession<Container> containerSession;
        public VehicleContoller(IMapperSession<Vehicle> vehicleSession,
            IMapperSession<Container> containerSession)
        {
            this.vehicleSession = vehicleSession;
            this.containerSession= containerSession;
        }
        //Bütün araçları listeleyen metot.
        [HttpGet("GetVehicleList")]
        public List<Vehicle> GetVehicleList()
        {       
            List<Vehicle> result = vehicleSession.entity.ToList();
            return result;
        }

        //Id bilgisine göre aracı listeleyen metot.
        [HttpGet("GetByVehicleId/{id}")]
        public Vehicle GetByVehicleId(int id)
        {
            Vehicle result = vehicleSession.entity.Where(x => x.Id == id).FirstOrDefault();
            return result;
        }
        //yeni bir araç oluşturan metot.
        [HttpPost("CreateVehicle")]
        public void CreateVehicle([FromBody] Vehicle vehicle)
        {
            try
            {
                vehicleSession.BeginTransaction();
                vehicleSession.Save(vehicle);
                vehicleSession.Commit();
            }
            catch (Exception ex)
            {
                vehicleSession.Rollback();
                Log.Error(ex, "Vehicle Insert Error");
            }
            finally
            {
                vehicleSession.CloseTransaction();
            }
        }
        //aracı güncelleyen metot.
        [HttpPut("UpdateVehicle")]
        public ActionResult<Vehicle> UpdateVehicle([FromBody] Vehicle request)
        {
            Vehicle vehicle = vehicleSession.entity.Where(x => x.Id == request.Id).FirstOrDefault();
            if (vehicle == null)
            {
                return NotFound();
            }

            try
            {
                vehicleSession.BeginTransaction();
                vehicle.Id = vehicle.Id;
                vehicle.VehicleName = request.VehicleName;
                vehicle.VehiclePlate = request.VehiclePlate;

                vehicleSession.Update(vehicle);

                vehicleSession.Commit();
            }
            catch (Exception ex)
            {
                vehicleSession.Rollback();
                Log.Error(ex, "Vehicle Update Error");
            }
            finally
            {
                vehicleSession.CloseTransaction();
            }
            return Ok();
        }

        //aracı ve araca ait conteynırları silen metot.
        [HttpDelete("DeleteVehicle/{id}")]
        public ActionResult<Vehicle> DeleteVehicle(int id)
        {
            Vehicle vehicle = vehicleSession.entity.Where(x => x.Id == id).FirstOrDefault();          
            var container = containerSession.entity.Where(x => x.VehicleId == id);

            if (vehicle == null)
            {
                return NotFound();
            }

            try
            {              
                vehicleSession.BeginTransaction();
                vehicleSession.Delete(vehicle);
                vehicleSession.Commit();
            }
            catch (Exception ex)
            {
                vehicleSession.Rollback();
                Log.Error(ex, "Vehicle Delete Error");
            }
            finally
            {
                vehicleSession.CloseTransaction();
            }
            try
            {
                containerSession.BeginTransaction();
                foreach (var item in container)
                {
                    containerSession.Delete(item);
                }
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
    }
}
