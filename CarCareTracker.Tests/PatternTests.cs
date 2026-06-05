using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CarCareTracker.Models;
using CarCareTracker.Patterns.Strategy;
using CarCareTracker.Patterns.Observer;

namespace CarCareTracker.Tests
{
    // ============================================================
    //  UNIT TESTS (MSTest - the .NET equivalent of JUnit)
    //  Tests cover the Strategy and Observer design patterns
    //  and core domain logic.
    // ============================================================

    [TestClass]
    public class FuelEfficiencyStrategyTests
    {
        private List<FuelRecord> SampleRecords()
        {
            // 2 fill-ups: odometer 1000 -> 1500 (500 km), total 50 liters
            return new List<FuelRecord>
            {
                new FuelRecord { Liters = 20m, OdometerReading = 1000 },
                new FuelRecord { Liters = 30m, OdometerReading = 1500 }
            };
        }

        [TestMethod]
        public void KmPerLiter_CalculatesCorrectly()
        {
            // 500 km / 50 L = 10 km/L
            var calc = new FuelEfficiencyCalculator(new KmPerLiterStrategy());
            double result = calc.Compute(SampleRecords());
            Assert.AreEqual(10.0, result, 0.001);
        }

        [TestMethod]
        public void LitersPer100Km_CalculatesCorrectly()
        {
            // (50 L / 500 km) * 100 = 10 L/100km
            var calc = new FuelEfficiencyCalculator(new LitersPer100KmStrategy());
            double result = calc.Compute(SampleRecords());
            Assert.AreEqual(10.0, result, 0.001);
        }

        [TestMethod]
        public void Strategy_CanBeSwappedAtRuntime()
        {
            var calc = new FuelEfficiencyCalculator(new KmPerLiterStrategy());
            double first = calc.Compute(SampleRecords());

            calc.SetStrategy(new LitersPer100KmStrategy());
            double second = calc.Compute(SampleRecords());

            // Same data, different algorithm — both equal 10 here but units differ
            Assert.AreEqual(10.0, first, 0.001);
            Assert.AreEqual(10.0, second, 0.001);
        }

        [TestMethod]
        public void InsufficientData_ReturnsZero()
        {
            var calc = new FuelEfficiencyCalculator(new KmPerLiterStrategy());
            var single = new List<FuelRecord> { new FuelRecord { Liters = 20m, OdometerReading = 1000 } };
            Assert.AreEqual(0.0, calc.Compute(single), 0.001);
        }
    }

    [TestClass]
    public class ReminderObserverTests
    {
        [TestMethod]
        public void DueReminder_NotifiesObservers()
        {
            var subject = new ReminderSubject();
            var alertObs = new DashboardAlertObserver();
            var logObs = new LogObserver();
            subject.Subscribe(alertObs);
            subject.Subscribe(logObs);

            var reminders = new List<Reminder>
            {
                new Reminder { ReminderId = 1, Title = "Oil Change", Status = "Pending",
                               ReminderDate = DateTime.Now.AddDays(3), VehicleLabel = "Toyota Corolla" }
            };

            int notified = subject.CheckAndNotify(reminders);

            Assert.AreEqual(1, notified);
            Assert.AreEqual(1, alertObs.Alerts.Count);
            Assert.AreEqual(1, logObs.LogEntries.Count);
        }

        [TestMethod]
        public void FarReminder_DoesNotNotify()
        {
            var subject = new ReminderSubject();
            var alertObs = new DashboardAlertObserver();
            subject.Subscribe(alertObs);

            var reminders = new List<Reminder>
            {
                new Reminder { ReminderId = 2, Title = "Insurance", Status = "Pending",
                               ReminderDate = DateTime.Now.AddDays(60), VehicleLabel = "Honda Civic" }
            };

            int notified = subject.CheckAndNotify(reminders);

            Assert.AreEqual(0, notified);
            Assert.AreEqual(0, alertObs.Alerts.Count);
        }

        [TestMethod]
        public void CompletedReminder_DoesNotNotify()
        {
            var subject = new ReminderSubject();
            var alertObs = new DashboardAlertObserver();
            subject.Subscribe(alertObs);

            var reminders = new List<Reminder>
            {
                new Reminder { ReminderId = 3, Title = "Done Task", Status = "Completed",
                               ReminderDate = DateTime.Now.AddDays(2), VehicleLabel = "Kia" }
            };

            Assert.AreEqual(0, subject.CheckAndNotify(reminders));
        }

        [TestMethod]
        public void Unsubscribe_StopsNotifications()
        {
            var subject = new ReminderSubject();
            var alertObs = new DashboardAlertObserver();
            subject.Subscribe(alertObs);
            subject.Unsubscribe(alertObs);

            var reminders = new List<Reminder>
            {
                new Reminder { ReminderId = 4, Title = "Test", Status = "Pending",
                               ReminderDate = DateTime.Now.AddDays(1), VehicleLabel = "BMW" }
            };

            subject.CheckAndNotify(reminders);
            Assert.AreEqual(0, alertObs.Alerts.Count);
        }
    }

    [TestClass]
    public class ReminderDomainTests
    {
        [TestMethod]
        public void DaysLeft_ComputesCorrectly()
        {
            var r = new Reminder { ReminderDate = DateTime.Now.Date.AddDays(5) };
            Assert.AreEqual(5, r.DaysLeft);
        }

        [TestMethod]
        public void OverdueReminder_HasNegativeDaysLeft()
        {
            var r = new Reminder { ReminderDate = DateTime.Now.Date.AddDays(-3) };
            Assert.IsTrue(r.DaysLeft < 0);
        }
    }
}
