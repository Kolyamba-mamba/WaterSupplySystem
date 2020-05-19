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
    public class WaterIntake : Point
    {
        public WaterIntake(Coordinate coordinate) : base(coordinate) {}
    }
    
    // Резервуар
    public class Reservoir : Point
    {
        public Reservoir(Coordinate coordinate) : base(coordinate) {}
    }
    
    // Водяной насос
    public class WaterPump : Point
    {
        public WaterPump(Coordinate coordinate) : base(coordinate) {}
    }
    
    // Очистные сооружения
    public class TreatmentFacilities : Point
    {
        public TreatmentFacilities(Coordinate coordinate) : base(coordinate) {}
    }
    
    // Водопровод
    public class Conduit : Point
    {
        public Conduit(Coordinate coordinate) : base(coordinate) {}
    }
    
    // Пользователь
    public class User : Point
    {
        public User(Coordinate coordinate) : base(coordinate) {}
    }
}