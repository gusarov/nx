using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Nx.Sample
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			DataContext = new MainViewModel();
		}
	}

	class MainViewModel
	{
		private readonly BookStore _store = new BookStore();

		public IEnumerable<Book> Books
		{
			get { return _store.Enumerable(); }
		}

		public IEnumerable<Book> Books2
		{
			get { return _store.Enumerable().Select(x=>x); }
		}

	}
}
