using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Mono.Options;
using System.ComponentModel;

namespace Paku.Models
{
    /// <summary>
    /// # PakuArguments
    /// 
    /// Handles parsing arguments for CLI.
    /// </summary>
    public class PakuArguments
    {
        /// <summary>
        /// ## Directory
        /// 
        /// The directory to clean.
        /// </summary>
        public string Directory { get; set; }

        /// <summary>
        /// ## SelectionStrategy
        /// 
        /// The selection strategy and arguments provided through the --select option.
        /// </summary>
        public Tuple<string, string> SelectionStrategy { get; set; }

        /// <summary>
        /// ## FilterStrategy
        /// 
        /// The filter strategy and arguments provided through the --filter option.
        /// </summary>
        public Tuple<string, string> FilterStrategy { get; set; }

        /// <summary>
        /// ## PakuStrategy
        /// 
        /// The paku strategy and arguments provided through the --paku option.
        /// </summary>
        public Tuple<string, string> PakuStrategy { get; set; }

        public OptionSet BuildOptionSet()
        {
            Dictionary<string, string> selectionAliases = Pipeline.GetStrategyAliasMap<ISelectionStrategy>();
            Dictionary<string, string> filterAliases = Pipeline.GetStrategyAliasMap<IFilterStrategy>();
            Dictionary<string, string> pakuAliases = Pipeline.GetStrategyAliasMap<IPakuStrategy>();

            var options = new OptionSet();

            // add selection strategies as CLI options
            AddOption(options, Pipeline.GetStrategyImplementations<ISelectionStrategy>(), "select", (string strategy, string parms) =>
            {
                string alias;
                if (selectionAliases.TryGetValue(strategy, out alias))
                {
                    SelectionStrategy = new Tuple<string, string>(alias, parms);
                }
                else
                {
                    throw new ArgumentException($"{strategy} is not a valid command.");
                }
            });

            AddOption(options, Pipeline.GetStrategyImplementations<IFilterStrategy>(), "filter", (string strategy, string parms) =>
            {
                string alias;
                if (filterAliases.TryGetValue(strategy, out alias))
                {
                    FilterStrategy = new Tuple<string, string>(alias, parms);
                }
                else
                {
                    throw new ArgumentException($"{strategy} is not a valid command.");
                }
            });

            AddOption(options, Pipeline.GetStrategyImplementations<IPakuStrategy>(), "paku", (string strategy, string parms) =>
            {
                string alias;
                if (pakuAliases.TryGetValue(strategy, out alias))
                {
                    PakuStrategy = new Tuple<string, string>(alias, parms);
                }
                else
                {
                    throw new ArgumentException($"{strategy} is not a valid command.");
                }
            });

            // allow the user to specify a directory
            options.Add("d|dir=", "Specify the directory to clean; if not specified, defaults to the current.", (string opt) =>
            {
                Directory = opt;
            });

            // add a help option
            options.Add("h|help|?", "Show this help menu.", (string opt) =>
            {
                options.WriteOptionDescriptions(Console.Out);
            });

            return options;
        }

        public bool Parse(string[] args)
        {
            OptionSet options = BuildOptionSet();
            bool success = true;

            try
            {
                options.Parse(args);

                if (SelectionStrategy == null || FilterStrategy == null || PakuStrategy == null)
                {
                    throw new ArgumentException("The --select, --filter, and --paku options are required.");
                }
            }
            catch (Exception ex)
            {
                // invalid option command provided or required options are missing
                // show error message and render help
                Console.WriteLine(ex.Message);
                options.WriteOptionDescriptions(Console.Out);

                success = false;
            }


            return success;
        }

        /// <summary>
        /// ## AddOption
        /// 
        /// Adds a new option to the option set.
        /// </summary>
        /// <param name="options">OptionSet to add to.</param>
        /// <param name="strategies">List of strategy types to use for this option.</param>
        /// <param name="option">The option name.</param>
        /// <param name="action">The delegate to call for the option.</param>
        private void AddOption(OptionSet options, List<TypeInfo> strategies, string option, OptionAction<string, string> action)
        {
            /*
             * The terminology here is a bit confusing, so here is quick explanation:
             * - Option is the overall argument provided and the bit of text right after the hypen(s): --foo or -f
             * - Command is the part after the colon following the option: --foo:bar
             *   This ties back directly to a strategy.
             * 
             * So for the CLI argument `--select:regex=^foo\.txt$`, we get:
             * - option: select
             * - command: regex (refers to the RegexSelectionStrategy)
             * - command parameters: ^foo\.txt$
             * 
             * We are also using the first character of the command as a short alias,
             * so we can use `-s` instead of `--select` for example.
            */

            // Let's use class attributes for each stategy to set the option command name
            // and build the option command description text.
            StringBuilder buffer = new StringBuilder($"The {option} strategy to use and any parameters to provide.");
            foreach (TypeInfo ti in strategies)
            {
                // default command alias for strategy is the class name
                string alias = ti.Name;
                string description = "(no description)";

                CommandAlias commandAlias = ti.GetCustomAttribute<CommandAlias>();
                if (commandAlias != null && commandAlias.Aliases.Count > 0)
                {
                    alias = String.Join(", ", commandAlias.Aliases);
                }

                DescriptionAttribute descriptionAttr = ti.GetCustomAttribute<DescriptionAttribute>();
                if (descriptionAttr != null)
                {
                    description = descriptionAttr.Description;
                }

                // final constuct should look like: - foo, bar: My foobar description.
                buffer.Append($"\n  - {alias}: {description}");
            }

            // add the option:
            // f|foo=:
            // The foo strategy to use...
            // delegate to call
            options.Add($"{option[0]}|{option}:", buffer.ToString(), action);
        }
    }
}
