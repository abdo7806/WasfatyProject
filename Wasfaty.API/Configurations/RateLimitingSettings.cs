namespace Wasfaty.API.Configurations
{
    public class RateLimitingSettings
    {
        public GlobalSettings Global { get; set; } = new();
        public StrictAuthSettings StrictAuth { get; set; } = new();
        public WriteOperationsSettings WriteOperations { get; set; } = new();
        public DashboardSettings Dashboard { get; set; } = new();
        public PollingSettings Polling { get; set; } = new();
        public MultipleGetSettings MultipleGet { get; set; } = new();
    }

    public class GlobalSettings
    {
        public int PermitLimit { get; set; } = 150;
        public int WindowMinutes { get; set; } = 1;
        public int QueueLimit { get; set; } = 3;
    }

    public class StrictAuthSettings
    {
        public int PermitLimit { get; set; } = 5;
        public int WindowMinutes { get; set; } = 1;
        public int QueueLimit { get; set; } = 0;
    }

    public class WriteOperationsSettings
    {
        public int PermitLimit { get; set; } = 50;
        public int WindowMinutes { get; set; } = 1;
        public int QueueLimit { get; set; } = 0;
    }

    public class DashboardSettings
    {
        public int PermitLimit { get; set; } = 30;
        public int WindowMinutes { get; set; } = 1;
        public int QueueLimit { get; set; } = 0;
    }

    public class PollingSettings
    {
        public int PermitLimit { get; set; } = 15;
        public int WindowMinutes { get; set; } = 1;
        public int SegmentsPerWindow { get; set; } = 4;
    }

    public class MultipleGetSettings
    {
        public int PermitLimit { get; set; } = 5;
        public int QueueLimit { get; set; } = 2;
    }
}
