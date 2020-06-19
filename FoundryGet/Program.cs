using FoundryGet.Commands;
using Microsoft.Extensions.CommandLineUtils;
using System;

namespace FoundryGet
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "foundryget"
            };

            app.HelpOption("-?|-h|--help");

            app.OnExecute(() =>
            {
                Console.WriteLine("Need help? Run `foundryget -?`");
                return 0;
            });

            app.Command("install", InstallCommand.Command);
            app.Command("update", UpdateCommand.Command);

            app.Execute(args);
        }
    }
}
