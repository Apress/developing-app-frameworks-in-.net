using System;
using System.Runtime.Remoting;

//TestConcreteFactory consists sample class factory and business class used by
//the TestConsole application.
namespace TestConcreteFactory
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public abstract class ProductFactory : MarshalByRefObject
	{
		public abstract Product GetCheapProduct();
		public abstract Product GetExpensiveProduct();

	}

	public class ConcreteProductFactory : ProductFactory
	{
		public override Product GetCheapProduct()
		{
			return new CheapProduct();
		}
		public override Product GetExpensiveProduct()
		{
			return new ExpensiveProduct();
		}
	}

	public class ConcreteNewProductFactory : ProductFactory
	{
		public override Product GetCheapProduct()
		{
			return new NewCheapProduct();
		}
		public override Product GetExpensiveProduct()
		{
			return new NewExpensiveProduct();
		}
	}

	public class ConcreteRemoteProductFactory: ProductFactory
	{
		public override Product GetCheapProduct()
		{
			return new RemoteCheapProduct();
		}
		public override Product GetExpensiveProduct()
		{
			return new RemoteExpensiveProduct();
		}
	}
	public abstract class Product : MarshalByRefObject
	{
		public abstract string Name {get;}
		public abstract int GetPrice();
		public abstract string GetColor();
	}

	public class CheapProduct : Product
	{
		private const int cost = 10;
		private const string color = "red";
		private const string name ="Cheap Product";
		
		public override int GetPrice()
		{
			return cost *2;	
		}
		public override string GetColor()
		{
			return color;
		}
		public override string Name
		{
			get
			{
				return name;
			}
		}
	}
	public class NewCheapProduct : Product
	{
		private const int cost = 10;
		private const string color = "black";
		private const string name ="New Cheap Product";
		public override int GetPrice()
		{
			return cost *2;	
		}
		public override string GetColor()
		{
			return color;
		}
		public override string Name
		{
			get
			{
				return name;
			}
		}
		
	}
	public class ExpensiveProduct : Product
	{
		private const int cost = 10;
		private const string color = "red";
		private const string name ="Expensive Product";

		public override int GetPrice()
		{
			return cost * 10;	
		}
		public override string GetColor()
		{
			return color;
		}
		public override string Name
		{
			get
			{
				return name;
			}
		}
	}
	public class NewExpensiveProduct : Product
	{
		private const int cost = 10;
		private const string color = "black";
		private const string name ="New Expensive Product";
		public override int GetPrice()
		{
			return cost * 10;	
		}
		public override string GetColor()
		{
			return color;
		}
		public override string Name
		{
			get
			{
				return name;
			}
		}
		
	}
	public class RemoteCheapProduct : Product
	{
		private const int cost = 10;
		private const string color = "yellow";
		private const string name ="Remote Cheap Product";

		public override int GetPrice()
		{
			return cost * 100;	
		}
		public override string GetColor()
		{
			return color;
		}
		public override string Name
		{
			get
			{
				return name;
			}
		}
	}
	public class RemoteExpensiveProduct : Product
	{
		private const int cost = 10;
		private const string color = "gray";
		private const string name ="Remote Expensive Product";
		public override int GetPrice()
		{
			return cost * 1000;	
		}
		public override string GetColor()
		{
			return color;
		}
		public override string Name
		{
			get
			{
				return name;
			}
		}
		
	}
}
