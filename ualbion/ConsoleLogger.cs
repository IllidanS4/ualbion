﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UAlbion.Core;
using UAlbion.Game;

namespace UAlbion
{
    [Event("help", "Display help on the available console commands.", new[] {"?", "usage"})]
    public class HelpEvent : GameEvent
    {
        public HelpEvent(string commandName)
        {
            CommandName = commandName;
        }

        [EventPart("command", true)]
        public string CommandName { get; }
    }

    class ConsoleLogger : IComponent
    {
        EventExchange _exchange;
        bool _done;

        public void Attach(EventExchange exchange)
        {
            _exchange = exchange;
            exchange.Subscribe<IEvent>(this);
            Task.Run(ConsoleReaderThread);
        }

        void PrintHelpSummary(IEnumerable<Event.EventMetadata> events)
        {
            foreach (var e in events)
            {
                var paramList = e.Parts.Length == 0
                    ? ""
                    : " " + string.Join(" ",
                        e.Parts.Select(x => x.IsOptional ? $"[{x.Name}]" : x.Name));

                Console.WriteLine("    {0}{1}: {2}", e.Name, paramList, e.HelpText);
            }
        }

        void PrintDetailedHelp(Event.EventMetadata metadata)
        {
            var paramList = metadata.Parts.Length == 0
                ? ""
                : " " + string.Join(" ",
                    metadata.Parts.Select(x => x.IsOptional ? $"[{x.Name}]" : x.Name));

            Console.WriteLine("    {0}{1}: {2}", metadata.Name, paramList, metadata.HelpText);
            foreach (var param in metadata.Parts)
                Console.WriteLine("        {0} ({1}): {2}", param.Name, param.PropertyType, param.HelpText);
        }

        public void Receive(IEvent @event, object sender)
        {
            switch(@event)
            {
                case EngineUpdateEvent _:
                case RenderEvent _:
                case UpdateEvent _:
                    break;

                case QuitEvent _:
                    _done = true;
                    break;

                case HelpEvent helpEvent:
                    if (string.IsNullOrEmpty(helpEvent.CommandName))
                    {
                        Console.WriteLine();
                        Console.WriteLine("Command Usage Help:");
                        Console.WriteLine("-------------------------------------");
                        PrintHelpSummary(Event.GetEventMetadata());
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine();
                        var metadata = Event.GetEventMetadata()
                            .FirstOrDefault(x => x.Name.Equals(helpEvent.CommandName, StringComparison.InvariantCultureIgnoreCase)
                            || x.Aliases != null && 
                               x.Aliases.Any(y => y.Equals(helpEvent.CommandName, StringComparison.InvariantCultureIgnoreCase)));

                        if (metadata != null)
                        {
                            PrintDetailedHelp(metadata);
                        }
                        else
                        {
                            var regex = new Regex(helpEvent.CommandName);
                            var matchingEvents = Event.GetEventMetadata().Where(x => regex.IsMatch(x.Name)).ToList();

                            if(matchingEvents.Any())
                                PrintHelpSummary(matchingEvents);
                            else
                                Console.WriteLine("The command \"{0}\" is not recognised.", helpEvent.CommandName);
                        }
                    }
                    break;

                default:
                    if (sender == this)
                        return;
                    Console.WriteLine(@event.ToString());
                    break;
            }
        }

        public void Detach()
        {
            _exchange = null;
        }

        public void ConsoleReaderThread()
        {
            do
            {
                var command = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(command))
                    continue;

                try
                {
                    var @event = Event.Parse(command);
                    if (@event != null)
                    {
                        try { _exchange.Raise(@event, this); }
                        catch (Exception e) { Console.WriteLine("Error: {0}", e.Message); }
                    }
                    else
                        Console.WriteLine("Unknown event \"{0}\"", command);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Parse error: {0}", e);
                }

            } while (!_done);
        }
    }
}