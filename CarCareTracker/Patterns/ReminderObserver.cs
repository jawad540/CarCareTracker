using System;
using System.Collections.Generic;
using CarCareTracker.Models;

namespace CarCareTracker.Patterns.Observer
{
    // ============================================================
    //  OBSERVER DESIGN PATTERN
    //  ------------------------------------------------------------
    //  Purpose: When a reminder becomes "due", notify all registered
    //  observers (e.g. dashboard badge, log, email stub) automatically.
    //
    //  Participants:
    //    - IReminderObserver      (Observer interface)
    //    - ReminderSubject        (Subject / Observable)
    //    - DashboardAlertObserver (Concrete Observer A)
    //    - LogObserver            (Concrete Observer B)
    // ============================================================

    /// <summary>The Observer interface - implemented by all listeners.</summary>
    public interface IReminderObserver
    {
        void OnReminderDue(Reminder reminder);
    }

    /// <summary>
    /// The Subject (Observable): keeps a list of observers and
    /// notifies them when a reminder is due.
    /// </summary>
    public class ReminderSubject
    {
        private readonly List<IReminderObserver> _observers = new List<IReminderObserver>();

        public void Subscribe(IReminderObserver observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);
        }

        public void Unsubscribe(IReminderObserver observer)
        {
            _observers.Remove(observer);
        }

        /// <summary>Checks reminders and notifies observers for any that are due.</summary>
        public int CheckAndNotify(IEnumerable<Reminder> reminders)
        {
            int notifiedCount = 0;
            foreach (var r in reminders)
            {
                if (r.Status == "Pending" && r.DaysLeft <= 7)
                {
                    foreach (var observer in _observers)
                        observer.OnReminderDue(r);
                    notifiedCount++;
                }
            }
            return notifiedCount;
        }
    }

    /// <summary>Concrete Observer A: collects alerts for the dashboard badge.</summary>
    public class DashboardAlertObserver : IReminderObserver
    {
        public List<string> Alerts { get; private set; }

        public DashboardAlertObserver()
        {
            Alerts = new List<string>();
        }

        public void OnReminderDue(Reminder reminder)
        {
            Alerts.Add(string.Format("'{0}' is due in {1} day(s) for {2}.",
                reminder.Title, reminder.DaysLeft, reminder.VehicleLabel));
        }
    }

    /// <summary>Concrete Observer B: writes the notification to a log.</summary>
    public class LogObserver : IReminderObserver
    {
        public List<string> LogEntries { get; private set; }

        public LogObserver()
        {
            LogEntries = new List<string>();
        }

        public void OnReminderDue(Reminder reminder)
        {
            LogEntries.Add(string.Format("[{0:yyyy-MM-dd HH:mm}] Reminder #{1} due: {2}",
                DateTime.Now, reminder.ReminderId, reminder.Title));
        }
    }
}
