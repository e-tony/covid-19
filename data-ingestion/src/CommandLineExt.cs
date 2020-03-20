using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Covid
{
    public static class CommandLineExt 
    {
        private static readonly string[] _argumentParameterNames =
        {
            "arguments",
            "argument",
            "args"
        };
        public static void AddFromMethod(this Command rootCmd, MethodInfo method, string description, object target = null)
        {
            var command = new Command(method.Name.ToKebabCase(), description: description);
            command.ConfigureFromMethod(method, target);
            rootCmd.Add(command);
        }

        public static void ConfigureFromMethod(this Command command, MethodInfo method, object target = null)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            command.Name = method.Name.ToKebabCase();

            foreach (var option in method.BuildOptions())
            {
                command.AddOption(option);
            }

            if (method.GetParameters().FirstOrDefault(p => _argumentParameterNames.Contains(p.Name)) is ParameterInfo argsParam)
            {
                var argument = new Argument
                {
                    ArgumentType = argsParam.ParameterType,
                    Name = argsParam.Name
                };

                if (argsParam.HasDefaultValue)
                {
                    if (argsParam.DefaultValue != null)
                    {
                        argument.SetDefaultValue(argsParam.DefaultValue);
                    }
                    else
                    {
                        argument.SetDefaultValueFactory(() => null);
                    }
                }

                command.AddArgument(argument);
            }

            command.Handler = CommandHandler.Create(method, target);
        }

        public static IEnumerable<Option> BuildOptions(this MethodInfo type)
        {
            var descriptor = HandlerDescriptor.FromMethodInfo(type);

            var omittedTypes = new[]
                               {
                                   typeof(IConsole),
                                   typeof(InvocationContext),
                                   typeof(BindingContext),
                                   typeof(ParseResult),
                                   typeof(CancellationToken),
                               };

            foreach (var option in descriptor.ParameterDescriptors
                                             .Where(d => !omittedTypes.Contains(d.Type))
                                             .Where(d => !_argumentParameterNames.Contains(d.ValueName))
                                             .Select(p => p.BuildOption()))
            {
                yield return option;
            }
        }

        public static Option BuildOption(this ParameterDescriptor parameter)
        {
            var argument = new Argument
            {
                ArgumentType = parameter.Type
            };

            if (parameter.HasDefaultValue)
            {
                argument.SetDefaultValueFactory(parameter.GetDefaultValue);
            }

            var option = new Option(parameter.BuildAlias(), parameter.ValueName)
            {
                Argument = argument
            };

            return option;
        }

        public static string BuildAlias(this IValueDescriptor descriptor)
        {
            if (descriptor == null)
            {
                throw new ArgumentNullException(nameof(descriptor));
            }

            return BuildAlias(descriptor.ValueName);
        }

        internal static string BuildAlias(string parameterName)
        {
            if (string.IsNullOrWhiteSpace(parameterName))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(parameterName));
            }

            return parameterName.Length > 1 ? $"--{parameterName.ToKebabCase()}" : $"-{parameterName.ToLowerInvariant()}";
        }
    }
}
