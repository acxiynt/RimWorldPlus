using System;

namespace Verse.MultiThreadUtility;

public static class GlobalTaskPool__0
{
	public static readonly TaskPool Instance = new TaskPool(Environment.ProcessorCount);
}
