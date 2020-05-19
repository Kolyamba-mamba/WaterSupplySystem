using System;
using NetTopologySuite.Geometries;
using OSMLSGlobalLibrary.Modules;

namespace WaterSupplySystemSimulation
{
    public class WaterSupplySystem: OSMLSModule
    {
        protected override void Initialize()
        {
            throw new NotImplementedException();
        }

        public override void Update(long elapsedMilliseconds)
        {
            throw new NotImplementedException();
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