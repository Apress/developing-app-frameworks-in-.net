using System;
using System.Reflection;
using SAF.Transaction;
using System.Data;
using System.Data.SqlClient;

namespace TestConsole
{
	/// <summary>
	/// Demo that show SAF.Transaction can be used wrap multiple 
	/// calls into different transactions
	/// </summary>
	class Class1
	{

		[STAThread]
		static void Main(string[] args)
		{
			DataAccess data = new DataAccess();
			//clean up the table 
			data.ClearupTable();
			
			//--------------Sample 1--------------------------------//
			ITransactionController transaction_A= TransactionManager.GetRequireTransactionController();
			try
			{
				transaction_A.ExecuteMethod(data,"AddNewRecord",new object[2]{"xin","111111111"});
				transaction_A.ExecuteMethod(data,"AddNewRecord",new object[2]{"mike","222222222"});
				//this call will fail due to duplicate key error.
				transaction_A.ExecuteMethod(data,"AddNewRecord",new object[2]{"john","111111111"});
				TransactionManager.Commit(transaction_A);
			}
			catch (Exception ex)
			{
				TransactionManager.Rollback(transaction_A);
				//additional error handling code....
			}
			finally
			{
				//release the resource used by the serviced component
				TransactionManager.DisposeAll(transaction_A);
			}

			//--------------Sample 1 result --------------------------//
			//None of three record is inserted to the employee table. //
			//--------------------------------------------------------//

			//clean up the table 
			data.ClearupTable();
			
			//--------------Sample 2----------------------------------//
			ITransactionController transaction_C= TransactionManager.GetRequireNewTransactionController();
			ITransactionController transaction_B= TransactionManager.GetRequireTransactionController();
			
			try
			{
				transaction_B.ExecuteMethod(data,"AddNewRecord","xin","111111111");
				transaction_B.ExecuteMethod(data,"AddNewRecord","mike ","222222222");
				try
				{
					//add an important employee. Add him regardless whether other
					//employees are added to table successfully
					transaction_C.ExecuteMethod(data,"AddNewRecord","bill","333333333");
					TransactionManager.Commit(transaction_C);
				}
				catch (Exception ex)
				{
					TransactionManager.Rollback(transaction_C);
				}
				//this call will fail due to duplicate key error.
				transaction_B.ExecuteMethod(data,"AddNewRecord",new object[2]{"john","111111111"});
				TransactionManager.Commit(transaction_B);
			}
			catch (Exception ex)
			{
				TransactionManager.Rollback(transaction_B);
				//additional error handling code....
			}
			finally
			{
				//release the resource used by the serviced component
				TransactionManager.DisposeAll(transaction_B,transaction_C);
			}
			
			//--------------Sample 2 result ---------------------------//
			//----------Only bill is added to the employee table.------//
			//---------------------------------------------------------//


			//clean up the table 
			data.ClearupTable();
		
			//--------------Sample 3-----------------------------------//
			ITransactionController transaction_D= TransactionManager.GetNoTransactionController();
			ITransactionController transaction_E= TransactionManager.GetSupportTransactionController();
			ITransactionController transaction_F= TransactionManager.GetRequireTransactionController();
			try
			{
				data.AddNewRecord("xin","111111111");
				transaction_D.ExecuteMethod(data,"AddNewRecord","mike","222222222");
				transaction_E.ExecuteMethod(data,"AddNewRecord","bill","333333333");
				//this call will fail due to duplicate key error.
				transaction_F.ExecuteMethod(data,"AddNewRecord","john","111111111");
				TransactionManager.Commit(transaction_D,transaction_E,transaction_F);
			}
			catch (Exception ex)
			{
				TransactionManager.Rollback(transaction_D,transaction_E,transaction_F);
				//additional error handling code....
			}
			finally
			{
				//release the resource used by the serviced component
				TransactionManager.DisposeAll(transaction_D,transaction_E,transaction_F);
			}

			//--------------Sample 3 result --------------------------------------------//
			//only xin, mike, bill are added to the employee table. John is not added.--//
			//--------------------------------------------------------------------------//

		}

	}
	/// <summary>
	/// DataAccess provides access to testing db.
	/// </summary>
	public class DataAccess
	{
		private string connection;
		public DataAccess()
		{
			connection ="Initial Catalog=SAFDemo;Data Source=localhost;Integrated Security=SSPI";
		}
		/// <summary>
		/// Insert a record to the table
		/// </summary>
		/// <param name="name"></param>
		/// <param name="ssn"></param>
		public void AddNewRecord(string name,string ssn)
		{
			
			SqlConnection conn = new SqlConnection(connection);
			try
			{
				conn.Open();
				SqlCommand command = new SqlCommand("insert into employees values('" + name + "','" + ssn + "')",conn);
				command.ExecuteNonQuery();
			}
			finally
			{
				conn.Close();
			}

		}

		/// <summary>
		/// Clearn up the table for the next test
		/// </summary>
		public void ClearupTable()
		{
			SqlConnection conn = new SqlConnection(connection);
			try
			{
				conn.Open();
				SqlCommand command = new SqlCommand("delete from employees",conn);
				command.ExecuteNonQuery();
			}
			finally
			{
				conn.Close();
			}
		}
	}
}
