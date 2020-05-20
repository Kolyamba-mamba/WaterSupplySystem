using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using OSMLSGlobalLibrary.Map;
using OSMLSGlobalLibrary.Modules;

namespace WaterSupplySystemSimulation
{
    public class WaterSupplySystem: OSMLSModule
    {
        protected override void Initialize()
        {
            var waterIntakeCoord = new Coordinate(4971565, 6250411);
            var waterIntake = new WaterIntake(waterIntakeCoord);
            MapObjects.Add(waterIntake);

            var waterPumpCoord = new Coordinate(4970979, 6250473);
            var waterPump = new WaterPump(waterPumpCoord);
            MapObjects.Add(waterPump);

            var treatmentFacilitiesCoord = new Coordinate(4968216, 6250901);
            var treatmentFacilities = new TreatmentFacilities(treatmentFacilitiesCoord);
            MapObjects.Add(treatmentFacilities);
            
            var reservoirCoord = new Coordinate(4968230, 6251527);
            var reservoir = new Reservoir(reservoirCoord);
            MapObjects.Add(reservoir);

            var conduit = new List<Conduit>
            {
                new Conduit(new []
                {
                    reservoirCoord,
                    new Coordinate(4969525, 6245020), new Coordinate(4959206, 6230638),
                    new Coordinate(4956179, 6225452), new Coordinate(4951146, 6220512),
                }),
                new Conduit(new []
                {
                    reservoirCoord,
                    new Coordinate(4968154, 6246190), new Coordinate(4961336, 6236609), 
                    new Coordinate(4954702, 6227112), new Coordinate(4949605, 6221690),  
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
                }) 
            };
            foreach (var elem in conduit)
            {
                MapObjects.Add(elem);
            }
        }

        public override void Update(long elapsedMilliseconds)
        { }
    }

    public static class PointExtension
    {
        public static double Distance(this Point p1, Point p2) => Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
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
}