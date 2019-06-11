using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using System.Linq;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Paku.Models
{
    /// <summary>
    /// # Pipeline
    /// 
    /// The pipeline ties together the entire file selection, filtering, and "paku" process.
    /// It also contains methods useful for dynamically instantiating types (specifically strategy implementations).
    /// </summary>
    public class Pipeline
    {
        /// <summary>
        /// ## SelectionStrategy
        /// 
        /// Logic used to select files for paku-ing.
        /// </summary>
        public ISelectionStrategy SelectionStrategy { get; set; }

        /// <summary>
        /// ## FilterStrategy
        /// 
        /// Logic to used to further filter and selected items.
        /// </summary>
        public IFilterStrategy FilterStrategy { get; set; }

        /// <summary>
        /// ## PakuStrategy
        /// 
        /// How the selected and filtered files are actually handled (deleted, zipped, etc.).
        /// </summary>
        public IPakuStrategy PakuStrategy { get; set; }

        /// <summary>
        /// ## Logger
        /// 
        /// Used to log activity.
        /// </summary>
        public Logger Logger { get; private set; }

        /// <summary>
        /// ## Execute
        /// 
        /// Executes the pipeline on a directory with the specified parameters.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="selectParams"></param>
        /// <param name="filterParams"></param>
        /// <returns></returns>
        public PakuResult Execute(string directory, string selectParams, string filterParams, bool logToFile = false)
        {
            PakuResult result = new PakuResult();

            // check if directory exists first
            DirectoryInfo di = new DirectoryInfo(directory);

            if (!di.Exists)
            {
                // if not, throw an argument exception
                // we do not want to create a logger in this case, as it will create the directory structure for the log file
                result.Error = new ArgumentException($"Directory does not exist: {directory}");
            }
            else
            {
                // setup the logger
                BootstrapLogger(logToFile, directory);

                try
                {
                    Logger.Info($"Cleaning directory: {directory}");

                    IList<VirtualFileInfo> files = SelectionStrategy.Select(di, selectParams);
                    Logger.Info($"Files selected: {files.Count}");

                    files = FilterStrategy.Filter(files, filterParams);
                    Logger.Info($"Files filtered: {files.Count}");

                    result = PakuStrategy.Eat(di, files, null);
                    Logger.Info($"Files removed: {result.RemovedFiles.Count}");

                    foreach (VirtualFileInfo fi in result.RemovedFiles)
                    {
                        Logger.Debug($"Removed: {fi.Name}");
                    }
                }
                catch (Exception ex)
                {
                    result.Error = ex;
                }

                if (!result.Success)
                {
                    Logger.Error(result.Error);
                }
            }

            // write any errors to the console
            if (!result.Success)
            {
                Console.WriteLine(result.Error.Message);
            }

            return result;
        }

        /// <summary>
        /// ## GetStrategyImplementations
        /// 
        /// Returns a list of implementions for the specified strategy interface.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<TypeInfo> GetStrategyImplementations<T>()
        {
            var assembly = typeof(Pipeline).Assembly;
            List<TypeInfo> types = assembly.DefinedTypes.Where(type => type.ImplementedInterfaces.Any(inter => inter == typeof(T))).ToList();

            return types;
        }

        /// <summary>
        /// ## GetStrategyAliasMap
        /// 
        /// Returns a mapping of all alias => class names for classes that implement the strategy.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Dictionary<string, string> GetStrategyAliasMap<T>()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            // iterate over all strategy implementations
            // and pull alias from class attribute
            List<TypeInfo> types = GetStrategyImplementations<T>();
            foreach (TypeInfo ti in types)
            {
                // then pull in from the attribute
                CommandAlias alias = ti.GetCustomAttribute<CommandAlias>();

                if (alias != null)
                {
                    foreach (string s in alias.Aliases)
                    {
                        if (map.ContainsKey(s))
                        {
                            throw new Exception($"Duplicate alias defined for the type {ti.Name}: {s}");
                        }

                        map[s] = ti.Name;
                    }
                }
                else
                {
                    // add the class name as the default "alias"
                    map[ti.Name] = ti.Name;
                }
            }

            return map;
        }

        /// <summary>
        /// ## GetStrategyFromString
        /// 
        /// Returns an instance of the specified strategy implementation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        private T GetStrategyFromString<T>(string str)
        {
            // attempt to load selection strategy by name
            T selectionStrategy = default(T);
            Type clazz = Type.GetType($"Paku.Models.{str}", false, true);

            // make sure that the class implements the interface
            if (typeof(T).IsAssignableFrom(clazz))
            {
                selectionStrategy = (T)Activator.CreateInstance(clazz);
            }

            if (selectionStrategy == null)
            {
                throw new ArgumentException($"{str} is not a valid implementation of {typeof(T)}.");
            }

            return selectionStrategy;
        }

        /// <summary>
        /// ## BootstrapLogger
        /// 
        /// Sets up the logger to log pipeline execution.
        /// </summary>
        /// <param name="logToFile"></param>
        /// <param name="directory"></param>
        private void BootstrapLogger(bool logToFile, string directory)
        {
            LoggingConfiguration config = new LoggingConfiguration();

            if (logToFile)
            {
                FileTarget fileTarget = new FileTarget("file")
                {
                    FileName = Path.Combine(directory, "logs", "paku.log"),
                    ArchiveEvery = FileArchivePeriod.Day,
                    ArchiveNumbering = ArchiveNumberingMode.Date,
                    ArchiveDateFormat = "yyyyMMdd"
                };
                config.AddRuleForAllLevels(fileTarget);
            }
            else
            {
                config.AddTarget(new NullTarget("null"));
                config.AddRuleForAllLevels("null");
            }

            LogManager.Configuration = config;
            this.Logger = LogManager.GetLogger("Paku");
        }

        public Pipeline(string selection, string filter, string paku, bool loggingEnabled = false)
        {
            this.SelectionStrategy = GetStrategyFromString<ISelectionStrategy>(selection);
            this.FilterStrategy = GetStrategyFromString<IFilterStrategy>(filter);
            this.PakuStrategy = GetStrategyFromString<IPakuStrategy>(paku);
        }

        public Pipeline(ISelectionStrategy selection, IFilterStrategy filter, IPakuStrategy paku, bool loggingEnabled = false)
        {
            this.SelectionStrategy = selection;
            this.FilterStrategy = filter;
            this.PakuStrategy = paku;
        }
    }
}
