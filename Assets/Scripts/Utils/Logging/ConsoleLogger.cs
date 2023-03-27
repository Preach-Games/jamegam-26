using System;

namespace DungeonDraws.Scripts.Utils.Logging
{
    public class ConsoleLogger : Logger {
        public override void output(string v)
        {
            switch (_logLevel)
            {
                case Loglevel.Error:
                    Console.WriteLine("[ERRO]: " + v);
                    break;
                case Loglevel.Warn:
                    Console.WriteLine("[WARN]: " + v);
                    break;
                case Loglevel.Info:
                    Console.WriteLine("[INFO]: " + v);
                    break;
            }
        }
    }
}