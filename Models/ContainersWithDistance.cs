using System.Collections.Generic;

namespace Waste.Models
{
    public class ContainersWithDistance
    {
        public ContainersWithDistance(List<Container> containers, double distance)
        {
            Containers = containers;
            Distance = distance;
        }

        public List<Container> Containers { get; set; } 
        public double Distance { get; set; } 
    }

}
