using System;
using System.Collections.Generic;
using System.Linq;
using NetTopologySuite.Geometries;
using OSMLSGlobalLibrary.Modules;

namespace WaterSupplySystemSimulation
{
    public class WaterSupplySystem: OSMLSModule
    {
        Coordinate waterIntakeCoord = new Coordinate(4971565, 6250411);
        Coordinate waterPumpCoord = new Coordinate(4970979, 6250473);
        Coordinate treatmentFacilitiesCoord = new Coordinate(4968216, 6250901);
        Coordinate reservoirCoord = new Coordinate(4968230, 6251527);
        protected override void Initialize()
        {
            var waterIntake = new WaterIntake(waterIntakeCoord);
            MapObjects.Add(waterIntake);
            
            var waterPump = new WaterPump(waterPumpCoord);
            MapObjects.Add(waterPump);
            
            var treatmentFacilities = new TreatmentFacilities(treatmentFacilitiesCoord);
            MapObjects.Add(treatmentFacilities);
            
            var reservoir = new Reservoir(reservoirCoord);
            MapObjects.Add(reservoir);

            var pipeline = new Pipeline(new []
            {
                waterIntakeCoord, waterPumpCoord,
                treatmentFacilitiesCoord, reservoirCoord
            });
            MapObjects.Add(pipeline);

            var conduit = new List<Conduit>
            {
                new Conduit(new []
                {
                    reservoirCoord,
                    new Coordinate(4969525, 6245020), new Coordinate(4959206, 6230638),
                    new Coordinate(4956179, 6225452), new Coordinate(4951146, 6220512)
                }),
                new Conduit(new []
                {
                    reservoirCoord,
                    new Coordinate(4968154, 6246190), new Coordinate(4961336, 6236609), 
                    new Coordinate(4954702, 6227112), new Coordinate(4949605, 6221690)  
                }),
                new Conduit(new []
                {
                    reservoirCoord,
                    new Coordinate(4967380, 6246826), new Coordinate(4948304, 6222447) 
                }), 
                new Conduit(new []
                {
                    reservoirCoord,
                    new Coordinate(4946403, 6223302)
                }),
                new Conduit(new []
                {
                    reservoirCoord,
                    new Coordinate(4963582, 6249539), new Coordinate(4943335, 6223579) 
                }),
                new Conduit(new []
                {
                    reservoirCoord,
                    new Coordinate(4961002, 6251345), new Coordinate(4934220, 6221640)  
                }),
                new Conduit(new []
                {
                    new Coordinate(4951146, 6220512), new Coordinate(4949605, 6221690),
                    new Coordinate(4948304, 6222447), new Coordinate(4946403, 6223302),
                    new Coordinate(4943335, 6223579), new Coordinate(4934220, 6221640)
                })
            };
            foreach (var elem in conduit)
            {
                MapObjects.Add(elem);
            }
            
            var user = new User(new Coordinate(4950600, 6223050));
            MapObjects.Add(user);
        }

        public override void Update(long elapsedMilliseconds)
        {
            var waterPump = MapObjects.Get<WaterPump>()[0];
            var treatmentFacilities = MapObjects.Get<TreatmentFacilities>()[0];
            var reservoir = MapObjects.Get<Reservoir>()[0];
            var conduit = MapObjects.GetAll<Conduit>();
            var users = MapObjects.GetAll<User>();
            var riverWater = MapObjects.GetAll<RiverWater>();
            var cleanWater = MapObjects.GetAll<CleanWater>();
            
            if (MapObjects.Get<RiverWater>().Count == 0)
            {
                var water = new RiverWater(new Coordinate(waterIntakeCoord), 
                    MoveValue(waterIntakeCoord, waterPumpCoord, 200));
                MapObjects.Add(water);
            }

            if (riverWater != null)
            {
                foreach (var water in riverWater)
                {
                    water.Move();
                    if (water.InPlace(waterPump))
                    {
                        (water._moveX, water._moveY) =
                            MoveValue(waterPumpCoord, treatmentFacilitiesCoord, 1000);
                    }

                    if (water.InPlace(treatmentFacilities))
                    {
                        (water._moveX, water._moveY) =
                            MoveValue(treatmentFacilitiesCoord, reservoirCoord, 200);
                    }

                    if (water.InPlace(reservoir))
                    {
                        MapObjects.Remove(water);
                    }
                }
            }

            if (users == null) return;
            {
                foreach (var user in users)
                {
                    if (user.flag) continue;
                    var waterToUserCoord = user.GetNearestPoint(conduit);
                    var waterToUser = new CleanWater(waterToUserCoord,
                        MoveValue(waterToUserCoord, user.Coordinate, 50));
                    MapObjects.Add(waterToUser);
                    user.flag = true;
                }
                
                if (cleanWater == null) return;
                foreach (var water in cleanWater)
                {
                    foreach (var user in users.Where(user => water.InPlace(user)))
                    {
                        MapObjects.Remove(water);
                        MapObjects.Remove(user);
                    }
                    water.Move();
                }
            }
        }

        private static (double, double) MoveValue(Coordinate start, Coordinate end, int value) 
            => (((end.X - start.X) / value), (end.Y - start.Y) / value);
    }

    public static class PointExtension
    {
        public static Coordinate GetNearestPoint(this Geometry point, IEnumerable<Conduit> conduits)
        {
            var min = double.MaxValue;
            Coordinate coordinateFirstPoint = null;
            Coordinate coordinateSecondPoint = null;
            Conduit usingConduit = null;
            foreach (var conduit in conduits)
            {
                foreach (var coordinate in conduit.Coordinates)
                {
                    var dist = point.Coordinate.Distance(coordinate);
                    if (!(dist < min)) continue;
                    min = dist;
                    usingConduit = conduit;
                    coordinateFirstPoint = coordinate;
                }  
            }
            
            min = double.MaxValue;
            foreach (var coordinate in usingConduit?.Coordinates)
            {
                var dist = point.Coordinate.Distance(coordinate);
                if ((!(dist < min)) || (coordinate == coordinateFirstPoint)) continue;
                min = dist;
                coordinateSecondPoint = coordinate;
            }

            return point.Coordinate.SearchingHeightCoordinate(coordinateFirstPoint, coordinateSecondPoint);
        }
    }

    public static class CoordinateExtension
    {
        public static Coordinate SearchingHeightCoordinate(this Coordinate A, Coordinate B, Coordinate C)
        {
            var Ax = C.X - B.X;
            var Ay = C.Y - B.Y;
            var Tmin = (Ax * (A.X - B.X) + Ay * (A.Y - B.Y)) / (Math.Pow(Ax, 2) + Math.Pow(Ay, 2));
            var X = B.X + Ax * Tmin;
            var Y = B.Y + Ay * Tmin;
            return new Coordinate(X, Y);
        }
    }
}