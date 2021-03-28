﻿using System.Windows.Input;

namespace GuiForSimpleFTP
{
    public static class Commands
	{
		public static RoutedUICommand Download { get; } = new RoutedUICommand("Download", "Download", typeof(Commands));

		public static RoutedUICommand DownloadAll { get; } = new RoutedUICommand("DownloadAll", "DownloadAll", typeof(Commands));

		public static RoutedUICommand GoBack { get; } = new RoutedUICommand("GoBack", "GoBack", typeof(Commands));

		public static RoutedUICommand Connect { get; } = new RoutedUICommand("Connect", "Connect", typeof(Commands));

		public static RoutedUICommand StepInto { get; } = new RoutedUICommand("StepInto", "StepInto", typeof(Commands));
	}
}
