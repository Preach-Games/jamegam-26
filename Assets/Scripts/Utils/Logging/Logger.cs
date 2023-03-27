namespace DungeonDraws.Scripts.Utils.Logging
{
    public abstract class Logger: IXLogger
    {
        public Loglevel _logLevel = Loglevel.Info;
        public Loglevel _logLimit = Loglevel.Info;

        private void log(string v)
        {
            if (_logLevel <= _logLimit)
            {
                output(v);
            }
        }
        public void error(string v) {
            setLogLevel(Loglevel.Error);
            log(v);
        }

        public void info(string v) {
            setLogLevel(Loglevel.Info);
            log(v);
        }

        public void warning(string v) {
            setLogLevel(Loglevel.Warn);
            log(v);
        }

        public virtual void output(string v) {}

        public void setLogLevel(Loglevel logLevel)
        {
            _logLevel = logLevel;
        }
        
        public void setLogLimit(Loglevel logLimit)
        {
            _logLimit = logLimit;
        }
    }
}