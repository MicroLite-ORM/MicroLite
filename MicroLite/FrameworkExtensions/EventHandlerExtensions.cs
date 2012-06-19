// -----------------------------------------------------------------------
// <copyright file="EventHandlerExtensions.cs" company="MicroLite">
// Copyright 2012 Trevor Pilley
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// </copyright>
// -----------------------------------------------------------------------
namespace MicroLite.FrameworkExtensions
{
    using System;

    /// <summary>
    /// Extension methods for the EventHandler delegate.
    /// </summary>
    internal static class EventHandlerExtensions
    {
        internal static void Raise(this EventHandler eventHandler, object sender)
        {
            Raise(eventHandler, sender, EventArgs.Empty);
        }

        internal static void Raise(this EventHandler eventHandler, object sender, EventArgs eventArgs)
        {
            var handler = eventHandler;

            if (handler != null)
            {
                handler(sender, eventArgs);
            }
        }
    }
}