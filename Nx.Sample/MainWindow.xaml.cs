using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
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
using NX;
using NX.Internal;

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

	class MainViewModel : NxObservableObject
	{
		private readonly BookStore _store = new BookStore();

		public IEnumerable<Book> Books1
		{
			get { return _store.Enumerable(); }
		}

		public IEnumerable<BookViewModel> Books2
		{
			get { return _store.Enumerable().SelectO(x => new BookViewModel(x), SelectorBehaviour.Cache); }
		}

		public IEnumerable<BookViewModel> Books3
		{
			get { return _store.Enumerable().WhereO(x => x.ToString().Contains(_query)).TakeO(300).SelectO(x => new BookViewModel(x)); }
		}

		public object AllInt
		{
			get { return LargeEnumerable.AllInt; }
		}

		private string _query;
		public string Query
		{
			get { return _query; }
			set { _query = value; base.OnPropertyChanged("Query"); }
		}
	}
}
