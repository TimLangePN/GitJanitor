﻿using System.CommandLine;

namespace GitJanitor.CLI;

internal class Program
{
    internal static async Task Main(string[] args)
    {
        var services = new ServiceConfiguration().ConfigureServices();
        var command = new CommandBuilder().BuildCommandLineApplication(services);

        // Parse the incoming args and invoke the handler
        await command.InvokeAsync(args);
    }
}