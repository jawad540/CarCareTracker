using System.Collections.Generic;
using System.Linq;
using CarCareTracker.Models;

namespace CarCareTracker.Patterns.Strategy
{
    // ============================================================
    //  STRATEGY DESIGN PATTERN
    //  ------------------------------------------------------------
    //  Purpose: Calculate fuel efficiency using interchangeable
    //  algorithms (metric vs imperial) without changing the client.
    //
    //  Participants:
    //    - IFuelEfficiencyStrategy   (Strategy interface)
    //    - KmPerLiterStrategy        (Concrete Strategy A)
    //    - LitersPer100KmStrategy    (Concrete Strategy B)
    //    - FuelEfficiencyCalculator  (Context)
    // ============================================================

    /// <summary>The Strategy interface - common to all algorithms.</summary>
    public interface IFuelEfficiencyStrategy
    {
        string Name { get; }
        string Unit { get; }
        double Calculate(double totalLiters, double distanceKm);
    }

    /// <summary>Concrete Strategy A: kilometers per liter (km/L).</summary>
    public class KmPerLiterStrategy : IFuelEfficiencyStrategy
    {
        public string Name { get { return "Kilometers per Liter"; } }
        public string Unit { get { return "km/L"; } }

        public double Calculate(double totalLiters, double distanceKm)
        {
            if (totalLiters <= 0) return 0;
            return distanceKm / totalLiters;
        }
    }

    /// <summary>Concrete Strategy B: liters per 100 km (L/100km).</summary>
    public class LitersPer100KmStrategy : IFuelEfficiencyStrategy
    {
        public string Name { get { return "Liters per 100 km"; } }
        public string Unit { get { return "L/100km"; } }

        public double Calculate(double totalLiters, double distanceKm)
        {
            if (distanceKm <= 0) return 0;
            return (totalLiters / distanceKm) * 100.0;
        }
    }

    /// <summary>
    /// The Context: uses a strategy to compute fuel efficiency.
    /// The strategy can be swapped at runtime.
    /// </summary>
    public class FuelEfficiencyCalculator
    {
        private IFuelEfficiencyStrategy _strategy;

        public FuelEfficiencyCalculator(IFuelEfficiencyStrategy strategy)
        {
            _strategy = strategy;
        }

        public void SetStrategy(IFuelEfficiencyStrategy strategy)
        {
            _strategy = strategy;
        }

        public double Compute(IEnumerable<FuelRecord> records)
        {
            var list = records as IList<FuelRecord> ?? records.ToList();
            if (list.Count < 2) return 0;

            double totalLiters = list.Sum(r => (double)r.Liters);
            long minOdo = list.Min(r => r.OdometerReading);
            long maxOdo = list.Max(r => r.OdometerReading);
            double distance = maxOdo - minOdo;

            return _strategy.Calculate(totalLiters, distance);
        }

        public string ResultLabel(double value)
        {
            return value.ToString("0.00") + " " + _strategy.Unit + " (" + _strategy.Name + ")";
        }
    }
}
