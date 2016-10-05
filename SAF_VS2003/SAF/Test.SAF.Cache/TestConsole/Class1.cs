using System;
using SAF.Cache;
using SAF.Configuration;
using System.Configuration; 

namespace TestConsole
{
	/// <summary>
	/// The demo shows how to use the SAF.Cache service
	/// to add, remove and retrieve objects from the cache
	/// </summary>
	class Class1
	{
	
		[STAThread]
		static void Main(string[] args)
		{
			//get the cache object 
			Cache cache = SAF.Cache.Cache.GetSAFCacheService();
			//add some objects into the cache service
			cache.AddObject("/WebApplication/Users/Xin", "customer xin");
			cache.AddObject("/WebApplication/Users/Mike", "customer mike");
			cache.AddObject("/WebApplication/Users/Steve", "customer steve");
			cache.AddObject("/WebApplication/GlobalData", "1/1/2003");

			//retrieve the objects as a group
			object[] objects = cache.RetrieveObjectList("/WebApplication/Users");
			foreach (object o in objects)
			{
				Console.WriteLine("Customer in cache: {0}", o.ToString());
			}

			//retrieve the object as individual
			string time =(string) cache.RetrieveObject("/WebApplication/GlobalData");
			string name = (string) cache.RetrieveObject("/WebApplication/Users/Xin");

			//remove the object
			cache.RemoveObject("/WebApplication/GlobalData");

			//remove all the object under /Users
			cache.RemoveObject("/WebApplication/Users");

			Console.WriteLine("Press Enter to finish");
			Console.ReadLine();
		}
	}
}
