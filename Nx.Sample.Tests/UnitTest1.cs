using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Nx.Sample.Tests
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void TestMethod1()
		{
			var store = new BookStore(); 
			var list = new List<Book>();
			foreach (var book in ((IEnumerable<Book>)store.Enumerable()).Take(100))
			{
				list.Add(book);
				Assert.IsNotNull(book);
			}
			int i = 0;
			foreach (var book in ((IEnumerable<Book>)store.Enumerable()).Take(100))
			{
				Assert.AreEqual(list[i], book);
				i++;
			}
		}
	}
}
