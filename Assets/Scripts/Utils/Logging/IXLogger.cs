namespace DungeonDraws.Scripts.Utils.Logging
{
    public enum Loglevel
    {
        Error,
        Warn,
        Info
    }
    public interface IXLogger
    {
        void warning(string v);
        void error(string v);
        void info(string v);

        void output(string v);

        void setLogLevel(Loglevel _loglevel); 
        void setLogLimit(Loglevel _logLimit);
    }
}