using System;
using SAF.ClassFactory;
using SAF.Configuration;
using System.Configuration;
using TestConcreteFactory;
using System.Runtime.Remoting;

namespace TestConsole
{
	/// <summary>
	/// The demo show how to use the SAF.ClassFactory service
	/// </summary>
	class Class1
	{
		[STAThread]
		static void Main(string[] args)
		{
			//set up the sever side remoting for remote class factory.
			RemotingConfiguration.Configure("TestConsole.exe.config");
			//create the instances for class factory
			ProductFactory pfA = (ProductFactory)ClassFactory.GetFactory("ProductFactory-A");
			ProductFactory pfB = (ProductFactory)ClassFactory.GetFactory("ProductFactory-B");
			ProductFactory pfC = (ProductFactory)ClassFactory.GetFactory("Remote-ProductFactory-C");
			
			//creating different product objects on each class factory instance
			Product p1 =pfA.GetCheapProduct();
			Product p3 =pfA.GetExpensiveProduct();
			Product p2 =pfB.GetCheapProduct();
			Product p4 =pfB.GetExpensiveProduct();
			//these two are the remoting calls because of remoting class factory.
			Product p5 =pfC.GetCheapProduct();
			Product p6 =pfC.GetExpensiveProduct();

			//calling the methods and properties on each prodcut object.
			PrintOutProductDescription(p1);
			PrintOutProductDescription(p2);
			PrintOutProductDescription(p3);
			PrintOutProductDescription(p4);
			//these two call are remoting calls.
			PrintOutProductDescription(p5);
			PrintOutProductDescription(p6);

			Console.WriteLine("Press enter to finish");
			Console.ReadLine();

		}

		private static void PrintOutProductDescription(Product p)
		{
			Console.WriteLine ("Product Name: {0}",p.Name);
			Console.WriteLine ("Product Color: {0}",p.GetColor());
			Console.WriteLine ("Product Price: {0} \n\n",p.GetPrice());

	
		}
	}
}
