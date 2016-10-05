using System;
using System.EnterpriseServices;
using System.Runtime.InteropServices;
using System.Reflection;


[assembly: ApplicationActivation(ActivationOption.Library)]
[assembly: ApplicationID("77769CB1-A707-11D0-989D-00C04FD444E4")]
[assembly: ApplicationName("SAF.Transaction")]
namespace SAF.Transaction
{
	/// <summary>
	/// Interface each transaction controller must implement.
	/// </summary>
	public interface ITransactionController
	{
		object ExecuteMethod(object o, string method, params object[] p);
		void Rollback();
		void Commit();
	}

	/// <summary>
	/// TransactionControllerBase is the base class for serviced components in SAF.Transaction.
	/// It provides a way to execute business method as part of the transaction context and abilities
	/// to commit and rollback transactions.
	/// </summary>
	public abstract class TransactionControllerBase : System.EnterpriseServices.ServicedComponent,ITransactionController
	{
	
		public virtual object ExecuteMethod(object o, string method, params object[] p )
		{
			try
			{
				//use the reflection to invoke the business method within the current transaction scope.
				return o.GetType().InvokeMember(method,BindingFlags.InvokeMethod,null,o,p);
			}
			catch (Exception ex)
			{
				//if exception occurs, mark the current transaction as such so that we can
				//rollback the transaction later.
				if (ContextUtil.IsInTransaction)
				{
					ContextUtil.DisableCommit();
				}
				//rethow the exception to notify the caller.
				throw ex;
			}
		}	
	
		/// <summary>
		/// Rollback method set abort the current transaction.
		/// </summary>
		public virtual void Rollback()
		{
			//if it is part of a transaction, abort it.
			if (ContextUtil.IsInTransaction)
			{
				ContextUtil.SetAbort();
			}
		}

		/// <summary>
		/// Commit method commit the current transaction if all the transaction vode
		/// are good.
		/// </summary>
		public virtual void Commit()
		{
			//check if it is in a transaction
			if (ContextUtil.IsInTransaction)
			{
				//check if the transaction vote is good. if so, set complete on the transaction
				//otherwise, abort the transaction.
				if (ContextUtil.MyTransactionVote == TransactionVote.Commit)
				{
					ContextUtil.SetComplete();
				}
				else
				{
					ContextUtil.SetAbort();
				}
			}
		}	
	}


	/// <summary>
	/// Provide the Manager class that 
	/// clients interact when creating different type of transaction compnent.
	/// </summary>
	public class TransactionManager
	{
		/// <summary>
		/// Return a new serviced component that supports transaction
		/// </summary>
		/// <returns>SupportTransaction object</returns>
		public static ITransactionController GetSupportTransactionController()
		{
			return new SupportTransaction();
		}

		/// <summary>
		/// Return a new service component that require transaction
		/// </summary>
		/// <returns>RequireTransaction object</returns>
		public static ITransactionController GetRequireTransactionController()
		{
			return new RequireTransaction();
		}

		/// <summary>
		/// Return a new service component that require new transaction
		/// </summary>
		/// <returns>RequireNewTransaction object</returns>
		public static ITransactionController GetRequireNewTransactionController()
		{
			return new RequireNewTransaction();
		}

		/// <summary>
		/// Return a new service component with no transaction
		/// </summary>
		/// <returns>NoTransaction object</returns>
		public static ITransactionController GetNoTransactionController()
		{
			return new NoTransaction();
		}

		/// <summary>
		/// Helper method that will rollback array of ITransactionController objects
		/// </summary>
		/// <param name="txControllers">ITransactionController array</param>
		public static void Rollback(params ITransactionController[] txControllers)
		{
			foreach (ITransactionController txController in txControllers)
			{
				if (txController != null)
				{
					txController.Rollback();
				}
				
			}
		}

		/// <summary>
		/// Helper method that will commit array of ITransactionController objects
		/// </summary>
		/// <param name="txControllers">ITransactionController array</param>
		public static void Commit(params ITransactionController[] txControllers)
		{
			foreach (ITransactionController txController in txControllers)
			{
				if (txController != null)
				{
					txController.Commit();
				}
				
			}
		}


		/// <summary>
		/// Release the resource held by the serviced components
		/// </summary>
		/// <param name="txControllers">ITransactionController array</param>
		public static void DisposeAll(params object[] txControllers)
		{
			foreach (object txController in txControllers)
			{
				if (txController != null)
				{
					try
					{
						//check is txController is a service compnen.
						if (txController is System.EnterpriseServices.ServicedComponent)
						{
							//destory the serviced component
							((ServicedComponent)txController).Dispose();
						}
					}
					catch (Exception ex){}
				}
			}
		}
	}
	
	/// <summary>
	/// Transaction controller that supports transaction.
	/// </summary>
	[ProgId("SupportTransaction")]
	[Guid("E42F5FFF-823B-4F20-AE80-B13A3C991112")]
	[Transaction(System.EnterpriseServices.TransactionOption.Supported)]
	public class SupportTransaction : TransactionControllerBase
	{
		public SupportTransaction()
		{
		}
	}

	
	/// <summary>
	/// Transaction controller that requires transaction
	/// </summary>
	[ProgId("RequireTransaction")]
	[Guid("E42F5FFF-823B-4F20-AE80-B13A3C991113")]
	[Transaction(System.EnterpriseServices.TransactionOption.Required)]
	public class RequireTransaction : TransactionControllerBase 
	{
		public RequireTransaction()
		{
		}

		
	}

	/// <summary>
	/// Transaction controller that requires new transaction
	/// </summary>
	[ProgId("RequireNewTransaction")]
	[Guid("E42F5FFF-823B-4F20-AE80-B13A3C991114")]
	[Transaction(System.EnterpriseServices.TransactionOption.RequiresNew,Isolation=TransactionIsolationLevel.ReadCommitted)]
	public class RequireNewTransaction : TransactionControllerBase 
	{
		public RequireNewTransaction()
		{
		}		
	}

	/// <summary>
	/// Transaction controller that doesn't support transaction
	/// </summary>
	[ProgId("NoTransaction")]
	[Guid("E42F5FFF-823B-4F20-AE80-B13A3C991115")]
	[Transaction(System.EnterpriseServices.TransactionOption.NotSupported)]
	public class NoTransaction : TransactionControllerBase
	{
		public NoTransaction()
		{
		}	
	}
}
