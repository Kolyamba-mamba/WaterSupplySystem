using System;
using System.Collections.Generic;
using System.Linq;
using NetTopologySuite.Geometries;
using OSMLSGlobalLibrary.Map;
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
        }

        public override void Update(long elapsedMilliseconds)
        {
            if (MapObjects.Get<Water>().Count == 0)
            {
                var waterToPump = new Water(new Coordinate(waterIntakeCoord), -2.93, moveY: 0.31 );
                MapObjects.Add(waterToPump);
            }
            var water = MapObjects.Get<Water>()[0];
            var waterPump = MapObjects.Get<WaterPump>()[0];
            var treatmentFacilities = MapObjects.Get<TreatmentFacilities>()[0];
            var reservoir = MapObjects.Get<Reservoir>()[0];
            water.Move();
            if (water.InPlace(waterPump))
            {
                (water._moveX, water._moveY) = (-2.763 , 0.428);
            }

            if (water.InPlace(treatmentFacilities))
            {
                (water._moveX, water._moveY) = (0.07, 3.13);
            }
            if (water.InPlace(reservoir))
            {
                MapObjects.Remove(water);
            }
        }

        private Coordinate GetNearestPoint(Geometry point, IEnumerable<Conduit> conduits)
        {
            var min = double.MaxValue;
            Coordinate result = null;
            foreach (var coordinate in conduits.SelectMany(conduit => conduit.Coordinates))
            {
                var dist = PointExtension.Distance(point.Coordinate, coordinate);
                if (!(dist < min)) continue;
                min = dist;
                result = coordinate;
            }
            return result;
        }
    }

    public static class PointExtension
    {
        public static double Distance(this Point p1, Point p2) => Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        public static double Distance(this Coordinate c1, Coordinate c2) 
            => Math.Sqrt(Math.Pow(c1.X - c2.X, 2) + Math.Pow(c1.Y - c2.Y, 2));
        
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
    
    // Водозаборное сооружение
    [CustomStyle(
        @"new ol.style.Style({
        image: new ol.style.Circle({
            opacity: 1.0,
            scale: 1.0,
            radius: 6,
            fill: new ol.style.Fill({
                color: 'rgba(0, 0, 0, 0.8)'
            }),
            stroke: new ol.style.Stroke({
                color: 'rgba(0, 0, 0, 0.4)',
                width: 1
            }),
        })
    });
    ")]
    public class WaterIntake : Point
    {
        public WaterIntake(Coordinate coordinate) : base(coordinate) {}
    }
    
    // Резервуар
    [CustomStyle(
        @"new ol.style.Style({
        image: new ol.style.Circle({
            opacity: 1.0,
            scale: 1.0,
            radius: 30,
            fill: new ol.style.Fill({
                color: 'rgba(51, 255, 255, 0.8)'
            }),
            stroke: new ol.style.Stroke({
                color: 'rgba(0, 0, 0, 0.4)',
                width: 1
            }),
        })
    });
    ")]
    public class Reservoir : Point
    {
        public Reservoir(Coordinate coordinate) : base(coordinate) {}
    }
    
    // Водяной насос
    [CustomStyle(
        @"new ol.style.Style({
        image: new ol.style.Circle({
            opacity: 1.0,
            scale: 1.0,
            radius: 3,
            fill: new ol.style.Fill({
                color: 'rgba(0, 255, 0, 0.8)'
            }),
            stroke: new ol.style.Stroke({
                color: 'rgba(0, 0, 0, 0.4)',
                width: 1
            }),
        })
    });
    ")]
    public class WaterPump : Point
    {
        public WaterPump(Coordinate coordinate) : base(coordinate) {}
    }
    
    // Очистные сооружения
    [CustomStyle(
        @"new ol.style.Style({
        image: new ol.style.Circle({
            opacity: 1.0,
            scale: 1.0,
            radius: 18,
            fill: new ol.style.Fill({
                color: 'rgba(102, 0, 204, 0.8)'
            }),
            stroke: new ol.style.Stroke({
                color: 'rgba(0, 0, 0, 0.4)',
                width: 1
            }),
        })
    });
    ")]
    public class TreatmentFacilities : Point
    {
        public TreatmentFacilities(Coordinate coordinate) : base(coordinate) {}
    }
    
    // Водопровод
    [CustomStyle(
        @"new ol.style.Style({
            stroke: new ol.style.Stroke({
                color: 'blue',
                width: 2
            })
        })"
        )]
    public class Conduit : LineString
    {
        public Conduit(Coordinate[] points) : base(points) { }
    }

    [CustomStyle(
        @"new ol.style.Style({
            stroke: new ol.style.Stroke({
                color: 'rgba(0, 0, 0, 0.2)',
                width: 2
            })
        })"
    )]
    public class Pipeline : LineString
    {
        public Pipeline(Coordinate[] points) : base(points) { }
    }
    
    // Пользователь
    [CustomStyle(
        @"new ol.style.Style({
        image: new ol.style.Circle({
            opacity: 1.0,
            scale: 1.0,
            radius: 2,
            fill: new ol.style.Fill({
                color: 'rgba(255, 0, 0, 0.8)'
            }),
            stroke: new ol.style.Stroke({
                color: 'rgba(0, 0, 0, 0.4)',
                width: 1
            }),
        })
    });
    ")]
    public class User : Point
    {
        public User(Coordinate coordinate) : base(coordinate) {}
    }

    [CustomStyle(
        @"new ol.style.Style({
        image: new ol.style.Circle({
            opacity: 1.0,
            scale: 1.0,
            radius: 3,
            fill: new ol.style.Fill({
                color: 'rgba(51, 153, 255, 0.8)'
            }),
            stroke: new ol.style.Stroke({
                color: 'rgba(0, 0, 0, 0.4)',
                width: 1
            }),
        })
    });
    ")]
    public class Water : Point
    {
        public double _moveX { get; set; }
        public double _moveY { get; set; }

        public Water(Coordinate coordinate, double moveX, double moveY) : base(coordinate)
        {
            _moveX = moveX;
            _moveY = moveY;
        }

        public bool InPlace(Point point)
        {
            return PointExtension.Distance(this, point) < 10;
        }

        public void Move()
        {
            X += _moveX;
            Y += _moveY;
        }
    }
}