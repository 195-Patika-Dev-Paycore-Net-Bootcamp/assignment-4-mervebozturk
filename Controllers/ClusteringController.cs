using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Waste.Models;
using Waste.Utilities;
 
namespace Waste.Controllers
{
    [ApiController]
    [Route("api/nhb/[controller]")]
    public class ClusteringController : ControllerBase
    {
        private readonly ISession _session; 

        public ClusteringController(ISession session) => _session = session; 

        
        [HttpGet("{vehicleId}/{clusterCount}")]
        public async Task<ActionResult<List<List<ContainersWithDistance>>>> Get(long? vehicleId, int? clusterCount)
        {
            if (vehicleId == null) return BadRequest("Vehicle id can't be null."); // Vehicle Id girilmezse geçersiz istek hatası verecek
            if (clusterCount == null) return BadRequest("Cluster count can't be null."); // Küme sayısı girilmezse geçersiz istek hatası verecek

            var vehicle = await _session.Query<Vehicle>().FirstOrDefaultAsync(c => c.Id == vehicleId); 
            if (vehicle == null) return NotFound($"No vehicle were found for this ID : ({vehicleId})."); // Geçersiz araç girilirse bulunamadı hatası verecek

            var containers = await _session.Query<Container>().Where(c => c.VehicleId == vehicleId).ToListAsync(); 
            if (containers.Count == 0) return NotFound($"No containers were found for this vehicle : {vehicle.VehicleName}.\nContainer count must be at least 1."); // Eğer hiç konteyner yoksa "Bulunamadı" hatası döndürülür.
            if (containers.Count < clusterCount) return BadRequest($"Cluster count ({clusterCount}) must be less than container count ({containers.Count})."); // 

            var list = new List<ContainersWithDistance>();
            for (int i = 0; i < containers.Count - 1; i++) // Sırayla tüm mesafelerin hesaplanabilmesi için gerekli döngüsel işlem başlatılır.
                list.Add(Calculation.GetDistance(containers[i], containers[i + 1])); // Uzaklık bilgileri de eklenecek

            var clusteredList = Calculation.Cluster(list, (int)clusterCount); 
            return Ok(clusteredList); 
        }
    }
    }
