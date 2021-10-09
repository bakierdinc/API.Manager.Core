namespace API.Manager.Options
{
    public class ApiManagerOptions
    {
        public virtual string Schema { get; set; }
        public virtual bool? CreateTableIfNeccassary { get; set; }
        public virtual string HeaderKey { get; set; }
        public virtual string[] Channels { get; set; }
        public virtual bool IsServiceable { get; set; }
        public virtual string NotAcceptableMessage { get; set; }
    }
}
