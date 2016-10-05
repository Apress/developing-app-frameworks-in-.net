using System;

namespace SAF.Library.Security
{
	/// <summary>
	/// Interface for the authorization provider
	/// </summary>
	public interface IAuthorizationProvider
	{
		void Authorize(string[] allowedRoles, string[] deniedRoles);
	}
}
