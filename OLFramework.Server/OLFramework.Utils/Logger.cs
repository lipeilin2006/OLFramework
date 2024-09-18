namespace OLFramework.Utils
{
	public static class Logger
	{
		public static string FileName { get; set; } = ".\\log.txt";
		public static LogLevel Level { get; set; }= LogLevel.Error;
		public static void Log(string message, LogLevel level)
		{
			if ((int)level >= (int)Level)
			{
				switch (level)
				{
					case LogLevel.Debug:
						Console.ForegroundColor = ConsoleColor.Green; break;
					case LogLevel.Info:
						Console.ForegroundColor = ConsoleColor.Blue; break;
					case LogLevel.Warn:
						Console.ForegroundColor = ConsoleColor.Yellow; break;
					case LogLevel.Error:
						Console.ForegroundColor = ConsoleColor.Red; break;
				}

				string msg = $"[UTC {DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}] : {message}";
				Console.WriteLine(msg);
				Console.ForegroundColor = ConsoleColor.White;
				//File.AppendAllText(FileName, msg + Environment.NewLine);
			}
		}
	}
	public enum LogLevel
	{
		Debug = 0,
		Info = 1,
		Warn = 2,
		Error = 3
	}
}
