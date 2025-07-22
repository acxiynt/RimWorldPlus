using System;

namespace Verse.MultiThreadUtility;

public static class GlobalTaskPool
{
	public static readonly TaskPool Instance = new TaskPool(Environment.ProcessorCount);
}
