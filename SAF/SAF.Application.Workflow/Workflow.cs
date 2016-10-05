using System;
using SAF.Application.DocumentLayer;

namespace SAF.Application.Workflow
{
	/// <summary>
	/// interface that represents the members of
	///  Component type class
	/// </summary>
	public interface IComponent
	{
		IDocument Request{get;set;}
		IDocument Response{get;set;}
		//provides the reference to the next component in the work flow chain.
		IComponent NextComponent{get;set;}
		//Accept method in the Visitor design pattern.
		void Accept(IVisitor v);
	}

	/// <summary>
	/// A marker interface that represents the Visitor class
	/// </summary>
	public interface IVisitor
	{
	}
}
